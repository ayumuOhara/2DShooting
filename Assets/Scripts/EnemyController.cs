using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyStatus
{
    public string _name { get; private set; }
    public float _hp { get; private set; }
    public float _maxHp { get; private set; }
    public float _pow { get; private set; }
    public float _speed { get; private set; }
    public float _rate { get; private set; }

    // �X�e�[�^�X��ݒ肷�郁�\�b�h
    public void SetStatus(string name, float hp, float pow, float speed, float rate)
    {
        _name = name;
        _hp = hp;
        _maxHp = hp;
        _pow = pow;
        _speed = speed;
        _rate = rate;
    }

    // �_���[�W���󂯂郁�\�b�h
    public void TakeDamage(float damage)
    {
        _hp = Mathf.Max(_hp - damage, 0); // HP��0�����ɂ��Ȃ�
    }

    // �G���|���ꂽ���ǂ������m�F���郁�\�b�h
    public bool IsDead()
    {
        return _hp <= 0;
    }
}

public class EnemyController : MonoBehaviour
{
    public EnemyStatus _status { get; private set; }

    PlayerController _player;
    GameManager _gameManager;
    EnemyGenerator _enemyGene;
    [SerializeField] EnemyList enemyStatus;
    EnemyData enemy;
    AudioManager _audio;

    Rigidbody2D _rb;

   private SplineAnimate _splineAnimate;

    [SerializeField] GameObject _defeatEff;
    public GameObject _bullet1;
    public GameObject _bullet2;
    public GameObject _bullet3;
    public GameObject _bullet4;
    public GameObject _powerItem;
    public GameObject _energyItem;
    public GameObject _coin;
    string answerTag;
    [SerializeField]
    bool _stopSpline = false;
    float atkSpan = 0;
    Vector3 playerPos;

    // �R���|�[�l���g�擾
    void GetCom()
    {
        _splineAnimate = GetComponent<SplineAnimate>();
        _status = new EnemyStatus();
        _enemyGene = GameObject.Find("EnemyGenerator").GetComponent<EnemyGenerator>();
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audio = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        _rb = GetComponent<Rigidbody2D>();
    }
    // ���\�[�X�v���t�@�u�擾
    void GetResources()
    {
        // �G�̒e�̎��
        _bullet1 = (GameObject)Resources.Load("Bullet1");
        _bullet2 = (GameObject)Resources.Load("Bullet2");
        _bullet3 = (GameObject)Resources.Load("Bullet3");
        _bullet4 = (GameObject)Resources.Load("Bullet4");

        // �h���b�v����A�C�e��
        _powerItem = (GameObject)Resources.Load("PowerItem");
        _energyItem = (GameObject)Resources.Load("EnergyItem");
        _coin = (GameObject)Resources.Load("Coin");
    }
    // ScriptableObject����f�[�^�擾
    void GetStatus()
    {
        answerTag = this.gameObject.tag;
        // �^�O�ɉ������G�f�[�^���������ăX�e�[�^�X��ݒ�
        switch (answerTag)
        {
            case "EnemyS":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "EnemyS");
                break;
            case "EnemyM":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "EnemyM");
                break;
            case "EnemyL":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "EnemyL");
                break;
            case "EnemyE":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "EnemyE");
                break;
            case "Boss":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "Boss");
                break;
        }
        _status.SetStatus(enemy._name, enemy._hp, enemy._pow, enemy._speed, enemy._rate);
    }

    // Start is called before the first frame update
    void Start()
    {
        GetCom();
        GetResources();
        GetStatus();
        
        _stopSpline = false;
        StartCoroutine(EnemyAttack());
        if (answerTag == "Boss")
        {
            StartCoroutine(BulletHSpawn());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �X�v���C���̐i�s�x���`�F�b�N
        if (_splineAnimate != null && _splineAnimate.NormalizedTime >= 1.0f)
        {
            if(_player._status._hp > 0)
            {
                _stopSpline = true;
            }
        }
        // �{�X��HP�̊����ōU���p�x��ύX
        if(_status._hp / _status._maxHp <= 0.2f)
        {
            atkSpan = 0.3f;
        }
        else if (_status._hp / _status._maxHp <= 0.5f)
        {
            atkSpan = 0.5f;
        }
        else
        {
            atkSpan = 1.0f;
        }
        // �{�X�o�����ɎG��������
        if (_enemyGene.wave >= 3 && _enemyGene.wavePercent >= 100)
        {
            switch (this.gameObject.tag)
            {
                case "EnemyS":
                case "EnemyM":
                case "EnemyL":
                case "EnemyE":
                    Destroy(this.gameObject);
                    break;
            }
        }
    }

    // �_���[�W����
    public void ApplyDamage(float damage)
    {
        _status.TakeDamage(damage);
        if (_status.IsDead())
        {
            _audio.PlaySound("EnemyDefeat");
            _enemyGene.WavePerAdd(5.0f);
            _enemyGene.OnEnemyDestroyed(1);
            Instantiate(_defeatEff, this.gameObject.transform.position, this.gameObject.transform.rotation);
            Destroy(this.gameObject);
            DropItems();
            if(answerTag == "Boss")
            {
                _gameManager.Clear();
            }
        }
    }

    // �G�l�~�[�̍U��
    IEnumerator EnemyAttack()
    {
        while (true)
        {
            if(_stopSpline)
            {
                switch (answerTag)
                {
                    case "EnemyS":
                        // �P��
                        BulletSpawn(_bullet1,transform.position);
                        break;
                    case "EnemyM":
                        // �O�_�o�[�X�g
                        int i = 0;
                        do
                        {
                            BulletSpawn(_bullet3, transform.position);
                            i++;
                            yield return new WaitForSeconds(0.2f);
                        } while (i < 3);
                        break;
                    case "EnemyL":
                        // �񔭓�������
                        BulletMulti(_bullet2, 0.3f);
                        break;
                    case "EnemyE":
                        // �P��
                        BulletSpawn(_bullet1, transform.position);
                        break;
                    case "Boss":
                        int k = 0;
                        do
                        {
                            BulletMulti(_bullet2, 1.0f);
                            BulletMulti(_bullet2, 0.7f);
                            k++;
                            yield return new WaitForSeconds(0.2f);
                        } while (k < 3);

                        yield return new WaitForSeconds(atkSpan);

                        int j = 0;
                        do
                        {
                            BulletMulti(_bullet3, 1.3f);
                            j++;
                            yield return new WaitForSeconds(0.2f);
                        } while(j < 5);
                        break;
                }
                yield return new WaitForSeconds(_status._rate);
            }
            else
            {
                yield return null;
            }
        }
    }

    // �G�l�~�[�̒e���C���X�^���X��
    void BulletSpawn(GameObject bulletType,Vector3 spawnPoint)
    {
        GameObject bullet = Instantiate(bulletType, spawnPoint, Quaternion.Euler(180,0,0));
        EnemyBullets bulletScript = bullet.GetComponent<EnemyBullets>();
        bulletScript._enemy = this;
    }

    // �G�l�~�[�̒e��2�J���ɃC���X�^���X��
    void BulletMulti(GameObject bulletType, float direction)
    {
        Vector3[] offsets = { new Vector3(direction, 0, 0), new Vector3(-direction, 0, 0) };

        foreach (Vector3 offset in offsets)
        {
            Vector3 newPosition = transform.position + offset;
            BulletSpawn(bulletType, newPosition);
        }
    }

    // �{�X�p�U���p�^�[��
    IEnumerator BulletHSpawn()
    {
        while (true)
        {
            if(_stopSpline)
            {
                BulletSpawn(_bullet4, transform.position);
                yield return new WaitForSeconds(atkSpan);
            }
            else
            {
                yield return null;
            }
        }
    }

    void DropItems()
    {
        int rnd = Random.Range(1, 101);

        if(_status.IsDead())
        {
            switch(answerTag)
            {
                case "EnemyS":
                    if(rnd >= 50)   // 50%�ŃR�C������
                    {
                        Instantiate(_coin, transform.position, transform.rotation);
                    }
                    break;
                case "EnemyM":
                    if(rnd >= 60 && rnd <= 100)     // 40%��POW�A�C�e������
                    {
                        Instantiate(_powerItem, transform.position, transform.rotation);
                    }
                    else if(rnd >= 20)              // 40%�ŃR�C������
                    {
                        Instantiate(_coin,transform.position, transform.rotation);
                    }
                    break;
                case "EnemyL":
                    if (rnd >= 50 && rnd <= 100)    // 50%��POW�A�C�e������
                    {
                        Instantiate(_powerItem, transform.position, transform.rotation);
                    }
                    else if (rnd >= 20)             // 30%�ŃR�C������
                    {
                        Instantiate(_coin, transform.position, transform.rotation);
                    }
                    break;
                case "EnemyE":                      // 100%�ŃG�i�W�[�A�C�e������
                    Instantiate(_energyItem, transform.position, transform.rotation);
                    break;
            }
        }
    }
}
