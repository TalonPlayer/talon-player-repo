using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCostumeManager : MonoBehaviour
{
    public static PlayerCostumeManager Instance;
    public Transform playerHead;
    public GameObject currentCostume;
    public List<GameObject> costumes;
    public GameObject savedCostume;
    public int defaultIndex;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentCostume.SetActive(false);
        if (!string.IsNullOrEmpty(SaveSystem.selectedPlayerName))
        {
            PlayerData data = SaveSystem.LoadPlayer(SaveSystem.selectedPlayerName);
            Debug.Log("Loaded Character: " + data._name);
            defaultIndex = data.costumeIndex;
            Debug.Log("Costume Manager Costume Index: " + data.costumeIndex);
            PutOnCostume(costumes[defaultIndex]); // Saved Costume
        }
        else
        {
            PutOnCostume(costumes[0]); // Default Eyes
        }
    }

    public void PutOnCostume(GameObject costume)
    {
        currentCostume = Instantiate(costume, playerHead);
    }

    public void ChangeCostume(int costumeIndex, bool temporaryChange = false)
    {
        if (costumeIndex > costumes.Count) return;

        if (temporaryChange)
        {
            savedCostume = costumes[defaultIndex];
        }
        else
        {
            defaultIndex = costumeIndex;
        }

        Destroy(currentCostume);
        PutOnCostume(costumes[costumeIndex]);
    }

    public void ChangeCostume(int costumeIndex, Transform transform)
    {
        if (costumeIndex > costumes.Count) return;

        Destroy(currentCostume);
        currentCostume = Instantiate(costumes[costumeIndex], transform);
    }

    public GameObject ChangeCostume(int costumeIndex, Transform transform, GameObject currentCostume)
    {
        Destroy(currentCostume);
        return Instantiate(costumes[costumeIndex], transform);
    }

    public void ResetCostume()
    {
        Destroy(currentCostume);
        PutOnCostume(costumes[defaultIndex]);
    }
}
