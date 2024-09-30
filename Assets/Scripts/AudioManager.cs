using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    List<AudioClip> SeList;
    AudioClip sound;
    [SerializeField]
    List<AudioSource> BGMList;

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
                sound = SeList[0];
                audioSource.PlayOneShot(sound);
                break;
            case "PlayerDamage":
                sound = SeList[1];
                audioSource.PlayOneShot(sound);
                break;
            case "PlayerDefeat":
                sound = SeList[2];
                audioSource.PlayOneShot(sound);
                break;
            case "EnemyDamage":
                sound = SeList[3];
                audioSource.PlayOneShot(sound);
                break;
            case "CoinGet":
                sound = SeList[4];
                audioSource.PlayOneShot(sound);
                break;
            case "ItemGet":
                sound = SeList[5];
                audioSource.PlayOneShot(sound);
                break;
        }
    }

    public void PlayBGM(string BGMName)
    {
        switch (BGMName)
        {
            case "InGame":
                BGMList[0].Play();
                break;
            case "BOSS":
                BGMList[1].Play();
                break;
            case "InGameStop":
                BGMList[0].Stop();
                break;
            case "BOSSStop":
                BGMList[1].Stop();
                break;
        }
    }
}
