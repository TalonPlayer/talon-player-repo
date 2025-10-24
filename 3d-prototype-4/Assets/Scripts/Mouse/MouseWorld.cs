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

    /// <summary>
    /// Returns the position of the transform the controller aims towards
    /// </summary>
    /// <param name="groundMask"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Vector3 GetControllerWorldPosition(LayerMask groundMask, Transform player)
    {
        float distance = 50f;
        int steps = 19;
        float maxOffset = 2f;
        float horizontalOffset = 0.5f;

        for (int side = -1; side <= 1; side++)
        {
            Vector3 sideOffset = player.right * (side * horizontalOffset);

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                float verticalOffset = Mathf.Lerp(-maxOffset, maxOffset, t);

                Vector3 dir = (player.forward + Vector3.up * verticalOffset).normalized;

                Vector3 origin = player.position + sideOffset;

                Debug.DrawRay(origin, dir * distance, Color.red, 2f);

                if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, groundMask))
                {
                    Debug.DrawLine(origin, hit.point, Color.green, 2f);
                    return hit.point;
                }
            }
        }

        return Vector3.zero;
    }

}
