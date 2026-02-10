using System;
using UnityEngine;

public class ClockSync : MonoBehaviour
{
    [SerializeField] private Transform hourHand;
    [SerializeField] private Transform minuteHand;
    
    [SerializeField] private int fallbackHour = 9;
    [SerializeField] private int fallbackMinute = 41;

    private float timer;

    void FixedUpdate()
    {
        if (timer >= 1f)
        {
            
        }
        
        int hour, minute;

        try
        {
            DateTime now = DateTime.Now;
            hour = now.Hour;
            minute = now.Minute;
        }
        catch
        {
            hour = fallbackHour;
            minute = fallbackMinute;
        }

        float minuteAngle = 90f - (minute / 60f) * 360f;
        float hourAngle = 90f - ((hour % 12 + minute / 60f) / 12f) * 360f;

        Vector3 hourEuler = hourHand.localEulerAngles;
        hourEuler.x = hourAngle;
        hourEuler.y = -90;
        hourEuler.z = -90;
        hourHand.localEulerAngles = hourEuler;

        Vector3 minuteEuler = minuteHand.localEulerAngles;
        minuteEuler.x = minuteAngle;
        minuteEuler.y = -90;
        minuteEuler.z = -90;
        minuteHand.localEulerAngles = minuteEuler;
    }
}