using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotanyManager : MonoBehaviour
{
    public static BotanyManager Instance;
    public List<Soil> soil;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    void Update()
    {

    }

    public void RemovePlant()
    {
        foreach (Soil s in soil)
        {
            if (s.plant)
            {
                if (!s.plant.isAlive)
                {
                    s.hasPlant = false;
                    s.DryOut();
                }
            }

        }
    }
}
