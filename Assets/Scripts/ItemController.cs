using System.Collections;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    PlayerController _player;
    AudioManager _audio;
    private string itemType;
    float _deletTime;
    [SerializeField] private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _deletTime = 10.0f;
        if(GameObject.Find("Player"))
        {
            GameObject player = GameObject.Find("Player");
            _player = player.GetComponent<PlayerController>();
        }
        _audio = GameObject.Find("Speaker").GetComponent<AudioManager>();
        itemType = this.gameObject.tag;
        StartCoroutine(DeleteItem());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, -0.6f * Time.deltaTime, 0));
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
                case "BonusItem":
                    _audio.PlaySound("ItemGet");
                    _player._status.GetBonusHP(1);
                    break;
                case "PowerItem":
                    _audio.PlaySound("ItemGet");
                    _player._status.LvUp(1);
                    break;
            }
            _player.StatusAdd();
            Destroy(gameObject);
        }
    }

    IEnumerator DeleteItem()
    {
        yield return new WaitForSeconds(_deletTime - 3);
        _animator.SetTrigger("Delete");
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
