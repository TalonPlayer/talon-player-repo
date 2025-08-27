using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlaughterhouseWorld : MonoBehaviour
{
    public List<Transform> spawns;
    public List<Enemy> enemyPrefabs;
    public List<Drop> rewards; // Rewards for beating slaughterhouse
    public Drop nuke;
    public Animator animator;
    public BGMusic soundtrack;

    [Header("Zombie Stats")]
    public int health = 50;
    public int minSpeed = 12;
    public int maxSpeed = 14;
    private float initialSpeed;
    private int numOfNukes;
    private Player player;
    private bool active = true;
    Coroutine spawnRoutine;
    void Start()
    {
        player = PlayerManager.Instance.player;

        // Save some stats before the player enters
        // So if they fail, they get it back
        initialSpeed = player.movement.maxSpeed;
        numOfNukes = PlayerManager.Instance.nukes;
        PlayerManager.Instance.nukes = 0;
        HudManager.Instance.UpdateText(2, 0);
        for (int i = 0; i < numOfNukes; i++)
            rewards.Add(nuke);
    }
    void Update()
    {
        if (!player.isAlive && active)
        {
            active = false;
            Fail();
        }
    }

    /// <summary>
    /// Sets the lighting environment
    /// </summary>
    public void CutLights()
    {
        RenderSettings.ambientLight = Color.black;

        RenderSettings.fog = true;

        RenderSettings.fogColor = Color.black;

        RenderSettings.fogMode = FogMode.Linear;

        RenderSettings.fogStartDistance = 0f;
        RenderSettings.fogEndDistance = 14f;
    }

    /// <summary>
    /// Spawns all the enemies
    /// </summary>
    public void SpawnWave()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(BatchSpawn());
    }

    /// <summary>
    /// Spawns enemies one at time
    /// </summary>
    /// <returns></returns>
    IEnumerator BatchSpawn()
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            EntityManager.Instance.SpawnEnemy(
                spawns[i],
                enemyPrefabs[Random.Range(0, enemyPrefabs.Count)],
                health,
                minSpeed,
                maxSpeed,
                false);
            yield return new WaitForSeconds(0.05f);

        }
    }

    /// <summary>
    /// Used so that it can be called in the animation
    /// </summary>
    public void GiveReward()
    {
        StartCoroutine(Rewards());
    }

    /// <summary>
    /// Individually spawn rewards so they arent clumped
    /// </summary>
    /// <returns></returns>
    IEnumerator Rewards()
    {
        Transform player = PlayerManager.Instance.player.transform;
        PlayerManager.Instance.multiplier += 3;
        Vector3 pos = player.position;
        pos.y += 20f;
        foreach (Drop drop in rewards)
        {
            DropObject obj = DropManager.Instance.SpawnDropObject(drop, pos);
            obj.MoveTo(player);
            obj.moveSpeed = 2.5f;
            yield return new WaitForSeconds(.15f);
        }
    }

    /// <summary>
    /// Complete Slaughterhouse, is called at end of animation
    /// </summary>
    public void Complete()
    {
        HudManager.Instance.AdvanceLevel("Slaughterhouse");
        KillAllEnemies();
        animator.enabled = false;
        PlayerManager.Instance.activeRedRoom = false;
    }

    /// <summary>
    /// Kills all enemies
    /// </summary>
    public void KillAllEnemies()
    {
        EntityManager.Instance.KillAllEnemies();
    }

    /// <summary>
    /// If Player failed, stop scene and return changed stats
    /// </summary>
    public void Fail()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        animator.enabled = false;
        HudManager.Instance.AdvanceLevel("hide");
        StopSong();
        KillAllEnemies();
        PlayerManager.Instance.nukes = numOfNukes;
        HudManager.Instance.UpdateText(2, numOfNukes);
        player.movement.maxSpeed = initialSpeed;
        player.movement.moveSpeed = initialSpeed;
    }

    public void StopSong()
    {
        soundtrack.gameObject.SetActive(false);
    }

    /*
        After beating magatsu inaba, introduce slaughterhouse optional challenge.
        After every world, spawn portal and slaughterhouse door. 
        Keep spawning it until the player beats it.

        Playermanager - bool slaugterhouse availabe
        save world index so slaughterhouse can continue the normal path.

        when slaughterhouse finishes, use this index to send to next level.
    */
}
