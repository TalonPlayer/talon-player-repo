using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource dropAudio;
    public AudioSource bgAudio;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }


    void Update()
    {

    }

    public void PlayBackgroundMusic(AudioClip audioClip)
    {
        bgAudio.PlayOneShot(audioClip);
    }
    public void PlayDropSound(AudioClip audioClip)
    {
        dropAudio.PlayOneShot(audioClip, Random.Range(.6f, 1.8f));
    }
}
