using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopElement : MonoBehaviour
{
    public int cost;
    public UnityEvent onPay;
    protected Button button;
    protected TextMeshProUGUI costText;
    protected TextMeshProUGUI itemText;
    protected Player player;
    protected bool isAvailable = false;

    void Awake()
    {
        button = GetComponent<Button>();
        costText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        itemText = transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
    }
    protected virtual void Start()
    {
        player = PlayerManager.Instance.player;
        costText.text = "$" + cost;
        itemText.text = name;
        button.onClick.AddListener(OnPay);
    }
    protected virtual void Update()
    {
        if (player.points < cost) OnExpensive();
        else if (player.points >= cost) OnAvailable();
    }
    protected virtual void OnExpensive()
    {
        isAvailable = false;
        button.interactable = true;
        Color clr = ShopHUDManager.GetTextColor(1);
        costText.color = clr;
        itemText.color = clr;
        itemText.text = name;

        button.interactable = false;
    }

    protected virtual void OnAvailable()
    {
        isAvailable = true;
        Color clr = ShopHUDManager.GetTextColor(0);
        costText.color = clr;
        itemText.color = clr;
        itemText.text = name;

        button.interactable = true;
    }
    protected virtual void OnPay()
    {
        onPay?.Invoke();
        player.AddPoints(-cost);
    }
}

public interface IDuplicateItem
{
    public void DuplicateItem();

}

public interface ILimitedItem
{
    public void LimitedItem();

}
