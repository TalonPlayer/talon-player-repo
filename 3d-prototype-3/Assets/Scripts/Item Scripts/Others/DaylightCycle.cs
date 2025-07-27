using TMPro;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    public float startTime = 7f;
    public float endTime = 22f;
    public float currentTime = 7f;

    public float secondsPerHour = 6f;
    public bool dayEnded = false;

    private float totalRealTime;
    private float rotationDuration = 0f;  // Tracks elapsed real time
    
    [Header("Fog Settings")]
    public float fogStartHour = 20f;          // 8 PM
    public float fogEndHour = 22f;            // 10 PM
    public float maxFogDensity = 0.04f;       // Tweak to your liking
    private bool fogStarted = false;

    [Header("Clock Display")]
    public TextMeshProUGUI clockText;
    private float clockUpdateTimer = 0f;
    private float gameMinutesPerRealSecond;


    void Start()
    {
        totalRealTime = (endTime - startTime) * (6f * secondsPerHour);
        // 22f - 7f = 15f
        // 6f * 6f = 36f
        // 540 Seconds = 9 Minutes
        gameMinutesPerRealSecond = ((endTime - startTime) * 60f) / totalRealTime;
    }

    void Update()
    {
        if (dayEnded) return;

        rotationDuration += Time.deltaTime;

        // Update in-game time based on real time
        currentTime = startTime + (rotationDuration / totalRealTime) * (endTime - startTime);
        if (currentTime >= fogStartHour && currentTime <= fogEndHour)
        {
            if (!fogStarted)
            {
                RenderSettings.fog = true;
                fogStarted = true;
            }

            // Lerp fog density between 8 PM and 10 PM
            float t = Mathf.InverseLerp(fogStartHour, fogEndHour, currentTime);
            RenderSettings.fogDensity = Mathf.Lerp(0f, maxFogDensity, t);
        }

        clockUpdateTimer += Time.deltaTime * gameMinutesPerRealSecond;

        if (clockUpdateTimer >= 1f) // 1 in-game minute has passed
        {
            clockUpdateTimer = 0f;

            int hour = Mathf.FloorToInt(currentTime);
            int minute = Mathf.FloorToInt((currentTime - hour) * 60f);
            if (minute % 10 == 0)
            {
                string ampm = hour >= 12 ? "PM" : "AM";
                int displayHour = hour > 12 ? hour - 12 : (hour == 0 ? 12 : hour);

                clockText.text = $"{displayHour}:{minute:00} {ampm}";
            }
        }

        if (currentTime >= endTime)
        {
            currentTime = endTime;
            dayEnded = true;
            clockText.text = "10:00 PM";
            return;
        }

        // Rotate sun gradually over 180 degrees (e.g., from -90 to 90)
        float rotationSpeed = 180f / totalRealTime; // degrees per second
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.World);


    }
}
