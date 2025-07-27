using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("Zombie Count")]
    public int zombieCount;
    public int minWaveCount;
    public int maxWaveCount;

    [Header("Zombie Stats")]
    public int health;
    public int minSpeed;
    public int maxSpeed;

    public bool isLast = false;
}
