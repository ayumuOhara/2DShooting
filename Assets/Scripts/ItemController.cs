using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    PlayerController _player;
    AudioManager _audio;
    private string itemType;
    float _deletTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _deletTime = 10.0f;
        if(GameObject.Find("Player"))
        {
            GameObject player = GameObject.Find("Player");
            _player = player.GetComponent<PlayerController>();
        }
        _audio = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        itemType = this.gameObject.tag;
        StartCoroutine(DeleteItem());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            switch (itemType)
            {
                case "Coin":
                    _audio.PlaySound("CoinGet");
                    if (_player._status._coin < 10)  // 10 == coin‚ÌãŒÀ
                    {
                        _player._status._coin++;
                    }
                    break;
                case "EnergyItem":
                    if(_player._status._energy < _player._status._maxEnergy)
                    {
                        _player._status._energy++;
                    }
                    break;
                case "PowerItem":
                    _player._status.LvUp(1);
                    break;
            }
            Destroy(gameObject);
        }
    }

    IEnumerator DeleteItem()
    {
        yield return new WaitForSeconds(_deletTime);
        Destroy(gameObject);
    }
}
