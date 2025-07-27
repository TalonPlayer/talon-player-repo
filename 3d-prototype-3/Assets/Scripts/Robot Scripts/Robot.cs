using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public GameObject target;
    public bool isAlive = true;
    public int health = 100;
    public UnityEvent onDeath;
    [HideInInspector] public RobotMovement movement;
    [HideInInspector] public RobotCombat combat;
    [HideInInspector] public RobotBody body;
    [HideInInspector] public RobotBrain brain;
    void Start()
    {
        movement = GetComponent<RobotMovement>();
        combat = GetComponent<RobotCombat>();
        body = GetComponent<RobotBody>();
        brain = GetComponent<RobotBrain>();
        RobotManager.closestTarget += TargetPlant;
        RobotManager.checkHealth += CheckHealth;
    }

    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        RobotManager.damageTick += () => { health -= damage; };
    }

    public void CheckHealth()
    {
        if (health <= 0)
        {
            isAlive = false;

            OnDeath();
        }
    }

    public void OnDeath()
    {
        onDeath?.Invoke();
    }

    public void TargetPlant()
    {
        GameObject closest = null;
        float closestDistance = float.MaxValue;
        float distance = 0f;
        foreach (Plant plant in PlantManager.Instance.plants)
        {
            distance = Vector3.Distance(transform.position, plant.transform.position);
            if (distance < closestDistance)
            {
                closest = plant.gameObject;
                closestDistance = distance;
            }
        }

        distance = Vector3.Distance(transform.position, PlayerManager.Instance.player.transform.position);

        if (distance < closestDistance)
        {
            closest = PlayerManager.Instance.player.gameObject;
        }

        target = closest.gameObject;
    }
}
