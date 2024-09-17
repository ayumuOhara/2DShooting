using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    PlayerController _player;
    EnemyController _enemy;
    EnemyGenerator _enemyGene;
    public Image _energyBar;
    public Image _hpBar;
    TextMeshProUGUI _energyText;
    TextMeshProUGUI _coinText;
    TextMeshProUGUI _waveText;
    TextMeshProUGUI _wavePerText;
    [SerializeField]
    GameObject _clear;
    [SerializeField]
    GameObject _title;
    [SerializeField]
    GameObject _inGame; 
    [SerializeField] 
    GameObject InGameObjects;
    bool isInGame;

    // コンポーネント取得
    void GetCom()
    {
        GameObject player = GameObject.Find("Player");
        _player = player.GetComponent<PlayerController>();
        GameObject enemyGene = GameObject.Find("EnemyGenerator");
        _enemyGene = enemyGene.GetComponent<EnemyGenerator>();
        _energyText = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        _coinText = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
        _waveText = GameObject.Find("waveText").GetComponent<TextMeshProUGUI>();
        _wavePerText = GameObject.Find("wavePerText").GetComponent<TextMeshProUGUI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
        InGameObjects.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isInGame)
        {
            float gaugeHP = _player._status._hp / _player._status._maxHp;
            _hpBar.fillAmount = gaugeHP;

            float gaugeEN = _player._status._energy / _player._status._maxEnergy;
            _energyBar.fillAmount = gaugeEN;

            _energyText.text = $"{_player._status._energy} / {_player._status._maxEnergy}";
            _coinText.text = $"{_player._status._coin} / 10";
            _waveText.text = $"WAVE : {_enemyGene.wave}";
            _wavePerText.text = $"{_enemyGene.wavePercent} %";
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("Main");
    }

    public void Clear()
    {
        _clear.SetActive(true);
        _inGame.SetActive(false);
    }

    public void StartGame()
    {
        _inGame.SetActive(true);
        _title.SetActive(false);
        InGameObjects.SetActive(true);
        GetCom();
        isInGame = true;
    }
}
