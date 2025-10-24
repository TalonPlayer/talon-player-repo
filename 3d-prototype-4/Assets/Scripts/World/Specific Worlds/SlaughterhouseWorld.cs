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
    private List<float> initialSpeed;
    private List<int> numOfNukes;
    private List<int> numOfDashes;
    private bool active = true;
    Coroutine spawnRoutine;
    void Start()
    {
        foreach (Player player in PlayerManager.Instance.players)
        {
            // Save some stats before the player enters
            // So if they fail, they get it back
            initialSpeed.Add(player.movement.maxSpeed);
            WorldManager.Instance.ChangePlayerSpeed(player, 9f);


            numOfNukes.Add(player.stats.nukes);
            numOfDashes.Add(player.stats.dashes);
            player.stats.nukes = 0;
            player.stats.dashes = 2;
            HudManager.Instance.UpdateText(player.playerIndex, 2, 0);
        }
    }
    void Update()
    {
        if (!PlayerManager.Instance.AllPlayersAlive() && active)
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
        foreach (Player player in PlayerManager.Instance.players)
        {
            player.stats.multiplier += 3;
            Vector3 pos = player.transform.position;
            pos.y += 20f;
            foreach (Drop drop in rewards)
            {
                DropObject obj = DropManager.Instance.SpawnDropObject(drop, pos);
                obj.MoveTo(player.transform);
                obj.moveSpeed = 2.5f;
                yield return new WaitForSeconds(.15f);
            }
        }

    }

    /// <summary>
    /// Complete Slaughterhouse, is called at end of animation
    /// </summary>
    public void Complete()
    {
        HudManager.Instance.AdvanceLevel("Slaughterhouse");
        EntityManager.Instance.KillAllEnemies();
        animator.enabled = false;
        PlayerManager.Instance.activeRedRoom = false;
    }

    /// <summary>
    /// Kills all enemies
    /// </summary>
    public void KillAllEnemies()
    {
        foreach (Enemy e in EntityManager.Instance.enemies)
            e.OnHit(500);
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
        WorldManager.Instance.onFinish.Invoke();

        for (int i = 0; i < PlayerManager.Instance.players.Count; i++)
        {
            Player player = PlayerManager.Instance.players[i];
            player.stats.dashes = numOfDashes[i];
            HudManager.Instance.UpdateText(player.playerIndex, 1, numOfDashes[i]);
            player.stats.nukes = numOfNukes[i];
            HudManager.Instance.UpdateText(player.playerIndex, 2, numOfNukes[i]);
            WorldManager.Instance.ChangePlayerSpeed(player, initialSpeed[i]);
        }

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
