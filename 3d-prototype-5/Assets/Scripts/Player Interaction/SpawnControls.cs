using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnControls : MonoBehaviour
{
    public TextMeshProUGUI humanText;
    public TextMeshProUGUI zombieText;
    public List<Entity> humans = new List<Entity>();
    public List<Entity> zombies = new List<Entity>();
    public int maxHumans;
    public int maxZombies;

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
            Entity h = humans[i];
            if (h == null) humans.Remove(h);
        }
    }
    public void UpdateText()
    {
        humanText.text = "Humans: " + humans.Count;
        zombieText.text = "Zombie: " + zombies.Count;
    }

    public void SpawnHuman()
    {
        if (humans.Count == maxHumans) return;
        humans.Add(MyEntityManager.Instance.SpawnRandomHuman());
    }
    public void SpawnZombie()
    {
        if (zombies.Count == maxZombies) return;
        zombies.Add(MyEntityManager.Instance.SpawnRandomZombie());
    }

    public void SpawnMaxHuman()
    {
        int difference = maxHumans - humans.Count;
        for (int i = 0; i < difference; i++)
        {
            humans.Add(MyEntityManager.Instance.SpawnRandomHuman());
        }
    }

    public void SpawnMaxZombie()
    {
        int difference = maxZombies - zombies.Count;
        for (int i = 0; i < difference; i++)
        {
            zombies.Add(MyEntityManager.Instance.SpawnRandomZombie());
        }
    }

    public void KillAll()
    {
        foreach (Entity entity in MyEntityManager.Instance.entities)
            entity.OnRemove();
        foreach (Coroutine respawn in MyEntityManager.Instance.scheduledRespawn)
            StopCoroutine(respawn);

        zombies.Clear();
        humans.Clear();
    }

    public void SelectAll()
    {
        List<Entity> selection = PlayerManager.Instance.dragSelection.selectedEntities;
        foreach (Entity human in humans)
        {
            if (!selection.Contains(human))
            {
                selection.Add(human);
                human.outline.SetOutlineActive(true);
            }

        }
    }
}
