using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopHUDManager : MonoBehaviour
{
    public static ShopHUDManager Instance;
    [SerializeField] protected Color[] textColors;
    public GameObject shopHud;
    public GameObject johnFilesHud;
    private GameObject currentShopkeeperHud;
    public bool isOpened = false;
    void Awake()
    {
        Instance = this;
    }
    public void OpenJFShop()
    {
        shopHud.SetActive(true);

        currentShopkeeperHud = johnFilesHud;
        currentShopkeeperHud.SetActive(true);

        OnOpenShop();
    }
    private void OnOpenShop()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PlayerManager.Instance.player.input.SwapInput("Menu Traversal");
        isOpened = true;
    }
    public void ExitShop()
    {
        isOpened = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerManager.Instance.player.input.SwapInput("Movement");
        currentShopkeeperHud.SetActive(false);
        shopHud.SetActive(false);
    }
    
    /// <summary>
    /// Returns the color for the texts prompts
    /// <para>0 = White</para>
    /// <para>1 = Red</para>
    /// <para>2 = Yellow</para>
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public static Color GetTextColor(int idx)
    {
        return Instance.textColors[idx];
    }

}

public enum Shopkeeper
{
    None,
    JohnFiles
}