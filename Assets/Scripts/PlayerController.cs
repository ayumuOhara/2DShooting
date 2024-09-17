using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStatus
{
    public float _lv { get; private set; }
    public float _hp { get; private set; }
    public float _maxHp { get; private set; }
    public float _energy { get; set; }
    public float _maxEnergy { get; private set; }
    public float _pow { get; set; }
    public float _powNormal { get; set; }
    public float _rate { get; set; }
    public int _coin { get; set; }

    // �X�e�[�^�X�����������郁�\�b�h
    public void SetStatus()
    {
        _lv = 1;
        _maxHp = 100.0f;
        _hp = _maxHp;
        _maxEnergy = 10.0f;
        _energy = _maxEnergy;
        _pow = 1.0f;
        _powNormal = _pow;
        _rate = 0.4f;
        _coin = 0;
    }

    public void LvUp(float lv)
    {
        _lv = Mathf.Clamp(_lv + lv, 1, 3);
    }

    // �G�l���M�[�����炷���\�b�h
    public void DecreaseEnergy(float amount)
    {
        _energy = Mathf.Clamp(_energy - amount, 0, _maxEnergy);
    }

    // �_���[�W���󂯂�HP�����炷���\�b�h
    public void TakeDamage(float damage)
    {
        _hp = Mathf.Clamp(_hp - damage, 0, _maxHp); // HP��0�ƍő�l�̊Ԃɐ���
    }

    // �v���C���[���|�ꂽ���ǂ������m�F���郁�\�b�h
    public bool IsDead()
    {
        return _hp <= 0;
    }
}

public class PlayerController : MonoBehaviour
{
    AudioManager _audio;
    public PlayerStatus _status { get; private set; }
    [SerializeField] GameObject retry;
    public GameObject _bulletN;
    public GameObject _bulletSp;
    public GameObject _defeatEff;
    public Transform _bulletspawn;
    private Animator _animator;
    const float LOAD_WIDTH = 4.5f;  // ���ړ��̑��x
    const float LOAD_HIGHT = 4.5f;  // �c�ړ��̑��x
    const float MOVE_MAX_X = 1.7f;  // ���ړ��̌��E
    const float MOVE_MAX_Y = 4f;    // �c�ړ��̌��E
    Vector3 previousPos, currentPos;

    // �R���|�[�l���g�擾
    void GetCom()
    {
        _animator = gameObject.GetComponent<Animator>();
        EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
        _audio = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    // ���\�[�X�v���t�@�u�擾
    void GetResources()
    {
        // �e�ۂ̃��\�[�X�ǂݍ���
        _bulletN = (GameObject)Resources.Load("BulletN");
        _bulletSp = (GameObject)Resources.Load("BulletSP");
    }

    void Start()
    {
        GetCom();
        GetResources();
        transform.position = new Vector3(0, -1.3f, 0);

        _status = new PlayerStatus();
        _status.SetStatus();  // �X�e�[�^�X�̏�����

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((x) =>
        {
            EnergyShot();
        });
        StartCoroutine(Shotballs());
    }

    void Update()
    {
        PlayerMove();
        StatusAdd();
        if (Input.GetMouseButtonDown(1))  // �E�N���b�N�ŃG�l���M�[�V���b�g
        {
            if (_status._energy > 0)
            {
                _status.DecreaseEnergy(1.0f);  // �G�l���M�[�����炷
                Instantiate(_bulletSp, _bulletspawn.position, Quaternion.identity);
            }
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
            float newY = Mathf.Clamp(transform.localPosition.y + diffDistanceY, -MOVE_MAX_Y + 1.0f, MOVE_MAX_Y);
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
            Instantiate(_bulletN, _bulletspawn.position, Quaternion.identity);
            yield return new WaitForSeconds(_status._rate);
        }
    }

    public void EnergyShot()
    {
        if (_status._energy > 0)
        {
            _status.DecreaseEnergy(1.0f);
            Instantiate(_bulletSp, _bulletspawn.position, Quaternion.identity);
        }
    }

    // �_���[�W���󂯂����̏���
    public void ApplyDamage(float damage)
    {
        _audio.PlaySound("PlayerDamage");
        _status.TakeDamage(damage);  // �_���[�W���󂯂�
        if (_status.IsDead())  // ���S����
        {
            _audio.PlaySound("PlayerDefeat");
            Instantiate(_defeatEff, transform.position, transform.rotation);
            retry.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    void StatusAdd()
    {
        _status._pow = _status._powNormal + _status._coin * 0.1f;
        switch(_status._lv)
        {
            case 1:
                _status._rate = 0.4f;
                break;
            case 2:
                _status._rate = 0.3f;
                break;
            case 3:
                _status._rate = 0.2f;
                break;
            case 4:
                _status._rate = 0.1f;
                break;
        }
    }
}
