using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText, weaponText, enemyStateText, enemyActionText;
    public TextMeshProUGUI interactText;
    public static HUDManager Instance;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void UpdateWeaponText(string weaponName)
    {
        Instance.weaponText.text = $"{weaponName}";
    }
    public static void UpdateAmmoText(int current, int max)
    {
        Instance.ammoText.text = $"{current} / {max}";
    }

    public static void InteractText(string message)
    {
        Instance.interactText.text = message;
    }

}
