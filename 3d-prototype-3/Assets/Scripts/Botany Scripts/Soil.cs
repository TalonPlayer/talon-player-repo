using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Soil : MonoBehaviour
{
    public Seedling plantedSeedling;
    public Plant plant;
    public bool isWatered;
    public bool hasPlant;
    public GameObject soilModel;
    public GameObject wateredModel;
    void Start()
    {

    }

    void Update()
    {

    }

    public void PlantSeed(Seedling seedling)
    {
        seedling.Init();
        plantedSeedling = seedling;
        seedling.soil = this;
        hasPlant = true;

        if (isWatered)
        {
            Water();
        }
    }

    public void Water()
    {
        isWatered = true;
        soilModel.SetActive(false);
        wateredModel.SetActive(true);
        if (plantedSeedling) plantedSeedling.Water();
    }

    public void DryOut()
    {
        isWatered = false;
        soilModel.SetActive(true);
        wateredModel.SetActive(false);
        plant = null;
    }


}
