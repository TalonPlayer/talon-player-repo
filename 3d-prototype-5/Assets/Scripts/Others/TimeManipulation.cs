using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManipulation : MonoBehaviour
{
    public float targetTime;
    public void SetTargetTime(float target)
    {
        targetTime = target;
    }
    public void LerpTime(float time)
    {
        StartCoroutine(TimeRoutine(targetTime, time));
    }
    
    IEnumerator TimeRoutine(float target, float time)
    {
        float start = Time.timeScale;
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / time;

            Time.timeScale = Mathf.Lerp(start, target, t);
            yield return null;
            
        }
    }
}
