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
    public Entity selectedEntity;
    public CameraMovementType moveType;
    public EntityDragSelection dragSelection;
    public ControlMode controlMode;
    public Waypoint waypoint;
    public Waypoint objectiveWaypoint;
    public List<Waypoint> waypoints;
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
    public void SelectEntity(Entity entity, bool additive)
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
        if (dragSelection.selectedEntities.Count == 0) return;
        Objective o = null;
        Waypoint w = null;
        Vector3 pos = Vector3.zero;
        bool isObjective = false;
        bool isWaypoint = false;
        float size = 1f;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layers = (1 << 11) | (1 << 12) | (1 << 19) | (1 << 20);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, layers, QueryTriggerInteraction.Collide))
            if (hit.collider.tag == "Wall") return;
            else
            {
                isObjective = hit.collider.tag == "Objective";
                isWaypoint = hit.collider.tag == "Waypoint";
                if (isObjective)
                {
                    o = hit.collider.GetComponent<Objective>();
                    if (o) size = o.objectiveSize;
                    pos = o.transform.position;
                }
                else if (isWaypoint)
                {
                    w = hit.collider.GetComponent<Waypoint>();
                    if (w) pos = w.transform.position;
                }
                else
                {
                    pos = hit.point;
                    pos.y += .1f;
                }
            }

        Waypoint waypointObj = null;

        if (isObjective)
            waypointObj = Instantiate(objectiveWaypoint, pos, Quaternion.identity, o.transform);
        else if (!isWaypoint)
            waypointObj = Instantiate(waypoint, pos, Quaternion.identity, transform);

        foreach (Entity entity in dragSelection.selectedEntities)
        {

            if (isWaypoint)
            {
                w.entities.Add(entity);
                entity.brain.objectiveTarget = w.transform;
            }
            else
            {
                foreach (Waypoint p in waypoints)
                    p.RemoveAndEvaluate(entity);
                List<Waypoint> temp = waypoints.FindAll(w => w.entities.Count == 0 && w.canBeCleaned);
                foreach (Waypoint p in temp)
                    Destroy(p.gameObject);
                waypoints = waypoints.FindAll(w => w.entities.Count != 0 || !w.canBeCleaned);

                waypointObj.entities.Add(entity);
                waypoints.Add(waypointObj);
                entity.brain.objectiveTarget = waypointObj.transform;
            }

            entity.brain.orbitRadius = isObjective ? size : entity.brain.baseOrbitRadius;


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
