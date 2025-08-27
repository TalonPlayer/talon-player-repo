using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagatsuInabaWorld : MonoBehaviour
{
    /// <summary>
    /// Adds fog for Magatsu Inaba
    /// </summary>
    public void AddFog()
    {
        RenderSettings.fog = true;

        Color color;
        if (!ColorUtility.TryParseHtmlString("#3C2E00", out color))
            Debug.LogWarning("Invalid Hex Code");

        RenderSettings.fogColor = color;

        RenderSettings.fogMode = FogMode.ExponentialSquared;

        RenderSettings.fogDensity = .075f;
    }
}
