using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    PlayerController _player;
    EnemyController _enemy;
    EnemyGenerator _enemyGene;
    public Image _hpBar;
    public Image _BhpBar;
    public Image _enemyhpBar;
    TextMeshProUGUI _coinText;
    TextMeshProUGUI _waveText;
    TextMeshProUGUI _nextWaveText;
    TextMeshProUGUI _bulletNameText;
    [SerializeField] List<GameObject> UI;
    bool isInGame;
    AudioManager _audio;
    [SerializeField] private Animator _waveTextAnim;
    [SerializeField] private Animator _nextWaveTextAnim;
    [SerializeField] private Animator _bulletNameTextAnim;
    [SerializeField] List<Animator> _BossHPAnim;

    // コンポーネント取得
    void GetCom()
    {
        GameObject player = GameObject.Find("Player");
        _player = player.GetComponent<PlayerController>();
        GameObject enemyGene = GameObject.Find("EnemyGenerator");
        _enemyGene = enemyGene.GetComponent<EnemyGenerator>();
        _audio = GameObject.Find("Speaker").GetComponent<AudioManager>();
        _coinText = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
        _waveText = GameObject.Find("waveText").GetComponent<TextMeshProUGUI>();
        _nextWaveText = GameObject.Find("nextWaveText").GetComponent<TextMeshProUGUI>();
        _bulletNameText = GameObject.Find("BulletNameText").GetComponent<TextMeshProUGUI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
        UI[3].SetActive(false);
    }

    public void GetEnemyHP()
    {
        GameObject boss = GameObject.Find("Boss(Clone)");
        _enemy = boss.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isInGame)
        {
            if (_player._status.IsDead() && Time.timeScale == 1.0f)
            {
                UI[4].SetActive(true);
            }

            float gaugeHP = _player._status._hp / _player._status._maxHp;
            _hpBar.fillAmount = gaugeHP;

            float gaugeBHP = _player._status._bonusHp / _player._status._bonusMaxHp;
            _BhpBar.fillAmount = gaugeBHP;

            if(_enemyGene.isBoss)
            {
                float gaugeEnemyHP = _enemy._status._hp / _enemy._status._maxHp;
                _enemyhpBar.fillAmount = gaugeEnemyHP;
            }

            _coinText.text = $"{_player._status._coin} / 10";
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("Main");
    }

    public void Clear()
    {
        _audio.PlayBGM("BOSSStop");
        _audio.PlayBGM("InGame");
        UI[0].SetActive(true);      // ゲームクリアUI
        UI[2].SetActive(false);     // インゲームUI
    }

    public void StartGame()
    {
        UI[2].SetActive(true);      // インゲームUI
        UI[1].SetActive(false);     // タイトル
        UI[3].SetActive(true);      // ゲームのオブジェクト
        GetCom();
        isInGame = true;
        _audio.PlayBGM("InGame");
    }

    public void BossHpOnDisplay()
    {
        _BossHPAnim[0].SetTrigger("BossCome");
        _BossHPAnim[1].SetTrigger("BossCome");
    }

    public void BossHpHideDisplay()
    {
        _BossHPAnim[0].SetTrigger("BossDead");
        _BossHPAnim[1].SetTrigger("BossDead");
    }

    public void FadeAnimation(float waveCnt)
    {
        if(waveCnt > 3)
        {
            _waveText.text = $"!! BOSS COMMING !!";
            _nextWaveText.text = $"";
        }
        else
        {
            _waveText.text = $"wave : {waveCnt}";
            if (waveCnt >= 3)
            {
                _nextWaveText.text = $"Next Wave : BOSS ENEMY";
            }
            else
            {
                _nextWaveText.text = $"Next Wave : {++waveCnt}";
            }
        }
        
        _waveTextAnim.SetTrigger("FadeAnim");
        _nextWaveTextAnim.SetTrigger("FadeAnim");
    }

    public void BulletTextAnimation(float lv)
    {
        switch(lv)
        {
            case 4:
                // ドレイン弾
                _bulletNameText.text = $"=CHANGE=\r\n<color=#FF4678>≪回復弾≫</color>";
                _bulletNameTextAnim.SetTrigger("Change");
                break;
            case 5:
                // ホーミング弾
                _bulletNameText.text = $"=CHANGE=\r\n<color=#FFFF82>≪追尾弾≫</color>";
                _bulletNameTextAnim.SetTrigger("Change");
                break;
            case 6:
                // 貫通弾
                _bulletNameText.text = $"=CHANGE=\r\n<color=#B4FFAA>≪貫通弾≫</color>";
                _bulletNameTextAnim.SetTrigger("Change");
                break;
            case 7:
                // 超高速弾
                _bulletNameText.text = $"=CHANGE=\r\n<color=#A9FFF0>≪超速弾≫</color>";
                _bulletNameTextAnim.SetTrigger("Change");
                break;
            default:
                break;
        }
    }
}
