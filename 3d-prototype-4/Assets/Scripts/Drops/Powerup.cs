using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : Drop
{
    public AudioSource loopSound;

    public void LoopSound(GameObject obj)
    {
        AudioSource source = obj.gameObject.AddComponent<AudioSource>();

        // Copy settings from the loopSound template
        source.clip = loopSound.clip;
        source.loop = loopSound.loop;
        source.volume = loopSound.volume;
        source.pitch = loopSound.pitch;
        source.spatialBlend = loopSound.spatialBlend;
        source.playOnAwake = false;

        source.Play();

    }
}
