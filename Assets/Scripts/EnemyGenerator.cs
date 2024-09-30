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
    [SerializeField] List<SplineContainer> allSplines; // ���ׂẴX�v���C����ێ����郊�X�g

    private List<SplineContainer> availableSplines; // ���ݎg�p�\�ȃX�v���C���̃��X�g
    [SerializeField] SplineContainer BossRoute;
    public float wave;              // �E�F�[�u��
    public float defeatedEnemyCnt;  // �|�����G�̐�
    public float waveMaxPer;        // �E�F�[�u���̓|���G�̖ڕW��
    
    // �E�F�[�u���Ƃ̓G�ݒ�Ɗm��
    [SerializeField] List<GameObject> wave1Enemies;
    [SerializeField] List<float> wave1Probabilities; // �e�G�̏o���m��

    [SerializeField] List<GameObject> wave2Enemies;
    [SerializeField] List<float> wave2Probabilities; // �e�G�̏o���m��

    [SerializeField] List<GameObject> wave3Enemies;
    [SerializeField] List<float> wave3Probabilities; // �e�G�̏o���m��

    [SerializeField] GameObject Boss;
    public bool isBoss = false;

    private float activeEnemyCount = 0; // ���݂̃A�N�e�B�u�ȓG�̐�
    private float maxEnemies = 4; // �ő�̃A�N�e�B�u�ȓG�̐�
    private float spawnSpan = 0;

    void Start()
    {
        isBoss = false;
        wave = 1;
        defeatedEnemyCnt = 0;
        spawnSpan = 5.0f;
        _audio = GameObject.Find("Speaker").GetComponent<AudioManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        availableSplines = new List<SplineContainer>(allSplines); // ������
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
        // �{�X�o����
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
                Debug.LogError("SplineAnimate�R���|�[�l���g���G�ɃA�^�b�`����Ă��܂���B");
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
        // �g�p�\�X�v���C�����X�g��������
        availableSplines = new List<SplineContainer>(allSplines);
        availableSplines.Sort((a, b) => Random.Range(-1, 2));
    }

    void SpawnEnemies(List<GameObject> enemySet, List<float> probabilities)
    {
        if (activeEnemyCount >= maxEnemies)
        {
            Debug.Log("�ő�̃A�N�e�B�u�ȓG���ɒB���܂����B����ȏ�o���ł��܂���B");
            return;
        }

        int rndSpawn = Random.Range(2, 4);
        float enemiesToSpawn = Mathf.Min(rndSpawn, maxEnemies - activeEnemyCount); // �o������G�̐��𐧌�

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (enemySet == null || enemySet.Count == 0)
            {
                Debug.LogError("�G�̃Z�b�g�������ł��B");
                return;
            }

            GameObject enemy = SelectEnemyByProbability(enemySet, probabilities);
            if (enemy == null)
            {
                Debug.LogError("�I�������G�̃v���n�u�������ł��B");
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
                    Debug.LogError("SplineAnimate�R���|�[�l���g���G�ɃA�^�b�`����Ă��܂���B");
                }
            }
        }
    }

    GameObject SelectEnemyByProbability(List<GameObject> enemySet, List<float> probabilities)
    {
        if (enemySet.Count != probabilities.Count)
        {
            Debug.LogError("�G�̃Z�b�g�Ɗm�����X�g�̃T�C�Y����v���܂���B");
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

        return enemySet[enemySet.Count - 1]; // �����ꒊ�I�ł��Ȃ������ꍇ�A�Ō�̓G��Ԃ�
    }

    public void WavePerAdd(float per)
    {
        defeatedEnemyCnt = Mathf.Clamp(defeatedEnemyCnt + per, 0, waveMaxPer);
    }
}
