using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyGenerator : MonoBehaviour
{
    AudioManager _audio;
    GameManager _gameManager;

    [SerializeField] Transform enemySpawn;
    [SerializeField] List<SplineContainer> allSplines; // すべてのスプラインを保持するリスト

    private List<SplineContainer> availableSplines; // 現在使用可能なスプラインのリスト
    [SerializeField] SplineContainer BossRoute;
    public float wave;              // ウェーブ数
    public float defeatedEnemyCnt;  // 倒した敵の数
    public float waveMaxPer;        // ウェーブ事の倒す敵の目標数
    
    // ウェーブごとの敵設定と確率
    [SerializeField] List<GameObject> wave1Enemies;
    [SerializeField] List<float> wave1Probabilities; // 各敵の出現確率

    [SerializeField] List<GameObject> wave2Enemies;
    [SerializeField] List<float> wave2Probabilities; // 各敵の出現確率

    [SerializeField] List<GameObject> wave3Enemies;
    [SerializeField] List<float> wave3Probabilities; // 各敵の出現確率

    [SerializeField] GameObject Boss;
    public bool isBoss = false;

    private float activeEnemyCount = 0; // 現在のアクティブな敵の数
    private float maxEnemies = 4; // 最大のアクティブな敵の数
    private float spawnSpan = 0;

    void Start()
    {
        isBoss = false;
        wave = 1;
        defeatedEnemyCnt = 0;
        spawnSpan = 5.0f;
        _audio = GameObject.Find("Speaker").GetComponent<AudioManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        availableSplines = new List<SplineContainer>(allSplines); // 初期化
        _gameManager.FadeAnimation(wave);
        StartCoroutine(WaveGene());
    }

    private void Update()
    {
        activeEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if(defeatedEnemyCnt >= waveMaxPer && wave <= 3)
        {
            defeatedEnemyCnt = 0;
            wave++;
            _gameManager.FadeAnimation(wave);
        }
        // ボス出現時
        if(wave > 3 && isBoss == false)
        {
            isBoss = true;
            _audio.PlayBGM("BOSS");
            _audio.PlayBGM("InGameStop");
            GameObject spawnedEnemy = Instantiate(Boss, enemySpawn.position, Quaternion.identity);
            SplineAnimate splineAnimate = spawnedEnemy.GetComponent<SplineAnimate>();
            if (splineAnimate != null)
            {
                splineAnimate.Container = BossRoute;
                splineAnimate.Play();
            }
            else
            {
                Debug.LogError("SplineAnimateコンポーネントが敵にアタッチされていません。");
            }
        }
    }

    IEnumerator WaveGene()
    {
        while (true)
        {
            if(wave > 3)
            {
                StopCoroutine(WaveGene());
                yield break;
            }

            Random.InitState(System.Environment.TickCount);

            switch (wave)
            {
                case 1:
                    waveMaxPer = 15.0f;
                    SpawnEnemies(wave1Enemies, wave1Probabilities); break;
                case 2:
                    waveMaxPer = 40.0f;
                    maxEnemies = 6;
                    spawnSpan = 3.5f;
                    SpawnEnemies(wave2Enemies, wave2Probabilities); break;
                case 3:
                    waveMaxPer = 65.0f;
                    maxEnemies = 8;
                    spawnSpan = 2.5f;
                    SpawnEnemies(wave3Enemies, wave3Probabilities); break;
                default:
                    break;
            }
            yield return new WaitForSeconds(spawnSpan);
        }
    }

    void InitializeSplineList()
    {
        // 使用可能スプラインリストを初期化
        availableSplines = new List<SplineContainer>(allSplines);
        availableSplines.Sort((a, b) => Random.Range(-1, 2));
    }

    void SpawnEnemies(List<GameObject> enemySet, List<float> probabilities)
    {
        if (activeEnemyCount >= maxEnemies)
        {
            Debug.Log("最大のアクティブな敵数に達しました。これ以上出現できません。");
            return;
        }

        int rndSpawn = Random.Range(2, 4);
        float enemiesToSpawn = Mathf.Min(rndSpawn, maxEnemies - activeEnemyCount); // 出現する敵の数を制限

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (enemySet == null || enemySet.Count == 0)
            {
                Debug.LogError("敵のセットが無効です。");
                return;
            }

            GameObject enemy = SelectEnemyByProbability(enemySet, probabilities);
            if (enemy == null)
            {
                Debug.LogError("選択した敵のプレハブが無効です。");
            }

            GameObject spawnedEnemy = Instantiate(enemy, enemySpawn.position, Quaternion.identity);

            if (availableSplines.Count < 1)
            {
                InitializeSplineList();
            }
            if (availableSplines.Count > 0)
            {
                SplineContainer selectedSpline = availableSplines[0];
                availableSplines.RemoveAt(0);

                SplineAnimate splineAnimate = spawnedEnemy.GetComponent<SplineAnimate>();
                if (splineAnimate != null)
                {
                    splineAnimate.Container = selectedSpline;
                    splineAnimate.Play();
                }
                else
                {
                    Debug.LogError("SplineAnimateコンポーネントが敵にアタッチされていません。");
                }
            }
        }
    }

    GameObject SelectEnemyByProbability(List<GameObject> enemySet, List<float> probabilities)
    {
        if (enemySet.Count != probabilities.Count)
        {
            Debug.LogError("敵のセットと確率リストのサイズが一致しません。");
            return null;
        }

        float total = 0;
        foreach (float prob in probabilities)
        {
            total += prob;
        }

        float randomPoint = Random.value * total;
        for (int i = 0; i < probabilities.Count; i++)
        {
            if (randomPoint < probabilities[i])
            {
                return enemySet[i];
            }
            else
            {
                randomPoint -= probabilities[i];
            }
        }

        return enemySet[enemySet.Count - 1]; // 万が一抽選できなかった場合、最後の敵を返す
    }

    public void WavePerAdd(float per)
    {
        defeatedEnemyCnt = Mathf.Clamp(defeatedEnemyCnt + per, 0, waveMaxPer);
    }
}
