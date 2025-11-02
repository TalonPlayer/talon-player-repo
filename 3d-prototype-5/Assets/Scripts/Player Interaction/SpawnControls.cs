using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class SpawnControls : MonoBehaviour
{
    public TextMeshProUGUI humanText;
    public TextMeshProUGUI zombieText;
    public List<MyEntity> humans = new List<MyEntity>();
    public List<MyEntity> zombies = new List<MyEntity>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnHuman();
        }   
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SpawnZombie();
        }       

        UpdateCount();
        UpdateText();
    }
    public void UpdateCount()
    {
        for (int i = humans.Count - 1; i >= 0; i--)
        {
            MyEntity h = humans[i];
            if (h == null) humans.Remove(h);
        }
        for (int i = zombies.Count - 1; i >= 0; i--)
        {
            MyEntity z = zombies[i];
            if (z == null) zombies.Remove(z);
        }
    }
    public void UpdateText()
    {
        humanText.text = "Humans: " + humans.Count;
        zombieText.text = "Zombie: " + zombies.Count;
    }

    public void SpawnHuman()
    {
        if (humans.Count == 25) return;
        humans.Add(MyEntityManager.Instance.SpawnRandomHuman());
    }
    public void SpawnZombie()
    {
        if (zombies.Count == 75) return;
        zombies.Add(MyEntityManager.Instance.SpawnRandomZombie());
    }

    public void SpawnMaxHuman()
    {
        int difference = 25 - humans.Count;
        for (int i = 0; i < difference; i++)
        {
            humans.Add(MyEntityManager.Instance.SpawnRandomHuman());
        }
    }

    public void SpawnMaxZombie()
    {
        int difference = 75 - zombies.Count;
        for (int i = 0; i < difference; i++)
        {
            zombies.Add(MyEntityManager.Instance.SpawnRandomZombie());
        }
    }
}
