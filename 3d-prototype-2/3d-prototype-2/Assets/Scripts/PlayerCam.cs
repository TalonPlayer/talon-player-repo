using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;

    [SerializeField]
    private float camRotationSpeed = 10.0f;
    [SerializeField]
    private float rotationAmount = -5.5f;
    private float xRotation;
    private float yRotation;
    public bool IsSliding;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseLook();
    }

    private void MouseLook(){
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        if (IsSliding){
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.Euler(
                    xRotation, 
                    yRotation, 
                    rotationAmount),
                    camRotationSpeed * Time.deltaTime
                );
        }
        else{

            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.Euler(
                    xRotation, 
                    yRotation, 
                    0.0f),
                    camRotationSpeed * 3.5f * Time.deltaTime
                );
        }
        
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
