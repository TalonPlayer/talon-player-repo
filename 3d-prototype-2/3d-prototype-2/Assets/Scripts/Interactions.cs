using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactions : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer boxRenderer;

    [SerializeField]
    private InputActionReference actionReference;
    private InputAction test;

    private Color started = Color.yellow;
    private Color performed = Color.green;
    private Color canceled = Color.red;

    private void OnEnable()
    {
        actionReference.action.Enable();
    }

    private void OnDisable()
    {
        actionReference.action.Disable();
    }

    void Start()
    {
        actionReference.action.started += context => {
            boxRenderer.material.color = started;
        };
        actionReference.action.performed += context => {
            boxRenderer.material.color = performed;
        };
        actionReference.action.canceled += context => {
            boxRenderer.material.color = canceled;
        };
    }

    void Update()
    {
    }
}
