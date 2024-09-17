using UnityEngine;

public class BulletStatus
{
    public float _pow { get; set; }
    public float _speed { get; private set; }

    public void SetStatus()
    {
        _speed = 8.0f;
    }

    public void SetPowAndSpeed(float pow, float speed)
    {
        _pow = pow;
        _speed = speed;
    }
}

public class BulletController : MonoBehaviour
{
    BulletStatus _bulletStatus;
    PlayerController _player;
    EnemyController _enemy;
    GameObject _HitEffect;
    string bulletType;  // 弾の種類

    // コンポーネント取得
    void GetCom()
    {
        GameObject player = GameObject.Find("Player");
        _player = player.GetComponent<PlayerController>();
        _bulletStatus = new BulletStatus();
        _bulletStatus.SetStatus();
    }
    // リソースプレファブ取得
    void GetResource()
    {
        _HitEffect = (GameObject)Resources.Load("HitEffect");
    }

    // Start is called before the first frame update
    void Start()
    {
        GetCom();
        GetResource();
        bulletType = this.gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        if (bulletType == "BulletN")
        {
            _bulletStatus.SetPowAndSpeed(1.0f, 8.0f);  // 通常弾の威力と速度を設定
            transform.Translate(new Vector3(0, _bulletStatus._speed * Time.deltaTime, 0));
        }
        else if (bulletType == "BulletSP")
        {
            _bulletStatus.SetPowAndSpeed(10.0f, 10.0f); // 強化弾の威力と速度を設定
            transform.Translate(new Vector3(0, _bulletStatus._speed * Time.deltaTime, 0));
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);  // 画面外に出たらオブジェクトを削除
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "EnemyS":
            case "EnemyM":
            case "EnemyL":
            case "EnemyE":
            case "Boss":
                Instantiate(_HitEffect,this.gameObject.transform.position,this.gameObject.transform.rotation);
                _enemy = collision.gameObject.GetComponent<EnemyController>();
                Destroy(gameObject);
                // プレイヤーのパワーと弾の威力を合算してダメージ計算
                float damage = _player._status._pow + _bulletStatus._pow;
                _enemy.ApplyDamage(damage);  // 敵にダメージを与える
                break;
        }
    }
}
