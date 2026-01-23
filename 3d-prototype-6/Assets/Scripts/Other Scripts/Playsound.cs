using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playsound : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;

    public void PlaySound()
    {
        source.PlayOneShot(clip, Random.Range(.8f, 1.25f));
    }
}
