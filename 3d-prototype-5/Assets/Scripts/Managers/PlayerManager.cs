using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public CinemachineVirtualCamera entityFacingCamera;
    public CameraView cameraView = CameraView.TopDown;
    public Animator hudAnimator;
    public Animator controlAnimator;
    public MyEntity selectedEntity;
    public CameraMovementType moveType;
    public EntityDragSelection dragSelection;
    public ControlMode controlMode;
    public Objective waypoint;
    public List<Objective> waypoints;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && cameraView == CameraView.EntityFacing)
        {
            SetCamera(CameraView.TopDown, selectedEntity);
        }


        if (Input.GetKeyDown(KeyCode.Q) && cameraView == CameraView.TopDown)
        {
            if (controlMode == ControlMode.InfoMode)
                ChangeControls(1);
            else if (controlMode == ControlMode.ActionMode)
                ChangeControls(0);

        }

        if (controlMode == ControlMode.ActionMode && cameraView == CameraView.TopDown)
        {
            if (Input.GetMouseButtonDown(1))
            {
                MoveSelection();
            }
        }

    }
    public void ChangeControls(int controlMode)
    {
        this.controlMode = (ControlMode)controlMode;
        switch (this.controlMode)
        {
            case ControlMode.InfoMode:
                controlAnimator.SetTrigger("Info");
                dragSelection.ClearSelection();
                break;
            case ControlMode.ActionMode:
                controlAnimator.SetTrigger("Action");
                break;
        }
    }
    public void SetCamera(CameraView viewType, MyEntity target = null)
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
    public void SelectEntity(MyEntity entity, bool additive)
    {
        if (!additive)
            dragSelection.ClearSelection();
        if (entity.outline.renderers == null || entity.outline.renderers.Count == 0)
            entity.outline.Init();
        entity.outline.SetOutlineActive(true);
        dragSelection.selectedEntities.Add(entity);
    }

    public void DelayDisableUIOverlay()
    {
        selectedEntity.outline.DisableUIOverlay();
    }

    public void MoveSelection()
    {
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        Vector3 pos = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        if (ground.Raycast(ray, out enter))
        {
            pos = ray.GetPoint(enter);
        }
        Objective waypointObj = Instantiate(waypoint, pos, Quaternion.identity, transform);
        foreach (MyEntity entity in dragSelection.selectedEntities)
        {
            List<Objective> temp = new List<Objective>();
            foreach (Objective waypoint in waypoints)
            {
                waypoint.entities.Remove(entity);
                if (waypoint.entities.Count == 0)
                    Destroy(waypoint.gameObject);
                else
                    temp.Add(waypoint);
            }

            waypoints.Clear();
            waypoints = temp;

            waypointObj.entities.Add(entity);
            waypoints.Add(waypointObj);
            entity.brain.objectiveTarget = waypointObj.transform;
        }

        dragSelection.ClearSelection();
    }
    
    public void CancelDrag()
    {
        dragSelection.CancelDrag();
    }
}
public enum CameraView
{
    TopDown,
    EntityFacing
}
public enum ControlMode
{
    InfoMode = 0,
    ActionMode = 1,
}
