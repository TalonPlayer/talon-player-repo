using UnityEngine;
using UnityEngine.UI;

public class CursorFollower : MonoBehaviour
{
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false; // Optional: hide the default OS cursor
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        rectTransform.position = mousePosition;
    }
}
