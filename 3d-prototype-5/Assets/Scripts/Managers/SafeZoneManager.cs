using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneManager : MonoBehaviour
{
    public static SafeZoneManager Instance;
    public List<Transform> safezones;
    public delegate void SafeZoneTick(List<Transform> safezones);
    public static event SafeZoneTick safeZoneTick;

    void Awake()
    {
        Instance = this;
        GetSafeZones();
    }

    // Update is called once per frame
    void Update()
    {
        safeZoneTick?.Invoke(safezones);
    }

    public void GetSafeZones()
    {
        safezones = Helper.GetChildren(transform);
    }
}
