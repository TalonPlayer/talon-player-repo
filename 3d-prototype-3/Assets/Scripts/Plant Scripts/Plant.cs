using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Plant : MonoBehaviour
{
    public bool isAggro;
    public bool isAlive;
    public int health;
    public Robot target;
    public UnityEvent onAggro;
    public UnityEvent onDeath;
    public UnityEvent onPacify;
    [HideInInspector] public PlantCombat combat;
    [HideInInspector] public PlantBody body;
    void Start()
    {
        combat = GetComponent<PlantCombat>();
        body = GetComponent<PlantBody>();
        PlantManager.checkHealth += CheckHealth;
    }

    void Update()
    {

    }
    public void TakeDamage(int damage)
    {
        PlantManager.damageTick += () => { health -= damage; };
    }

    public void CheckHealth()
    {
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        isAlive = false;
        BotanyManager.Instance.RemovePlant();
        onDeath?.Invoke();
        PlantManager.Instance.plants.Remove(this);
    }

    public void RobotDetected(GameObject robot)
    {
        target = robot.GetComponent<Robot>();
        isAggro = true;
        onAggro?.Invoke();

        // Turn off detection
    }

    public void Pacify()
    {
        target = null;
        isAggro = false;
        onPacify?.Invoke();
    }
}
