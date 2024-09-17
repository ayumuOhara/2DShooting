using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] Transform enemySpawn;
    [SerializeField] List<SplineContainer> allSplines; // すべてのスプラインを保持するリスト

    private List<SplineContainer> availableSplines; // 現在使用可能なスプラインのリスト
    [SerializeField] SplineContainer BossRoute;
    public float wave;
    public float wavePercent;
    
    // ウェーブごとの敵設定と確率
    [SerializeField] List<GameObject> wave1Enemies;
    [SerializeField] List<float> wave1Probabilities; // 各敵の出現確率

    [SerializeField] List<GameObject> wave2Enemies;
    [SerializeField] List<float> wave2Probabilities; // 各敵の出現確率

    [SerializeField] List<GameObject> wave3Enemies;
    [SerializeField] List<float> wave3Probabilities; // 各敵の出現確率

    [SerializeField] GameObject Boss;
    bool isBoss = false;

    private float activeEnemyCount = 0; // 現在のアクティブな敵の数
    private const float maxEnemies = 4; // 最大のアクティブな敵の数

    void Start()
    {
        isBoss = false;
        wave = 1;
        wavePercent = 0;
        availableSplines = new List<SplineContainer>(allSplines); // 初期化
        StartCoroutine(WaveGene());
    }

    private void Update()
    {
        if (wavePercent >= 100 && wave < 3)
        {
            wave++;
            wavePercent = 0;
        }

        // ボス出現時
        if(wave == 3 && wavePercent >= 100 && isBoss == false)
        {
            isBoss = true;
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
            if(wave >= 3 && wavePercent >= 100)
            {
                StopCoroutine(WaveGene());
                yield break;
            }

            Random.InitState(System.Environment.TickCount);

            switch (wave)
            {
                case 1:
                    SpawnEnemies(wave1Enemies, wave1Probabilities); break;
                case 2:
                    SpawnEnemies(wave2Enemies, wave2Probabilities); break;
                case 3:
                    SpawnEnemies(wave3Enemies, wave3Probabilities); break;
                default:
                    Debug.Log("No settings available for this wave.");
                    break;
            }
            yield return new WaitForSeconds(5.0f);
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

        int rndSpawn = Random.Range(1, 4);
        float enemiesToSpawn = Mathf.Min(rndSpawn, maxEnemies - activeEnemyCount); // 出現する敵の数を制限

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            EnemyCntUp(1); // アクティブな敵の数を増やす
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

    public void OnEnemyDestroyed(float cnt)
    {
        activeEnemyCount = Mathf.Clamp(activeEnemyCount - cnt, 0, maxEnemies);
    }

    void EnemyCntUp(float cnt)
    {
        activeEnemyCount = Mathf.Clamp(activeEnemyCount + cnt, 0, maxEnemies);
    }

    public void WavePerAdd(float wavePer)
    {
        wavePercent = Mathf.Clamp(wavePercent + wavePer, 0, 100.0f);
    }
}
