using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    List<AudioClip> soundList;
    AudioClip sound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySound(string soundName)
    {
        switch (soundName)
        {
            case "EnemyDefeat":
                sound = soundList[3];
                audioSource.PlayOneShot(sound);
                break;
            case "PlayerDamage":
                sound = soundList[1];
                audioSource.PlayOneShot(sound);
                break;
            case "PlayerDefeat":
                sound = soundList[0];
                audioSource.PlayOneShot(sound);
                break;
            case "PlayerBullet":
                sound = soundList[2];
                audioSource.PlayOneShot(sound);
                break;
            case "CoinGet":
                sound = soundList[4];
                audioSource.PlayOneShot(sound);
                break;
        }
    }
}
