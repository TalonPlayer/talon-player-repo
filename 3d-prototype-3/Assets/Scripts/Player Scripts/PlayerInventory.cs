using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public List<Cell> cells = new List<Cell>();
    public List<GameObject> handObjs = new List<GameObject>();
    public Color highlightedColor;
    public Color defaultColor;
    [HideInInspector] public InventoryItem currentInventoryItem;
    [HideInInspector] public Item currentItem; // Still useful for ease of access
    [HideInInspector] public GameObject heldObj;
    [HideInInspector] public GameObject currentIndicator;
    [HideInInspector] public List<Animator> animators = new List<Animator>();
    [HideInInspector] public List<GameObject> indicators = new List<GameObject>();
    public int itemIndex = 0;
    private Player player;
    private bool canUse = true;
    private Coroutine useRoutine;
    void Start()
    {
        player = GetComponent<Player>();
        SetUpInventory();
    }

    void Update()
    {
        if (currentItem != null)
        {
            if (currentItem.Validation() && canUse)
            {
                if (currentItem.hasIndicator)
                {
                    currentIndicator.SetActive(true);
                    currentIndicator.transform.position = currentItem.indicatorPos;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    useRoutine = StartCoroutine(UseRoutine(0));
                }

                if (Input.GetMouseButtonDown(1))
                {
                    useRoutine = StartCoroutine(UseRoutine(1));
                }
            }
            else
            {
                if (currentItem.hasIndicator)
                    currentIndicator.SetActive(false);
            }
        }

        if ((int)Input.mouseScrollDelta.y != 0 && canUse)
        {
            SetItem(itemIndex - (int)Input.mouseScrollDelta.y);
        }
    }

    public void CheckItemCount()
    {
        currentInventoryItem.Remove(1);
        cells[itemIndex].TextFormatter();

        if (currentInventoryItem.count == 0)
        {
            currentItem = null;
            currentInventoryItem = null;
            cells[itemIndex].heldItem.itemData = null;
            SetUpInventory();
        }
    }

    public void OnOpenInventory()
    {
        cells[itemIndex].borderImg.color = defaultColor;
    }

    public IEnumerator UseRoutine(int mouse)
    {
        canUse = false;
        if (animators[itemIndex])
            animators[itemIndex].Play("Use");
        yield return new WaitForSeconds(currentItem.delay);
        switch (mouse)
        {
            case 0:
                currentItem.PrimaryUse();
                break;
            case 1:
                currentItem.SecondaryUse();
                break;
        }

        yield return new WaitForSeconds(currentItem.cooldown);
        if (currentItem.isStackable) CheckItemCount();

        canUse = true;
    }

    public void ClearInventory()
    {
        foreach (GameObject obj in handObjs)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Destroy(obj.transform.GetChild(i).gameObject);
            }
        }

        foreach (GameObject ind in indicators)
        {
            Destroy(ind);
        }
        animators.Clear();
        indicators.Clear();
    }

    public void SetUpInventory()
    {
        ClearInventory();

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].heldItem.itemData != null)
            {
                GameObject obj = Instantiate(
                    cells[i].heldItem.itemData.itemModel,
                    handObjs[i].transform);
                cells[i].spaceImg.color = Color.white;
                Animator animator = obj.GetComponent<Animator>();
                animators.Add(animator);

                if (cells[i].heldItem.itemData.hasIndicator)
                {
                    GameObject ind = Instantiate(
                        cells[i].heldItem.itemData.itemIndicator);
                    ind.SetActive(false);
                    indicators.Add(ind);
                }
                else
                    indicators.Add(null);

                cells[i].TextFormatter();
            }
            else
            {
                cells[i].spaceImg.sprite = null;
                cells[i].spaceImg.color = new Color(0,0,0,0);
                animators.Add(null);
                indicators.Add(null);
            }

            handObjs[i].SetActive(false);
        }

        SetItem(itemIndex);
    }

    public void SetItem(int index)
    {
        if (useRoutine != null)
        {
            StopCoroutine(useRoutine);
            useRoutine = null;
            canUse = true;
        }

        cells[itemIndex].borderImg.color = defaultColor;

        if (currentItem != null)
        {
            handObjs[itemIndex].SetActive(false);
            if (currentItem.hasIndicator)
                indicators[itemIndex].SetActive(false);
            if (currentIndicator) currentIndicator.SetActive(false);
        }

        if (index <= -1)
            index = 7;
        else if (index >= 8)
            index = 0;

        currentInventoryItem = cells[index].heldItem;
        currentItem = currentInventoryItem != null ? currentInventoryItem.itemData : null;

        if (currentItem != null)
        {
            handObjs[index].SetActive(true);
            currentIndicator = indicators[index];
        }

        itemIndex = index;
        cells[itemIndex].borderImg.color = highlightedColor;
        HudManager.Instance.SetItemIntText(false, "");
    }

    public void RecieveItem(Item item, int amount)
    {
        bool check = false;
        foreach (Cell c in cells)
        {
            if (c.heldItem.itemData == item)
            {
                c.heldItem.Add(amount);
                check = true;
                break;
            }
        }

        if (!check)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].heldItem.itemData == null)
                {
                    cells[i].heldItem = new InventoryItem(item, amount);
                    cells[i].spaceImg.sprite = item.itemSprite;
                    break;
                } 
            }
        }

        SetUpInventory();
    }
}
