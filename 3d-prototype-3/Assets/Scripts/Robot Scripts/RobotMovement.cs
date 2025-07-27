using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Robot robot;
    void Start()
    {
        robot = GetComponent<Robot>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (robot.target == null && !robot.isAlive) return;

        agent.SetDestination(robot.target.transform.position);
    }
}
