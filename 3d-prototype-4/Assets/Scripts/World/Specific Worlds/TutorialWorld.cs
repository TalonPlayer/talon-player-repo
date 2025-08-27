using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWorld : MonoBehaviour
{
    // Puts Textboxes for towers
    public Transform towerFolder;
    public List<Transform> locations;
    public GameObject textboxPrefab;
    void Start()
    {
        int num = PlayerPrefs.GetInt("ToggleTutorial");

        if (num == 0)
            GetTowers();
        else
            Destroy(gameObject);
    }
    public void GetTowers()
    {
        for (int i = 0; i < towerFolder.childCount; i++)
            locations.Add(towerFolder.GetChild(i));
        foreach (Transform loc in locations)
        {
            Instantiate(textboxPrefab, loc.position, Quaternion.identity, loc);
        }
    }
}
