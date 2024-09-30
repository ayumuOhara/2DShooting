using UnityEngine;

public class BulletStatus
{
    public float _pow { get; set; }
    public float _speed { get; private set; }
    public bool _through { get; private set; }
    public bool _homing { get; private set; }
    public bool _drain {  get; private set; }

    public void SetStatus(float pow, float speed,bool through,bool homing,bool drain)
    {
        _pow = pow;
        _speed = speed;
        _through = through;
        _homing = homing;
        _drain = drain;
    }
}

public class BulletController : MonoBehaviour
{
    BulletStatus _bulletStatus;
    PlayerController _player;
    EnemyController _enemy;
    [SerializeField]
    GameObject _HitEffect;
    [SerializeField] BulletList bullStatus;
    BulletData bullet;
    float rotationSpeed = 7.0f;

    // コンポーネント取得
    void GetCom()
    {
        GameObject player = GameObject.Find("Player");
        _player = player.GetComponent<PlayerController>();
        _bulletStatus = new BulletStatus();
    }
    // ScriptableObjectからデータ取得
    void GetStatus()
    {
        switch (this.gameObject.name)
        {
            case "BulletN(Clone)":
                bullet = bullStatus._bulletData.Find(bullet => bullet._bulletName == "BulletN");
                break;
            case "Drain(Clone)":
                bullet = bullStatus._bulletData.Find(bullet => bullet._bulletName == "Drain");
                break;
            case "Cross(Clone)":
                bullet = bullStatus._bulletData.Find(bullet => bullet._bulletName == "Cross");
                break;
            case "Pulse(Clone)":
                bullet = bullStatus._bulletData.Find(bullet => bullet._bulletName == "Pulse");
                break;
            case "Wave(Clone)":
                bullet = bullStatus._bulletData.Find(bullet => bullet._bulletName == "Wave");
                break;
        }

        _bulletStatus.SetStatus(bullet._pow,bullet._speed,bullet._through,bullet._homing,bullet._drain);
    }
    // Start is called before the first frame update
    void Start()
    {
        GetCom();
        GetStatus();
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.gameObject.name)
        {
            case "BulletN(Clone)":
                transform.Translate(new Vector3(0, _bulletStatus._speed * Time.deltaTime, 0));
                break;
            case "Drain(Clone)":
                transform.Translate(new Vector3(_bulletStatus._speed * Time.deltaTime, 0, 0));
                break;
            case "Cross(Clone)":
                RockOnBullet();
                break;
            case "Pulse(Clone)":
                transform.Translate(new Vector3(_bulletStatus._speed * Time.deltaTime, 0, 0));
                break;
            case "Wave(Clone)":
                transform.Translate(new Vector3(_bulletStatus._speed * Time.deltaTime, 0, 0));
                break;
        }
    }

    // エネミーに向かって進行する弾
    void RockOnBullet()
    {
        // ターゲットとなる一番近い敵を取得
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            // ターゲット方向の更新
            Vector3 targetDirection = (nearestEnemy.transform.position - transform.position).normalized;

            // ターゲット方向に向けてZ軸のみ回転
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg; // 角度を計算してZ軸回転に変換
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 弾をX軸方向に進行させる
        transform.Translate(Vector3.right * _bulletStatus._speed * Time.deltaTime); // X軸方向に移動
    }

    // 最も近い敵を探すメソッド
    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // "Enemy" タグのオブジェクトをすべて取得
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        // 弾の現在位置
        Vector3 currentPosition = transform.position;

        // すべての敵との距離を計算し、一番近い敵を探す
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy; // 最も近い敵を返す (見つからなければ null)
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);  // 画面外に出たらオブジェクトを削除
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                Instantiate(_HitEffect,this.gameObject.transform.position,this.gameObject.transform.rotation);
                _enemy = collision.gameObject.GetComponent<EnemyController>();
                if(bullet._through == false)
                {
                    Destroy(gameObject);
                }
                // プレイヤーのパワーと弾の威力を合算してダメージ計算
                float damage = _player._status._pow + _bulletStatus._pow;
                _enemy.ApplyDamage(damage);  // 敵にダメージを与える
                if (_bulletStatus._drain)
                {
                    _player._status.DrainHP(damage * 0.1f);
                }
                break;
        }
    }
}
