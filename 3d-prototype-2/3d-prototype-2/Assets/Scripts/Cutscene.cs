using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cutscene : MonoBehaviour
{
    public float cutsceneTime;
    public UnityEvent onStart;
    public UnityEvent onEnd;

    public void StartCutscene()
    {
        onStart?.Invoke();
        Invoke(nameof(EndCutscene), cutsceneTime);
    }
    
    public void EndCutscene()
    {
        onEnd?.Invoke();
    }
}
