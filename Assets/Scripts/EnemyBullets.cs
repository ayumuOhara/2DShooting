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
    string bulletType;  // 弾の種類
    Vector3 _playerPos;
    private Vector3 _targetDirection; // 弾が進む方向

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
    // 直進する弾
    private void BulletMove(float speed)
    {
        _bulletStatus.SetPowerAndSpeed(speed);
        transform.Translate(new Vector3(0, -_bulletStatus._speed * Time.deltaTime, 0), Space.World);
    }
    // プレイヤーに向かって進行する弾
    void RockOnBullet(float speed)
    {
        // 弾の速度を設定
        _bulletStatus.SetPowerAndSpeed(speed);

        // 初回のみターゲットの方向を計算
        if (_targetDirection == Vector3.zero) // まだ方向が計算されていない場合
        {
            _targetDirection = (_playerPos - transform.position).normalized;
        }

        // 弾の位置を計算された方向に向かって移動し続ける
        transform.Translate(_targetDirection * _bulletStatus._speed * Time.deltaTime,Space.World);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            _player = collision.gameObject.GetComponent<PlayerController>();
            Destroy(gameObject);
            // 敵のパワーからダメージを受ける処理を実行
            float damage = _enemy._status._pow;  // _enemy.Status._powを使用してダメージ計算
            _player.ApplyDamage(damage);
        }
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);  // 画面外に出たらオブジェクトを削除
    }
}
