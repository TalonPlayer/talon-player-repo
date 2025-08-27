using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackroomsWorld : MonoBehaviour
{
    /// <summary>
    /// Adds fog for the backrooms
    /// </summary>
    public void AddFog()
    {
        RenderSettings.fog = true;

        RenderSettings.fogColor = Color.black;

        RenderSettings.fogMode = FogMode.Linear;

        RenderSettings.fogStartDistance = 0f;
        RenderSettings.fogEndDistance = 11.75f;
    }
}
