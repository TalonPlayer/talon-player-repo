using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunnerMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float facingThreshold = .65f; // value must be between 0 and 1
    private Runner runner;
    private float radius;
    void Awake()
    {
        runner = GetComponent<Runner>();
    }

    void Start()
    {
        RunnerManager.moveTick += Move;
    }

    void Update()
    {

    }

    public void Move()
    {
        if (runner.target == null || !runner.isAlive)
        {
            ToggleMovement(false);
            return;
        }

        

        agent.destination = runner.target.transform.position;
        ToggleMovement(true);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            ToggleMovement(false);
        }
    }

    public void OnDeath()
    {
        RunnerManager.moveTick -= Move;
        ToggleMovement(false);
    }
    public void UnDeath()
    {
        RunnerManager.moveTick += Move;
        ToggleMovement(true);
    }


    public void ToggleMovement(bool on)
    {
        // If on is true, don't stop the movement
        agent.isStopped = !on;

        if (runner.isAlive)
        {
            runner.body.Play("Velocity", agent.speed);
            runner.body.PlayRandom("IsMoving", runner.body.idleAnims, on);
        }
    }
}
