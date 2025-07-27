using UnityEngine;

public static class MouseWorld
{
    public static Vector3 GetMouseWorldPosition(LayerMask groundMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
