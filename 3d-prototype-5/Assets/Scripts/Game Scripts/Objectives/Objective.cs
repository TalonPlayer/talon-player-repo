using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Objective : MonoBehaviour
{
    public string objectiveID = "OBJ_";
    public float objectiveSize;
    public bool isActive;
    [SerializeField] protected UnityEvent onInit;
    [SerializeField] protected UnityEvent onStart;
    [SerializeField] protected UnityEvent onInterrupt;
    [SerializeField] protected UnityEvent onFinish;


    public virtual void Start()
    {
        Init();
    }
    public abstract void Init();
    public abstract void OnStart();
    public abstract void OnInterrupt();
    public abstract void OnFinish();

}
