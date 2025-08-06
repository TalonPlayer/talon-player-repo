using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    public string songInfo;
    public double musicDuration;
    public double goalTime;

    [Header("Audio Looping")]
    public AudioClip currentClip;
    public List<AudioSource> audioSources;
    public int audioToggle = 0;

    [Header("Song Looping")]
    public AudioClip introClip;
    public AudioClip outroClip;
    public List<AudioClip> loopedClips;
    public int loopIndex = 0;
    public bool enable = false;
    void Start()
    {
        Invoke(nameof(StartDelay), 1.5f);

    }

    void StartDelay()
    {
        goalTime = AudioSettings.dspTime + 0.1f;
        SetCurrentClip(introClip);
        PlayScheduledClip();
        enable = true;
    }

    void Update()
    {
        if (enable && AudioSettings.dspTime > goalTime - 1) PlayScheduledClip();
    }

    private void PlayScheduledClip()
    {

        audioSources[audioToggle].clip = currentClip;
        audioSources[audioToggle].PlayScheduled(goalTime);

        musicDuration = (double)currentClip.samples / currentClip.frequency;
        goalTime = goalTime + musicDuration;

        // Swaps audioToggle from 0 and 1
        audioToggle = 1 - audioToggle;

        loopIndex = (loopIndex + 1) % loopedClips.Count;
        Debug.Log(loopIndex);
        SetCurrentClip(loopedClips[loopIndex]);
    }


    public void SetCurrentClip(AudioClip clip)
    {
        currentClip = clip;
    }

    public void PlayOutro()
    {
        foreach (AudioSource audio in audioSources)
            audio.Stop();

        audioSources[audioToggle].PlayOneShot(outroClip);

        enable = false;
    }
}
