using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class JumpReserveObstacle : MonoBehaviour
{
    public float failsafeMaxHold = 5f;

    NavMeshObstacle obstacle;
    int holds;

    void Awake()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        obstacle.carving = true;           // important: actually carves the mesh
        obstacle.enabled = false;          // off by default
    }

    public void Reserve(float seconds)
    {
        if (seconds <= 0f) seconds = 0.1f;
        seconds = Mathf.Min(seconds, failsafeMaxHold);

        holds++;
        if (!obstacle.enabled) obstacle.enabled = true;
        StartCoroutine(ReleaseAfter(seconds));
    }

    IEnumerator ReleaseAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        holds = Mathf.Max(0, holds - 1);
        if (holds == 0) obstacle.enabled = false;
    }
}
