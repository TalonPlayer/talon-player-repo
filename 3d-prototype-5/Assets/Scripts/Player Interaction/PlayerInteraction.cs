using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityDragSelection : MonoBehaviour
{
    public KeyCode addToSelectionKey = KeyCode.LeftShift;

    public Rect selectionRect;
    public bool isDraggingSelection = false;

    private Vector2 dragStart;
    private Texture2D whiteTex;
    public List<MyEntity> selectedEntities;
    void Awake()
    {
        whiteTex = new Texture2D(1, 1);
        whiteTex.SetPixel(0, 0, Color.white);
        whiteTex.Apply();
    }
    public void CancelDrag()
    {
        isDraggingSelection = false;
        selectionRect = new Rect();
    }
    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (PlayerManager.Instance.controlMode == ControlMode.InfoMode) return;
        if (Input.GetMouseButtonDown(0))
        {
            isDraggingSelection = true;
            dragStart = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDraggingSelection)
        {
            Vector2 cur = Input.mousePosition;
            selectionRect = GetScreenRect(dragStart, cur);
        }

        if (Input.GetMouseButtonUp(0) && isDraggingSelection)
        {
            isDraggingSelection = false;
            SelectUnitsIn(selectionRect, Input.GetKey(addToSelectionKey));
            selectionRect = new Rect();
        }
    }

    void OnGUI()
    {
        if (!isDraggingSelection) return;

        DrawScreenRect(selectionRect, new Color(0.2f, 0.6f, 1f, 0.15f));
        DrawScreenRectBorder(selectionRect, 2f, new Color(0.2f, 0.6f, 1f, 0.9f));
    }

    Rect GetScreenRect(Vector2 p1, Vector2 p2)
    {
        p1.y = Screen.height - p1.y;
        p2.y = Screen.height - p2.y;

        Vector2 topLeft = Vector2.Min(p1, p2);
        Vector2 bottomRight = Vector2.Max(p1, p2);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTex);
        GUI.color = Color.white;
    }

    void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
    }

    void SelectUnitsIn(Rect screenRect, bool additive)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        if (!additive)
            ClearSelection();

        List<MyEntity> humans = MyEntityManager.Instance.humans;
        for (int i = 0; i < humans.Count; i++)
        {
            Outline outline = humans[i].outline;
            if (outline == null) continue;

            if (outline.renderers == null || outline.renderers.Count == 0)
                outline.Init();

            bool inside = IsAnyRendererInsideRect(cam, outline.renderers, screenRect);

            if (inside)
            {
                if (!selectedEntities.Contains(humans[i]))
                    selectedEntities.Add(humans[i]);
                outline.SetOutlineActive(true);
            }
        }
    }

    bool IsAnyRendererInsideRect(Camera cam, List<Renderer> rends, Rect rect)
    {
        for (int r = 0; r < rends.Count; r++)
        {
            Renderer rend = rends[r];
            if (rend == null) continue;

            Bounds b = rend.bounds;
            Vector3 c = b.center;
            Vector3 e = b.extents;

            Vector3[] worldCorners = new Vector3[8]
            {
                new Vector3(c.x - e.x, c.y - e.y, c.z - e.z),
                new Vector3(c.x + e.x, c.y - e.y, c.z - e.z),
                new Vector3(c.x - e.x, c.y - e.y, c.z + e.z),
                new Vector3(c.x + e.x, c.y - e.y, c.z + e.z),
                new Vector3(c.x - e.x, c.y + e.y, c.z - e.z),
                new Vector3(c.x + e.x, c.y + e.y, c.z - e.z),
                new Vector3(c.x - e.x, c.y + e.y, c.z + e.z),
                new Vector3(c.x + e.x, c.y + e.y, c.z + e.z),
            };

            for (int k = 0; k < worldCorners.Length; k++)
            {
                Vector3 sp = cam.WorldToScreenPoint(worldCorners[k]);
                if (sp.z < 0f) continue; // behind camera

                Vector2 guiP = new Vector2(sp.x, Screen.height - sp.y);
                if (rect.Contains(guiP))
                    return true;
            }
        }
        return false;
    }

    public void ClearSelection()
    {
        for (int i = 0; i < selectedEntities.Count; i++)
        {
            if (selectedEntities[i] != null)
                selectedEntities[i].outline.SetOutlineActive(false);
        }
        selectedEntities.Clear();
    }
}
