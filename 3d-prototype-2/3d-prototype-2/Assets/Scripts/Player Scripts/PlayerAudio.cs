using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip punchSound;
    public AudioClip kickSound;
    public AudioClip wooshSound;
    public List<AudioClip> deathSounds;
    public void PlayHitSound()
    {
        audioSource.pitch = Random.Range(.4f, 1.75f);
        audioSource.PlayOneShot(punchSound);
    }

    public void PlayKickSound()
    {
        audioSource.pitch = Random.Range(.6f, 1.25f);
        audioSource.PlayOneShot(kickSound);
    }

    public void PlayWooshSound()
    {
        audioSource.pitch = Random.Range(.6f, 1.25f);
        audioSource.PlayOneShot(wooshSound);
    }

    public void PlayDeathSound()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Count)]);
    }
}
