using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NepNepCityWorld : MonoBehaviour
{
    [Header("City Logic")]
    public GameObject currentCity; // Start with Purple
    public List<GameObject> cities; // Start with Green
    public List<float> sectionDuration; // Start with 25f;
    public int index = 0;
    public Animator animator;
    void Start()
    {
        StartCoroutine(SwappingRoutine(cities[index], sectionDuration[index]));
    }

    void Update()
    {

    }

    /// <summary>
    /// Sets the zombie spawn locations when going through a portal
    /// </summary>
    /// <param name="spawnFolder"></param>
    public void SetSpawnLocation(Transform spawnFolder)
    {
        WorldManager.Instance.spawnPoints.Clear();

        for (int i = 0; i < spawnFolder.childCount; i++)
        {
            WorldManager.Instance.spawnPoints.Add(spawnFolder.GetChild(i));
        }
        WorldManager.Instance.currentSpawn = spawnFolder.GetChild(0);
        WorldManager.Instance.worldCenter = spawnFolder;
    }

    /// <summary>
    /// Loops through all the cities given the times
    /// </summary>
    /// <param name="nextCity"></param>
    /// <param name="currentDuration"></param>
    /// <returns></returns>
    IEnumerator SwappingRoutine(GameObject nextCity, float currentDuration)
    {
        yield return new WaitForSeconds(currentDuration - .5f);
        animator.SetTrigger("Play");
        yield return new WaitForSeconds(.5f);
        currentCity.SetActive(false);
        nextCity.SetActive(true);
        currentCity = nextCity;
        index++;
        if (index >= cities.Count)
            index = 0;
        StartCoroutine(SwappingRoutine(cities[index], sectionDuration[index]));
    }

    
    /*
        Supernatural ruler sections
        Purple 25 seconds
        Green ~24 seconds
        Red ~22 seconds
        Yellow 22 seconds
        Blue 22 seconds
        (Orange) Purple 32 seconds
        (Pink) Green 22 seconds
        (White) Red 27 seconds
        (Rainbow) Yellow 35 seconds
        (Dark Blue) Blue 33 seconds
        Bonus 45 seconds
        Purple 24 seconds
        Green 10 seconds
        Red 22 seconds
        Yellow 22 seconds
        Blue 23 seconds
        Purple 23 seconds
    */
}
