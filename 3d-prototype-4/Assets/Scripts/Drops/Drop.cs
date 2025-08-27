using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : ScriptableObject
{
    [Header("Stats")]
    public string _name;
    public int rarity; // 1 (common) - 100 (rare)

    [Header("Drop Visuals")]
    public ParticleSystem sparklePrefab;
    public GameObject model;
    public AudioClip pickUpSound;
    public AudioClip spawnSound;

    /// <summary>
    /// When the player picks up object, play the pick up sound
    /// </summary>
    public virtual void OnPickUp()
    {
        AudioManager.Instance.PlayDropSound(pickUpSound);
    }
}
