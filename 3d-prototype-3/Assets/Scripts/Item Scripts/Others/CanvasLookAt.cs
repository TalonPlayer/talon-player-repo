using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLookAt : MonoBehaviour
{
    public GameObject canvas;
    public Image fillBar;
    public TextMeshProUGUI timerText;
    void Start()
    {

    }

    void Update()
    {
        canvas.transform.LookAt(PlayerManager.Instance.player.transform);
    }

    public void UpdateBar(float current, float max)
    {
        float percentage = current / max;
        fillBar.fillAmount = percentage;
        percentage *= 100;
        timerText.text = (int) percentage + "%";
    }
}
