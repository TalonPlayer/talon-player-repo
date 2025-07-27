using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBody : MonoBehaviour
{
    public Transform body;
    public float turnSpeed = 15f;
    public float viewAngle = 35f;
    public bool isFacing;
    private Robot robot;
    void Start()
    {
        robot = GetComponent<Robot>();
    }

    void Update()
    {
        if (robot.target != null) return;

        Vector3 lookDirection = robot.target.transform.position - body.position;
        lookDirection.y = 0f;
        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation;
            rotation = Quaternion.LookRotation(lookDirection);
            body.rotation = Quaternion.Slerp(body.rotation, rotation, Time.deltaTime * turnSpeed);
        }
        isFacing = Vector3.Angle(body.forward, lookDirection.normalized) <= viewAngle;
    }
}
