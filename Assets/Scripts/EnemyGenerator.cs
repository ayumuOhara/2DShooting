using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] Transform enemySpawn;
    [SerializeField] List<SplineContainer> allSplines; // ���ׂẴX�v���C����ێ����郊�X�g

    private List<SplineContainer> availableSplines; // ���ݎg�p�\�ȃX�v���C���̃��X�g
    [SerializeField] SplineContainer BossRoute;
    public float wave;
    public float wavePercent;
    
    // �E�F�[�u���Ƃ̓G�ݒ�Ɗm��
    [SerializeField] List<GameObject> wave1Enemies;
    [SerializeField] List<float> wave1Probabilities; // �e�G�̏o���m��

    [SerializeField] List<GameObject> wave2Enemies;
    [SerializeField] List<float> wave2Probabilities; // �e�G�̏o���m��

    [SerializeField] List<GameObject> wave3Enemies;
    [SerializeField] List<float> wave3Probabilities; // �e�G�̏o���m��

    [SerializeField] GameObject Boss;
    bool isBoss = false;

    private float activeEnemyCount = 0; // ���݂̃A�N�e�B�u�ȓG�̐�
    private const float maxEnemies = 4; // �ő�̃A�N�e�B�u�ȓG�̐�

    void Start()
    {
        isBoss = false;
        wave = 1;
        wavePercent = 0;
        availableSplines = new List<SplineContainer>(allSplines); // ������
        StartCoroutine(WaveGene());
    }

    private void Update()
    {
        if (wavePercent >= 100 && wave < 3)
        {
            wave++;
            wavePercent = 0;
        }

        // �{�X�o����
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
                Debug.LogError("SplineAnimate�R���|�[�l���g���G�ɃA�^�b�`����Ă��܂���B");
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

        int rndSpawn = Random.Range(1, 4);
        float enemiesToSpawn = Mathf.Min(rndSpawn, maxEnemies - activeEnemyCount); // �o������G�̐��𐧌�

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            EnemyCntUp(1); // �A�N�e�B�u�ȓG�̐��𑝂₷
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
