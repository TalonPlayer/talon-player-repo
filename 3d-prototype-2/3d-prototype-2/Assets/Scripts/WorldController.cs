using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class WorldController : MonoBehaviour
{

    public static WorldController Instance;
    public List<NavMeshSurface> navMeshes = new List<NavMeshSurface>();

    [Header("Worlds")]
    public World currentWorld;
    public int worldIndex;

    [Header("Enemy Handler")]
    public UnityEvent onReset;
    public List<Enemy> currentEnemies;
    private Player player;
    public Transform head;
    public bool cleanUp;
    public bool postCleanCheckScheduled = false;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameManager.Instance.player;
        if (worldIndex == 0)
        {
            player.movement.playerScale = Vector3.one;
            player.transform.localScale = Vector3.one;
            player.movement.crouchScale = new Vector3(1, 0.25f, 1);
        }
        else
        {
            player.movement.playerScale = Vector3.one * 1.5f;
            player.transform.localScale = Vector3.one * 1.5f;
            player.movement.crouchScale = new Vector3(1, 0.5f, 1);
        }
        currentWorld.Init();
    }

    void Update()
    {
        if (cleanUp)
        {
            if (Vector3.Distance(player.transform.position, currentWorld.currentSpawnpoint.position) <= 0.05f)
            {
                cleanUp = false;
                Respawn();

                if (!postCleanCheckScheduled)
                    StartPostCleanUpCheck();
            }
            else
            {
                player.SetPosition(currentWorld.currentSpawnpoint);
            }
        }
    }

    public void Respawn()
    {
        player.Respawn();

        player.SetPosition(currentWorld.currentSpawnpoint);
        player.animator.enabled = true;
        GameManager.Instance.EndCutscene();
        HUDController.Instance.PlayDeathAnim(false);

        if (Vector3.Distance(player.transform.position, currentWorld.currentSpawnpoint.position) > 0.05f)
        {
            cleanUp = true;
        }
        else
        {
            if (!postCleanCheckScheduled)
                StartPostCleanUpCheck();
        }
    }
    public void CleanUp()
    {
        if (currentWorld.currentLevel.hasEnemies)
        {
            EnemyManager.Instance.CleanItems();
            ClearEnemies();
            BuildNavMesh();
            currentEnemies = currentWorld.currentLevel.SpawnEnemies();
        }

    }
    private void StartPostCleanUpCheck()
    {
        postCleanCheckScheduled = true;
        Invoke(nameof(PostCleanUpCheck), .1f);
    }

    private void PostCleanUpCheck()
    {
        if (Vector3.Distance(player.transform.position, currentWorld.currentSpawnpoint.position) > 0.05f)
        {
            cleanUp = true;
        }
    }

    public void Reset()
    {
        player.isAlive = false;
        player.animator.enabled = false;
        player.DisableInput();
        postCleanCheckScheduled = false;
        Invoke(nameof(Respawn), 1f);
        Invoke(nameof(CleanUp), 1f);
        if (currentWorld.currentLevel.finishInit) currentWorld.currentLevel.onReset?.Invoke();
    }
    public void RebuildNavMesh(float time)
    {
        Invoke(nameof(BuildNavMesh), time);
    }

    public void BuildNavMesh()
    {
        foreach (NavMeshSurface navMesh in navMeshes)
        {
            navMesh.BuildNavMesh();
        }
    }
    public void CheckEnemyCount()
    {
        int count = 0;
        foreach (Enemy e in currentEnemies)
        {
            if (e.isAlive)
            {
                count++;
            }
        }

        if (count == 0)
        {
            currentWorld.currentLevel.Complete();
        }
    }


    public void ClearEnemies()
    {
        foreach (Enemy e in currentEnemies)
        {
            Destroy(e.gameObject);
        }

        currentEnemies.Clear();
    }

    public void AutoAggro(int group)
    {
        foreach (Enemy e in currentEnemies)
        {
            if (e.isAlive && e.aggroGroup == group)
            {
                EnemyManager.Instance.AutoAggro(e);
            }
        }
    }

    public void EndWorld()
    {
        EnemyManager.Instance.CleanItems();
        ClearEnemies();
        HUDController.Instance.TurnOn(false);
        HUDController.Instance.CloseScene();
        Invoke(nameof(Transition), 1f);
    }

    
    public void CloseScene()
    {
        HUDController.Instance.CloseScene();
    }

    void Transition()
    {
        GameManager.Instance.TransitionToNewWorld(worldIndex + 1);
    }

    public void SetPlayerSpeed(float speed)
    {
        player.movement.maxSpeed = speed;
    }
}
