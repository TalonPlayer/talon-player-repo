using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public static PlantManager Instance;
    public List<Plant> plants;
    public delegate void CheckTarget();
    public static event CheckTarget checkTarget;
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
        if (checkTarget != null) checkTarget();

        if (damageTick != null)
        {
            damageTick();
            checkHealth();
            damageTick = null;
        }
    }
}
