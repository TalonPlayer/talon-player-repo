using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoot : MonoBehaviour
{
    void Awake()
    {
        GlobalSaveSystem.LoadOrCreate();
        Application.quitting += OnQuit;
    }

    void OnQuit()
    {
        GlobalSaveSystem.Save();
    }
}
