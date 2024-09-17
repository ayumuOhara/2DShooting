using UnityEngine;
public class EnemyBulletStatus
{
    public float _speed { get; private set; }

    public void SetStatus()
    {
        _speed = 1.0f;
    }

    public void SetPowerAndSpeed(float speed)
    {
        _speed = speed;
    }
}

public class EnemyBullets : MonoBehaviour
{
    EnemyBulletStatus _bulletStatus;
    PlayerController _player;
    public EnemyController _enemy;
    Rigidbody2D _rb;
    string bulletType;  // �e�̎��
    Vector3 _playerPos;
    private Vector3 _targetDirection; // �e���i�ޕ���

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bulletStatus = new EnemyBulletStatus();
        _bulletStatus.SetStatus();
        bulletType = this.gameObject.tag;
        if(GameObject.Find("Player"))
        _playerPos = GameObject.Find("Player").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (bulletType)
        {
            case "Bullet1":
            case "Bullet2":
            case "Bullet3":
                BulletMove(5.0f);
                break;
            case "Bullet4":
                RockOnBullet(4.0f);
                break;
        }
    }
    // ���i����e
    private void BulletMove(float speed)
    {
        _bulletStatus.SetPowerAndSpeed(speed);
        transform.Translate(new Vector3(0, -_bulletStatus._speed * Time.deltaTime, 0), Space.World);
    }
    // �v���C���[�Ɍ������Đi�s����e
    void RockOnBullet(float speed)
    {
        // �e�̑��x��ݒ�
        _bulletStatus.SetPowerAndSpeed(speed);

        // ����̂݃^�[�Q�b�g�̕������v�Z
        if (_targetDirection == Vector3.zero) // �܂��������v�Z����Ă��Ȃ��ꍇ
        {
            _targetDirection = (_playerPos - transform.position).normalized;
        }

        // �e�̈ʒu���v�Z���ꂽ�����Ɍ������Ĉړ���������
        transform.Translate(_targetDirection * _bulletStatus._speed * Time.deltaTime,Space.World);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            _player = collision.gameObject.GetComponent<PlayerController>();
            Destroy(gameObject);
            // �G�̃p���[����_���[�W���󂯂鏈�������s
            float damage = _enemy._status._pow;  // _enemy.Status._pow���g�p���ă_���[�W�v�Z
            _player.ApplyDamage(damage);
        }
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);  // ��ʊO�ɏo����I�u�W�F�N�g���폜
    }
}
