using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;
    public List<Objective> objectives;
    public HeldObjective flagPrefab;
    public List<FlagSpawn> flagSpawns;
    public Transform objectiveFolder;
    public int flagsCaptured;
    public LocationDetector detector;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    public void SetObjectiveList()
    {
        objectives.RemoveAll(o => !o.gameObject.activeInHierarchy);
        foreach (Entity zombie in MyEntityManager.Instance.zombies)
            if (!zombie.brain.objectiveTarget.gameObject.activeInHierarchy)
                zombie.brain.objectiveTarget = Helper.RandomElement(objectives).transform;
    }
    public void SpawnNewFlag()
    {
        HeldObjective flag = Instantiate(flagPrefab, objectiveFolder);

        FlagSpawn f = Helper.RandomElement(flagSpawns);

        flag.transform.position = f.flagSpawn.position;

        objectives.Add(flag);
    }
}

[System.Serializable]
public class FlagSpawn
{
    public Transform flagSpawn;
    public string flocationName;
}
