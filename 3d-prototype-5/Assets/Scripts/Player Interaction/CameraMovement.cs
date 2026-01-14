using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraSpeed = 8.5f;
    public float cameraShiftSpeed = 8.5f;
    public float dragSpeed = 8.5f;
    public float dragThreshold;
    public float maxHeight = 150f;
    public float baseHeight = 30f;
    public float step = 120f;
    public float lerpSpeed = 5f;
    public float minHeight = 10f;
    public Transform topDownCam;
    public CameraMovementType moveType;
    public TextMeshProUGUI controlsText;
    private bool centerMovement;
    private bool followObjective;
    private Vector3 dragPos;
    private bool isDragging = false;
    private Coroutine onScreenRoutine;
    private List<Transform> humans = new List<Transform>();
    void Update()
    {
        ZoomMovement();
        KeyMovement();
        DragMovement();

        if (centerMovement)
        {
            FixedMovement();
        }
        if (followObjective)
        {
            ObjectiveMovement(ObjectiveManager.Instance.objectives[0].transform);
        }
    }
    void StopCenterRoutine()
    {
        if (onScreenRoutine != null) StopCoroutine(onScreenRoutine);
    }
    public void BeginCenterMovement()
    {
        PlayerManager.Instance.CancelDrag();
        StopCenterRoutine();
        onScreenRoutine = StartCoroutine(OnScreenRoutine());
        centerMovement = true;
        followObjective = false;
    }
    public void BeginFollowMovement()
    {
        PlayerManager.Instance.CancelDrag();
        StopCenterRoutine();
        centerMovement = false;
        followObjective = true;
    }
    public void ZoomMovement()
    {
        if (topDownCam == null) return;

        float scroll = -Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.0001f) return;

        Vector3 camPos = topDownCam.position;


        float targetY = Mathf.Clamp(camPos.y + (scroll * step), minHeight, maxHeight);

        float newY = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * lerpSpeed);

        camPos.y = newY;
        topDownCam.position = camPos;
    }
    public void ResetCameraHeight()
    {
        Vector3 pos = topDownCam.position;
        pos.y = baseHeight;
        topDownCam.position = pos;
    }
    public void KeyMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(x, 0f, y);
        if (moveDir.magnitude > 0.1f)
        {
            StopCenterRoutine();
            centerMovement = false;
            followObjective = false;
        }

        moveDir.Normalize();
        if (Input.GetKey(KeyCode.LeftShift))
            moveDir *= cameraShiftSpeed * Time.deltaTime;
        else
            moveDir *= cameraSpeed * Time.deltaTime;

        transform.Translate(moveDir);
    }
    public void DragMovement()
    {
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (Input.GetMouseButtonDown(2))
        {
            StopCenterRoutine();
            centerMovement = false;
            followObjective = false;
            isDragging = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (ground.Raycast(ray, out enter))
            {
                dragPos = ray.GetPoint(enter);
            }
        }

        if (Input.GetMouseButton(2) && isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (ground.Raycast(ray, out enter))
            {
                Vector3 current = ray.GetPoint(enter);
                Vector3 difference = dragPos - current;
                difference.y = 0f;

                if (difference.magnitude < 0.5f) return;

                transform.position += difference * dragSpeed * Time.deltaTime;
                dragPos = current;
            }
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }
    }
    public void FixedMovement()
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        for (int i = 0; i < humans.Count; i++)
        {
            center += humans[i].position;
            count++;
        }
        center = (count > 0) ? center / count : transform.position;

        Vector3 pos = new Vector3(center.x, 0f, center.z);
        pos.y = transform.position.y;
        transform.position = pos;
    }
    private void FindHumansOnScreen()
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (Entity ent in MyEntityManager.Instance.entities)
        {
            if (!ent.brain.isHuman) continue;
            if (!ent.isAlive) continue;
            if (!ent.body) continue;
            if (humans.Contains(ent.transform)) continue;
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, ent.body.skin.bounds))
                humans.Add(ent.transform);
        }
    }

    public void GoToHumans()
    {
        List<Entity> humans = MyEntityManager.Instance.humans;
        Vector3 center = Vector3.zero;
        int count = 0;


        for (int i = 0; i < humans.Count; i++)
        {
            center += humans[i].transform.position;
            count++;
        }
        center = (count > 0) ? center / count : transform.position;

        Vector3 pos = new Vector3(center.x, 0f, center.z);
        pos.y = transform.position.y;
        transform.position = pos;
    }

    IEnumerator OnScreenRoutine()
    {
        humans.Clear();
        FindHumansOnScreen();
        yield return new WaitForEndOfFrame();
        onScreenRoutine = StartCoroutine(OnScreenRoutine());

    }

    public void ObjectiveMovement(Transform objective)
    {
        Vector3 pos = new Vector3(objective.position.x, 0f, objective.position.z);
        pos.y = transform.position.y;
        transform.position = pos;
    }
}

public enum CameraMovementType
{
    WASD = 0,
    Fixed = 1,
    ObjectiveFollow = 2
}