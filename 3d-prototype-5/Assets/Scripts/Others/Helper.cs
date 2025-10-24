using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class Helper
{
    /// <summary>
    /// Returns a normalized Vector3 in a random direction
    /// </summary>
    /// <param name="minAngle"></param>
    /// <param name="maxAngle"></param>
    /// <returns></returns>
    public static Vector3 RandomDirection(float minAngle, float maxAngle)
    {
        // Convert angles to radians
        float minRad = minAngle * Mathf.Deg2Rad;
        float maxRad = maxAngle * Mathf.Deg2Rad;

        // Random horizontal rotation around the Y-axis
        float horizontalAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Random vertical angle between min and max
        float verticalAngle = Random.Range(minRad, maxRad);

        // Spherical coordinates to Cartesian conversion
        float x = Mathf.Cos(verticalAngle) * Mathf.Cos(horizontalAngle);
        float y = Mathf.Sin(verticalAngle); // vertical lift
        float z = Mathf.Cos(verticalAngle) * Mathf.Sin(horizontalAngle);

        return new Vector3(x, y, z).normalized;
    }

    /// <summary>
    /// Returns a Vector3 representing torque in the given direction
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="torqueStrength"></param>
    /// <returns></returns>
    public static Vector3 ApplyTorque(Vector3 direction, float torqueStrength)
    {
        // Create a perpendicular axis for torque based on the input direction
        Vector3 torqueAxis = Vector3.Cross(direction.normalized, Vector3.up);

        // If the direction is vertical, use a default axis (to avoid zero vector)
        if (torqueAxis == Vector3.zero)
            torqueAxis = Vector3.right;

        return torqueAxis.normalized * torqueStrength;
    }

    /// <summary>
    /// Returns a random Vector3 position within a transform
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Vector3 RandomPosition(Transform transform)
    {
        Vector3 center = transform.position;
        Vector3 extents = transform.localScale * 0.5f;

        float randX = Random.Range(-extents.x, extents.x);
        float randY = Random.Range(-extents.y, extents.y);
        float randZ = Random.Range(-extents.z, extents.z);

        return new Vector3(center.x + randX, center.y + randY, center.z + randZ);
    }

    public static Vector3 RandomVectorInRadius(float radius)
    {
        Vector3 offset = Random.insideUnitSphere * radius;
        offset.y = 0f;

        return offset;
    }


    /// <summary>
    /// Returns a shuffled list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="param"></param>
    /// <returns></returns>
    public static List<T> ShuffleList<T>(List<T> param)
    {
        System.Random random = new System.Random();
        return param.OrderBy(x => random.Next()).ToList();
    }

    /// <summary>
    /// Returns random element from list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RandomElement<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Returns a list of children Transforms, excludes the parent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static List<Transform> GetChildren(Transform t)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < t.childCount; i++)
        {
            list.Add(t.GetChild(i));
        }

        return list;
    }

    public static Color GetColor(string colorCode)
    {
        Color color;
        if (!ColorUtility.TryParseHtmlString(colorCode, out color))
        {
            Debug.LogWarning("Invalid Hex Code");
            return new Color();
        }
        return color;
    }
}
