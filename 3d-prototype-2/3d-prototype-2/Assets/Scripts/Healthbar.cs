using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Canvas canvas;
    public Image healthBar;
    void Update(){
        canvas.transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }

    public void UpdateHealth(float current, float max){
        healthBar.fillAmount = current/max;
    }
}
