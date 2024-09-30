using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class PlayerStatus
{
    private GameManager _gameManager;

    public float _lv { get; private set; }
    public float _hp { get; private set; }
    public float _maxHp { get; private set; }
    public float _bonusHp { get; private set; }
    public float _bonusMaxHp { get; private set; }
    public float _pow { get; set; }
    public float _powNormal { get; set; }
    public float _rate { get; set; }
    public int _coin { get; set; }

    // �X�e�[�^�X�����������郁�\�b�h
    public void SetStatus(GameManager gameManager)
    {
        _lv = 1;
        _maxHp = 50.0f;
        _hp = _maxHp;
        _bonusMaxHp = 20.0f;
        _bonusHp = 0;
        _pow = 1.0f;
        _powNormal = _pow;
        _rate = 0.4f;
        _coin = 0;
        _gameManager = gameManager;
    }

    // ���x�����擾
    public void LvUp(float lv)
    {
        if(_lv >= 7)
        {
            _lv = 4;
            _gameManager.BulletTextAnimation(_lv);
        }
        else
        {
            _lv = Mathf.Clamp(_lv + lv, 1, 7);
            _gameManager.BulletTextAnimation(_lv);
        }
    }

    // �{�[�i�XHP���擾
    public void GetBonusHP(float bonus)
    {
        _bonusHp = Mathf.Clamp(_bonusHp + bonus, 0, _bonusMaxHp);
    }

    public void DrainHP(float heel)
    {
        _hp = Mathf.Clamp(_hp + heel, 0, _maxHp);
    }

    public void TakeDamage(float damage)
    {
        // �{�[�i�XHP���D��I�Ɍ���
        if(_bonusHp > 0)
        {
            _bonusHp -= damage;
            if(_bonusHp < 0)        // �{�[�i�XHP�𒴉߂�����HP�Ƀ_���[�W
            {
                TakeDamage(-(_bonusHp));
                _bonusHp = 0;
            }
        }
        else
        {
            _hp = Mathf.Clamp(_hp - damage, 0, _maxHp);
        }
    }

    // �v���C���[���|�ꂽ���ǂ������m�F���郁�\�b�h
    public bool IsDead()
    {
        return _hp <= 0;
    }
}

public class PlayerController : MonoBehaviour
{
    public PlayerStatus _status { get; private set; }
    PlayerHpVision _hpVision;       // �v���C���[�̉������ꂽ�����蔻��
    GameManager _gameManager;
    [SerializeField] List<GameObject> Bullets;
    GameObject _bullet;
    [SerializeField] GameObject _defeatEff;
    [SerializeField] GameObject _barrierEff;
    private Animator _animator;
    AudioManager _audio;
    const float LOAD_WIDTH = 6f;      // ���ړ��̑��x
    const float LOAD_HIGHT = 6f;      // �c�ړ��̑��x
    const float MOVE_MAX_X = 1.7f;    // ���ړ��̌��E
    const float MOVE_MAX_Y = 4.5f;    // �c�ړ��̌��E
    Vector3 previousPos, currentPos;
    SpriteRenderer sr;
    Rigidbody2D _rb;

    // �R���|�[�l���g�擾
    void GetCom()
    {
        _hpVision = GameObject.Find("CollisionDetection").GetComponent<PlayerHpVision>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _animator = this.gameObject.GetComponent<Animator>();
        _audio = GameObject.Find("Speaker").GetComponent<AudioManager>();
        sr = GetComponent<SpriteRenderer>();
        _rb = this.gameObject.GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        GetCom();
        transform.position = new Vector3(0, -1.3f, 0);  // �v���C���[�̏����ʒu
        _bullet = Bullets[0];
        _status = new PlayerStatus();
        _status.SetStatus(_gameManager);  // �X�e�[�^�X�̏�����
        StartCoroutine(Shotballs());
    }

    void Update()
    {
        if(!_status.IsDead())
        {
            PlayerMove();
        }
    }

    void PlayerMove()
    {
        // �X���C�v�ɂ��ړ�����
        if (Input.GetMouseButtonDown(0))
        {
            previousPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            currentPos = Input.mousePosition;
            float diffDistanceX = (currentPos.x - previousPos.x) / Screen.width * LOAD_WIDTH;
            float diffDistanceY = (currentPos.y - previousPos.y) / Screen.width * LOAD_HIGHT;
            if (currentPos.x - previousPos.x > 0)
            {
                _animator.SetBool("RightMove", true);
                _animator.SetBool("LeftMove", false);
            }
            else if (currentPos.x - previousPos.x < 0)
            {
                _animator.SetBool("LeftMove", true);
                _animator.SetBool("RightMove", false);
            }
            float newX = Mathf.Clamp(transform.localPosition.x + diffDistanceX, -MOVE_MAX_X, MOVE_MAX_X);
            float newY = Mathf.Clamp(transform.localPosition.y + diffDistanceY, -MOVE_MAX_Y, MOVE_MAX_Y);
            transform.localPosition = new Vector3(newX, newY, 0);
            previousPos = currentPos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _animator.SetBool("LeftMove", false);
            _animator.SetBool("RightMove", false);
        }
    }
    IEnumerator Shotballs()
    {
        while (true)
        {
            Instantiate(_bullet, transform.position, _bullet.transform.rotation);
            yield return new WaitForSeconds(_status._rate);
        }
    }

    public void ApplyDamage(float damage)
    {
        _audio.PlaySound("PlayerDamage");
        _status.TakeDamage(damage);                             // �_���[�W���󂯂�
        _hpVision.ColorChange(_status._hp, _status._maxHp);     // HP�̊����ŁZ�̐F��ύX
        if (_status.IsDead())                                   // ���S����
        {
            StartCoroutine(PlayerDead());
        }
    }

    IEnumerator PlayerDead()
    {
        Destroy(_rb);
        Time.timeScale = 0.1f; 
        _audio.PlaySound("PlayerDefeat");
        Instantiate(_defeatEff, transform.position, transform.rotation);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
        yield return new WaitForSecondsRealtime(3.0f);
        Time.timeScale = 1.0f;
        Destroy(this.gameObject);
    }

    public void StatusAdd()
    {
        float powAdd = 0.1f;
        _status._pow = _status._powNormal + _status._coin * powAdd;
        switch(_status._lv)
        {
            case 1:
                _status._rate = 0.4f;
                break;
            case 2:
                _status._rate = 0.2f;
                break;
            case 3:
                _status._rate = 0.1f;
                break;
            case 4:
                _bullet = Bullets[1];   // �h���C���e
                _status._rate = 0.15f;
                break;
            case 5:
                _bullet = Bullets[2];   // �z�[�~���O�e
                _status._rate = 0.25f;
                break;
            case 6:
                _bullet = Bullets[3];   // �ђʒe
                _status._rate = 0.15f;
                break;
            case 7:
                _bullet = Bullets[4];   // �������e
                _status._rate = 0.06f;
                break;
        }
    }
}
