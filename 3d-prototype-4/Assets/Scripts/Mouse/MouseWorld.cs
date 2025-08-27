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
        int steps = 19; // Amount of Raycasts
        float maxOffset = 2f;

        for (int i = 0; i <= steps; i++)
        {

            float t = (float)i / steps;
            float offset = Mathf.Lerp(-maxOffset, maxOffset, t);

            Vector3 dir = (player.forward + Vector3.up * offset).normalized;

            Debug.DrawRay(player.position, dir * distance, Color.red, 2f);

            if (Physics.Raycast(player.position, dir, out RaycastHit hit, distance, groundMask))
            {
                Debug.DrawLine(player.position, hit.point, Color.green, 2f);
                return hit.point;
            }
        }

        return Vector3.zero;
    }

}
