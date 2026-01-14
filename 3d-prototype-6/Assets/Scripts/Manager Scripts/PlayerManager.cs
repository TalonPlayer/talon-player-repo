using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player player;
    public Vector3 PlayerPos{ get { return player.transform.position; }}
    void Awake()
    {
        Instance = this;
    }
}
