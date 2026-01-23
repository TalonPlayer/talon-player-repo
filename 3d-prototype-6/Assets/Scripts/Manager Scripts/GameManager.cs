using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public static void Reset()
    {
        PlayerManager.Instance.ResetPlayer();
        EntityManager.Instance.CleanUp();
        WaveManager.Instance.Reset();
    }
}


