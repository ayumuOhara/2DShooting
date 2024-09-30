using System.Collections;
using System.Collections.Generic;
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

    // ステータスを設定するメソッド
    public void SetStatus(string name, float hp, float pow, float speed, float rate)
    {
        _name = name;
        _hp = hp;
        _maxHp = hp;
        _pow = pow;
        _speed = speed;
        _rate = rate;
    }

    // ダメージを受けるメソッド
    public void TakeDamage(float damage)
    {
        _hp = Mathf.Max(_hp - damage, 0); // HPを0未満にしない
    }

    // 敵が倒されたかどうかを確認するメソッド
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
    [SerializeField] private Animator _animator;

    [SerializeField] GameObject _defeatEff;
    [SerializeField] List<GameObject> EnemyBullet;
    [SerializeField] GameObject _powerItem;
    [SerializeField] GameObject _bonusItem;
    [SerializeField] GameObject _coin;
    string answerName;
    [SerializeField]
    bool _stopSpline = false;
    float atkSpan = 0;

    // コンポーネント取得
    void GetCom()
    {
        _splineAnimate = this.gameObject.GetComponent<SplineAnimate>();
        _status = new EnemyStatus();
        _enemyGene = GameObject.Find("EnemyGenerator").GetComponent<EnemyGenerator>();
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audio = GameObject.Find("Speaker").GetComponent<AudioManager>();
        _rb = GetComponent<Rigidbody2D>();
    }
    // ScriptableObjectからデータ取得
    void GetStatus()
    {
        answerName = this.gameObject.name;
        // タグに応じた敵データを検索してステータスを設定
        switch (answerName)
        {
            case "EnemyS(Clone)":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "EnemyS");
                break;
            case "EnemyM(Clone)":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "EnemyM");
                break;
            case "EnemyL(Clone)":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "EnemyL");
                break;
            case "EnemyE(Clone)":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "EnemyE");
                break;
            case "Boss(Clone)":
                enemy = enemyStatus._data.Find(enemy => enemy._name == "Boss");
                break;
        }
        _status.SetStatus(enemy._name, enemy._hp, enemy._pow, enemy._speed, enemy._rate);
    }

    // Start is called before the first frame update
    void Start()
    {
        GetCom();
        GetStatus();
        _stopSpline = false;
        StartCoroutine(EnemyAttack());
        if (enemy._name == "Boss")
        {
            _gameManager.BossHpOnDisplay();
            _gameManager.GetEnemyHP();
            StartCoroutine(BulletHSpawn());
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMove();
        // スプラインの進行度をチェック
        if (_splineAnimate != null && _splineAnimate.NormalizedTime >= 1.0f)
        {
            if(_player._status._hp > 0)
            {
                _stopSpline = true;
                Destroy(_splineAnimate);
            }
        }

        // ボスのHPの割合で攻撃頻度を変更
        if (_status._hp / _status._maxHp <= 0.2f)
        {
            atkSpan = 0.35f;
        }
        else if (_status._hp / _status._maxHp <= 0.5f)
        {
            atkSpan = 0.5f;
        }
        else
        {
            atkSpan = 1.0f;
        }

        // ボス出現時に雑魚を消去
        if (_enemyGene.isBoss)
        {
            if(enemy._name != "Boss")
            {
                Destroy(gameObject);
            }
        }
    }

    // ダメージ処理
    public void ApplyDamage(float damage)
    {
        _audio.PlaySound("EnemyDamage");
        _status.TakeDamage(damage);
        if (_status.IsDead())
        {
            if (enemy._name == "Boss")
            {
                StartCoroutine(BossDefeated());
            }
            else
            {
                _audio.PlaySound("EnemyDefeat");
                _enemyGene.WavePerAdd(1);
                Instantiate(_defeatEff, this.gameObject.transform.position, this.gameObject.transform.rotation);
                Destroy(this.gameObject);
                DropItems();
            }
        }
    }

    IEnumerator BossDefeated()
    {
        Time.timeScale = 0.1f;
        _audio.PlaySound("EnemyDefeat");
        _animator.SetTrigger("BossDefeated");

        int effectCount = 5; // エフェクトの数
        float effectRange = 2.0f; // エフェクトを出す範囲の半径
        float minDelay = 0.1f; // 各エフェクトの最小間隔
        float maxDelay = 0.5f; // 各エフェクトの最大間隔

        for (int i = 0; i < effectCount; i++)
        {
            // ランダムな位置を生成（ボスの中心からの相対的な位置）
            Vector3 randomPosition = this.gameObject.transform.position + new Vector3(
                Random.Range(-effectRange, effectRange), // X方向のランダムな範囲
                Random.Range(-effectRange, effectRange), // Y方向のランダムな範囲
                0); // Zは変更しない

            // エフェクトを生成
            Instantiate(_defeatEff, randomPosition, this.gameObject.transform.rotation);

            // 次のエフェクトまでランダムな時間待つ
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSecondsRealtime(waitTime);
        }

        Destroy(_rb);
        yield return new WaitForSecondsRealtime(3.0f);
        Time.timeScale = 1.0f;
        _gameManager.BossHpHideDisplay();
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
        _gameManager.Clear();
    }

    // エネミーの攻撃
    IEnumerator EnemyAttack()
    {
        while (true)
        {
            if (_status.IsDead())
            {
                yield break;
            }

            if (_stopSpline)
            {
                switch (enemy._name)
                {
                    case "EnemyE":
                    case "EnemyS":
                        // 単発
                        BulletSpawn(EnemyBullet[0], transform.position);
                        break;
                    case "EnemyM":
                        // 三点バースト
                        int i = 0;
                        do
                        {
                            BulletSpawn(EnemyBullet[2], transform.position);
                            i++;
                            yield return new WaitForSeconds(0.3f);
                        } while (i < 3);
                        break;
                    case "EnemyL":
                        // 二発同時撃ち
                        BulletMulti(EnemyBullet[1], 0.3f);
                        break;
                    case "Boss":
                        int k = 0;
                        do
                        {
                            BulletMulti(EnemyBullet[1], 1.0f);
                            BulletMulti(EnemyBullet[1], 0.7f);
                            k++;
                            yield return new WaitForSeconds(0.2f);
                        } while (k < 3);

                        yield return new WaitForSeconds(atkSpan);

                        int j = 0;
                        do
                        {
                            BulletMulti(EnemyBullet[2], 1.3f);
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

    void EnemyMove()
    {
        if (transform.position.x <= -1.5) _rb.linearVelocity = Vector2.right * _status._speed;
        if (transform.position.x >= 1.5) _rb.linearVelocity = Vector2.left * _status._speed;
    }

    // エネミーの弾をインスタンス化
    void BulletSpawn(GameObject bulletType,Vector3 spawnPoint)
    {
        GameObject bullet = Instantiate(bulletType, spawnPoint, Quaternion.Euler(180,0,0));
        EnemyBullets bulletScript = bullet.GetComponent<EnemyBullets>();
        bulletScript._enemy = this;
    }

    // エネミーの弾を2カ所にインスタンス化
    void BulletMulti(GameObject bulletType, float direction)
    {
        Vector3[] offsets = { new Vector3(direction, 0, 0), new Vector3(-direction, 0, 0) };

        foreach (Vector3 offset in offsets)
        {
            Vector3 newPosition = transform.position + offset;
            BulletSpawn(bulletType, newPosition);
        }
    }

    // ボス用攻撃パターン
    IEnumerator BulletHSpawn()
    {
        while (true)
        {
            if(_status.IsDead())
            {
                yield break;
            }

            if(_stopSpline)
            {
                BulletSpawn(EnemyBullet[3], transform.position);
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
            switch(enemy._name)
            {
                case "EnemyS":
                    if(rnd >= 50)   // 50%でコイン生成
                    {
                        ItemCreate(_coin);
                    }
                    else
                    {
                        ItemCreate(_bonusItem);
                    }
                    break;
                case "EnemyM":
                    if(rnd >= 50 && rnd <= 100)     // 50%でPOWアイテム生成
                    {
                        ItemCreate(_powerItem);
                    }
                    else if(rnd >= 30)              // 20%でコイン生成
                    {
                        ItemCreate(_coin);
                    }
                    break;
                case "EnemyL":
                    if (rnd >= 40 && rnd <= 100)    // 60%でPOWアイテム生成
                    {
                        ItemCreate(_powerItem);
                    }
                    else if (rnd >= 20)             // 20%でコイン生成
                    {
                        ItemCreate(_coin);
                    }
                    break;
                case "EnemyE":                      // 100%でボーナスHPアイテム生成
                    ItemCreate(_bonusItem);
                    break;
            }
        }
    }

    void ItemCreate(GameObject Item)
    {
        Instantiate(Item, transform.position, transform.rotation);
    }
}
