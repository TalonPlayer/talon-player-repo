using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool pause;
    public float idleTime = 0f;
    public float waypointDistance = .5f;
    public void RandomIdleAnimation(Animator animator, int idleCount)
    {
        animator.SetInteger("RandomAnimation", Random.Range(0, idleCount));

        animator.Play("RandomIdleAnimations");
    }
}
