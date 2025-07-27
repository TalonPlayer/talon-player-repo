using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{
    [Header("Environment Components")]
    public Material _skyBoxMaterial;
    public Light _sunSource;
    public bool hasFog;
    public UnityEvent onInit;


    [Header("Level Components")]
    public Level currentLevel;
    public List<Level> levels;
    public List<Cutscene> cutscene;
    public int levelIndex = 0;

    [Header("Spawnpoints")]
    public Transform currentSpawnpoint;

    public void Init()
    {
        onInit?.Invoke();
        currentLevel = levels[levelIndex];
        InitLevel(currentLevel);
    }

    public void InitLevel(Level level)
    {
        level.gameObject.SetActive(true);
        currentSpawnpoint = level.spawnpoint;
        WorldController.Instance.BuildNavMesh();
        level.Init();
        currentLevel = level;
    }

    public void GoToLevel(Level level)
    {
        currentLevel.Destroy();
        level.gameObject.SetActive(false);
        InitLevel(level);
    }
}
