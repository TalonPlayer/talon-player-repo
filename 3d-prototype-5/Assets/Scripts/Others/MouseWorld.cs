using UnityEngine;

public static class MouseWorld
{
    /// <summary>
    /// Returns the position of the transform the cursor hovers over
    /// </summary>
    /// <param name="groundMask"></param>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPosition(LayerMask groundMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    public static MyEntity GetMouseSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, 1 << 6))
        {
            if (hit.collider.CompareTag("Human"))
            {
                MyEntity entity = hit.collider.GetComponent<MyEntity>();
                return entity;
            }
        }
        return null;
    }
}
