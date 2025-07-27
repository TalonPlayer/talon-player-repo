using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance;
    public List<Robot> robots;
    public delegate void ClosestTarget();
    public static ClosestTarget closestTarget;
    public delegate void DamageTick();
    public static event DamageTick damageTick;
    public delegate void CheckHealth();
    public static event CheckHealth checkHealth;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    void Update()
    {
        if (closestTarget != null) closestTarget();

        
        if (damageTick != null)
        {
            damageTick();
            checkHealth();
            damageTick = null;
        }
    }
}
