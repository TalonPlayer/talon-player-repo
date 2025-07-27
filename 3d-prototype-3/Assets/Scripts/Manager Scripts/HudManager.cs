using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance;
    public TextMeshProUGUI itemIntText;
    public TextMeshProUGUI intText;
    public GameObject inventoryScreen;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SetInteractText(false, "");
        SetItemIntText(false, "");
    }

    void Update()
    {

    }

    public void SetInteractText(bool toggle, string text)
    {
        intText.gameObject.SetActive(toggle);
        intText.text = text;
    }

    public void SetItemIntText(bool toggle, string text)
    {
        itemIntText.gameObject.SetActive(toggle);
        itemIntText.text = text;
    }
}
