using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldObjective : Objective
{
    public bool isHeld = false;
    public bool hiddenWhenCarried = false;
    public bool canHumans;
    public bool canZombies;
    public GameObject model;
    [SerializeField] private Entity carriedEntity;
    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (isHeld)
            transform.position = carriedEntity.transform.position;
    }

    public override void Init()
    {
        onInit?.Invoke();
    }

    /// <summary>
    /// When picked up
    /// </summary>
    public override void OnStart()
    {
        isHeld = true;
        model.SetActive(!hiddenWhenCarried);

        onStart?.Invoke();
    }

    /// <summary>
    /// When entity dies while carrying
    /// </summary>
    public override void OnInterrupt()
    {
        onInterrupt?.Invoke();
    }

    /// <summary>
    /// When entity brings object to goal
    /// </summary>
    public override void OnFinish()
    {
        carriedEntity.onDeath -= Drop;
        onFinish?.Invoke();
    }

    private void PickUp(Entity entity)
    {
        OnStart();

        carriedEntity = entity;
        carriedEntity.onDeath += Drop;
    }

    public void Drop()
    {
        isHeld = false;
        model.SetActive(true);
        carriedEntity = null;
    }

    public void CaptureFlag()
    {
        ObjectiveManager.Instance.flagsCaptured++;
        LocationDetector detector = ObjectiveManager.Instance.detector;
        detector.progressText.text = "Flags Captured: " + ObjectiveManager.Instance.flagsCaptured;
        ObjectiveManager.Instance.SpawnNewFlag();
        ObjectiveManager.Instance.SetObjectiveList();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Location")
        {
            // locationText.text = "Location: " + other.name;
        }
        if (isHeld || !isActive) return;
        if (other.tag == "Human" && canHumans)
        {
            Entity entity = other.GetComponent<Entity>();

            if (entity)
                PickUp(entity);
        }
        else if (other.tag == "Zombie" && canZombies)
        {
            Entity entity = other.GetComponent<Entity>();

            if (entity)
                PickUp(entity);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Location")
        {
            // locationText.text = "Nearby: " + other.name;
        }
    }
}

