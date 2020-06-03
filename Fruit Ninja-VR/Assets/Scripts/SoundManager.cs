using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public AudioClip[] audioList;
    AudioSource source;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
           // DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void BombSound()
    {
        source.PlayOneShot(audioList[1]);
     //   Debug.Log("BombSound");

    }
    public void FrozenSound()
    {
        source.PlayOneShot(audioList[2]);
      //  Debug.Log("FrozenSound");
    }
    public void SplashFruit()
    {
        source.PlayOneShot(audioList[0]);
//        Debug.Log("SplashFruit");

    }
}
