using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISpyBGMusic : BGMusic
{
    [Header("I Spy With My Little Eye")]
    public List<AudioClip> introClips;
    public GameObject currentEffect; // Start with intro 1
    public List<GameObject> effects; // Start with intro 2
    public int effectIndex = 0;
    public int introIndex = 0;
    public bool isPlayingIntro;
    public Animator animator;
    void Start()
    {
        Invoke(nameof(StartDelay), 3f);

    }
    protected override void StartDelay()
    {
        goalTime = AudioSettings.dspTime + 0.1f;
        isPlayingIntro = true;
        SetCurrentClip(introClips[introIndex]);
        PlayScheduledIntroClip();
        enable = true;
    }
    public override void SetCurrentClip(AudioClip clip)
    {
        base.SetCurrentClip(clip);
        if (isPlayingIntro) return;
        animator.SetTrigger("Play");
        Invoke(nameof(DelayTransition), .5f);
    }

    void DelayTransition()
    {
        if (effectIndex >= effects.Count)
        {
            PlayerManager.Instance.EndGame();
            return;
        }
        currentEffect.SetActive(false);
        currentEffect = effects[effectIndex];
        currentEffect.SetActive(true);
        foreach (Player player in PlayerManager.Instance.players)
        {
            player.TeleportPlayer(WorldManager.Instance.worldCenter);
        }
        EntityManager.Instance.KillAllEnemies();
        
    }

    protected override void Update()
    {
        if (enable && AudioSettings.dspTime > goalTime - .25)
        {
            if (isPlayingIntro)
                PlayScheduledIntroClip();
            else
            {
                PlayScheduledClip();
                effectIndex++;
            }
        }
    }
    
    private void PlayScheduledIntroClip()
    {
        audioSources[audioToggle].clip = currentClip;
        audioSources[audioToggle].PlayScheduled(goalTime);

        musicDuration = (double)currentClip.samples / currentClip.frequency;
        goalTime = goalTime + musicDuration;

        // Swaps audioToggle from 0 and 1
        audioToggle = 1 - audioToggle;

        if (introIndex >= introClips.Count - 1)
        {
            SetCurrentClip(loopedClips[0]);
            isPlayingIntro = false;
            loopIndex = 0;
        }
        else
        {
            introIndex++;
            SetCurrentClip(introClips[introIndex]);
        }
    }
}
