using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementHolder : MonoBehaviour
{
    public TextMeshProUGUI _name;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI progress;
    public TextMeshProUGUI unlocks;
    public Image bar;
    public Image sprite;
    public GameObject grayOut;

    public void Init(string name, string _desc, Sprite pic, bool completed = false, string unlockableName = null)
    {
        desc.text = _desc;
        _name.text = name;
        if (!string.IsNullOrEmpty(unlockableName)) unlocks.text = "Reward: " + unlockableName;
        if (completed)
        {
            sprite.sprite = pic;
            grayOut.SetActive(true);
        }
    }

    public void Setbar(int current = 1, int max = 1)
    {
        float perc = (float) current / max;
        bar.fillAmount = perc;
        if (perc < 1f) progress.text = current + " / " + max;
        else progress.text = "";
        
    }
}
