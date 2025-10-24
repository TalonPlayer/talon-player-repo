using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudObject : MonoBehaviour
{
    public TextMeshProUGUI lifeText, dashText, nukeText, multiplierText, scoreText;
    public Image backdrop, multiplierBar, ammoBar, timerBar;
    public Color backdropColor;

    public void SetColor()
    {
        backdrop.color = backdropColor;
    }

    public void IsDead()
    {
        backdrop.color = Color.black;
    }


}
