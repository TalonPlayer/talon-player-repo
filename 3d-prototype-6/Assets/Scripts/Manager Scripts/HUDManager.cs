using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText, weaponText;
    public TextMeshProUGUI interactText, reloadInfo;
    public TextMeshProUGUI enemyCountText;
    public TextMeshProUGUI pointText;
    public Image reloadBar, healthBar;
    public float bulletSpread;
    public float recoilMultiplier = 10f;
    public RectTransform cursor;
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

    public static void UpdateWaveInfo(string message)
    {
        Instance.enemyCountText.text = message;
    }

    public static void UpdateReloadBar(float f)
    {
        Instance.reloadBar.fillAmount = f;
    }

    public static void UpdateHealthBar(float f)
    {
        Instance.healthBar.fillAmount = f;
    }

    public static void ToggleReloadInfo(bool active)
    {
        Instance.reloadBar.transform.parent.gameObject.SetActive(active);
        Instance.reloadInfo.gameObject.SetActive(active);
    }

    public static void UpdatePoints(int points)
    {
        Instance.pointText.text = points.ToString();
    }

    public static void CursorRecoil(float recoil)
    {
        float t = Mathf.InverseLerp(0f, .1f, Instance.bulletSpread);
        // Base cursor size + how much spread increases it
        float spreadBoost = Mathf.Lerp(0f, 125f, t);

        // Recoil adds extra expansion on top (optional but usually desired)
        float size = 20f + spreadBoost + (recoil * -Instance.recoilMultiplier);

        Instance.cursor.sizeDelta = new Vector2(size, size);
    }

    public static void ToggleCursor(bool active)
    {
        Instance.cursor.gameObject.SetActive(active);
    }
}
