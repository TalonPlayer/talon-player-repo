using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerManager : MonoBehaviour
{
    public static RunnerManager Instance;
    public List<Runner> runners;
    public Transform runnerFolder;
    public Transform ragdollFolder;
    public Transform itemFolder;
    public delegate void HealthTick();
    public static event HealthTick healthTick;
    public delegate void DamageTick();
    public static event DamageTick damageTick;
    public delegate void MoveTick();
    public static event MoveTick moveTick;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        runners = FolderUnpacker.Unpack<Runner>(runnerFolder);
    }

    void Update()
    {
        if (damageTick != null)
        {
            damageTick();
            damageTick = null;

            if (healthTick != null)
                healthTick();
        }

        if (moveTick != null)
        {
            moveTick();
        }
    }

    public Vector3 SnapToGround(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit, 3.5f, 1 << 6)) // Layer 6 = Ground
        {
            Vector3 groundedPos = pos;
            groundedPos.y = hit.point.y;
            return groundedPos;
        }

        return pos;
    }

    public void CleanUp()
    {
        foreach (Transform child in ragdollFolder)
            Destroy(child.gameObject);
    }

    public void RunnerDeath(Runner runner)
    {
        healthTick -= runner.CheckHealth;
        runners.Remove(runner);
    }
}
