using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public AudioSource audioSource;
    public List<AudioClip> deathSounds;
    public List<Enemy> enemyTypes;
    public List<GameObject> droppedItems;
    public Transform ragdollFolder;
    void Awake()
    {
        Instance = this;
    }

    public void AutoAggro(Enemy enemy)
    {
        enemy.onAggro?.Invoke();
    }
    public void CleanItems()
    {
        foreach (GameObject item in droppedItems)
        {
            Destroy(item);
        }
        for (int i = 0; i < ragdollFolder.childCount; i++)
        {
            Destroy(ragdollFolder.GetChild(i).gameObject);
        }
    }
    public void PlayDeathSound()
    {
        audioSource.pitch = Random.Range(.8f, 1.25f);
        audioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Count)]);
    }

    public void NearAggro(float radius, Vector3 pos)
    {
        foreach (Enemy e in WorldController.Instance.currentEnemies)
        {
            if (e.isAlive && !e.isAggro)
            {
                Vector3 direction = pos - e.transform.position;
                if (direction.magnitude <= radius)
                {
                    RaycastHit hit;
                    Ray ray = new Ray(transform.position, direction);
                    if (Physics.Raycast(ray, out hit, direction.magnitude))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            AutoAggro(e);
                        }
                    }
                }
            }
        }
    }
}