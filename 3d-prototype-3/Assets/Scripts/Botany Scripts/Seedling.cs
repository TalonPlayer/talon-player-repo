using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seedling : MonoBehaviour
{
    public GrowthStage stage;
    public Soil soil;
    public string _name;
    public string plantID;
    public CanvasLookAt bar;
    [Header("Growth Times")]
    public float seedlingTime;
    public float juvenileTime;
    public float adultTime;
    [HideInInspector] public float currentTime;
    [Header("Plant Models")]
    public GameObject seedModel;
    public GameObject seedlingModel;
    public GameObject juvenileModel;
    public GameObject adultPrefab;
    public Coroutine growthRoutine;
    private bool isGrowing = false;
    void Start()
    {
        bar.gameObject.SetActive(false);
    }

    void Update()
    {

    }

    public void Init()
    {
        stage = GrowthStage.Seed;
    }

    public void Water()
    {
        growthRoutine = StartCoroutine(GrowRoutine(seedlingTime));
        bar.gameObject.SetActive(true);
        isGrowing = true;
    }

    public IEnumerator GrowRoutine(float growthTime)
    {
        currentTime = 0f;

        while (true)
        {
            currentTime += Time.deltaTime;

            bar.UpdateBar(currentTime, growthTime);

            if (currentTime >= growthTime) break;
            yield return null;
        }

        switch (stage)
        {
            case GrowthStage.Seed:
                stage = GrowthStage.Seedling;
                seedModel.SetActive(false);
                seedlingModel.SetActive(true);
                growthRoutine = StartCoroutine(GrowRoutine(juvenileTime));
                break;
            case GrowthStage.Seedling:
                stage = GrowthStage.Juvenile;
                seedlingModel.SetActive(false);
                juvenileModel.SetActive(true);
                growthRoutine = StartCoroutine(GrowRoutine(adultTime));
                break;
            case GrowthStage.Juvenile:
                stage = GrowthStage.Adult;
                Vector3 pos = transform.position;
                pos.y += .5f;
                GameObject obj = Instantiate(adultPrefab, pos, Quaternion.identity, transform.parent);
                soil.plant = obj.GetComponent<Plant>();
                PlantManager.Instance.plants.Add(soil.plant);
                Destroy(gameObject);
                break;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (!isGrowing) return;
        if (other.gameObject.tag == "Player")
        {
            bar.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isGrowing) return;
        if (other.gameObject.tag == "Player")
        {
            bar.gameObject.SetActive(false);
        }
    }

    public enum GrowthStage
    {
        Seed,
        Seedling,
        Juvenile,
        Adult
    }
}
