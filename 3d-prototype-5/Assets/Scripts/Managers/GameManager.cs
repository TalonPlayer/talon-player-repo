using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CinemachineVirtualCamera entityFacingCamera;
    public CameraView cameraView = CameraView.TopDown;
    public Animator hudAnimator;
    public Entity selectedEntity;
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCamera(CameraView.TopDown, selectedEntity);
        }
    }

    public void SetCamera(CameraView viewType, Entity target = null)
    {
        cameraView = viewType;
        selectedEntity = target;
        switch (cameraView)
        {
            case CameraView.TopDown:
                if (selectedEntity != null)
                {
                    Invoke(nameof(DelayDisableUIOverlay), .4f);
                }
                hudAnimator.SetTrigger("Top View");
                break;
            case CameraView.EntityFacing:
                if (selectedEntity != null)
                {
                    EntityHUDManager.Instance.SetHud(selectedEntity);
                    selectedEntity.outline.EnableUIOverlay();
                }
                entityFacingCamera.Follow = target.transform;
                entityFacingCamera.LookAt = target.transform;
                hudAnimator.SetTrigger("Facing View");
                break;
        }
    }

    public void DelayDisableUIOverlay()
    {
        selectedEntity.outline.DisableUIOverlay();
    }
}

public enum CameraView
{
    TopDown,
    EntityFacing
}
