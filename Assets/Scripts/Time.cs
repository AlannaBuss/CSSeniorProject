using UnityEngine;
using System.Collections;

public enum timeOfDay
{
    morning, evening, night

    // morning = 0-5
    // evening = 5-10
    // night = 10-15
}

public class WorldTime : MonoBehaviour {

    private static float timeStart;         // Time the world was created
    private static timeOfDay dayTime;       // Current time of day

    // Length of each time of day in minutes
    private static float timeOfDayLength = 5;
    private static float lengthOfDay = timeOfDayLength * 3;


	// Use this for initialization
	public static void Start ()
    {
        timeStart = Time.time;
        dayTime = timeOfDay.morning;
	}
    
    // Gets the # of days passed since the world's creation
    public static int GetDaysPassed()
    {
        float totalMinutes = (Time.time - timeStart) / 60;
        float totalDays = totalMinutes / (timeOfDayLength * 3);

        return (int)totalDays;
    }
    
    // Gets the current time of the day (morning/evening/night)
    public static timeOfDay GetTimeOfDay()
    {
        float curTime = Time.time;
        float dayTime = (((curTime - timeStart) / 60) / timeOfDayLength) % lengthOfDay;

        if (dayTime < timeOfDayLength)
            return timeOfDay.morning;
        if (dayTime < timeOfDayLength * 2)
            return timeOfDay.evening;
        if (dayTime < timeOfDayLength * 3)
            return timeOfDay.night;

        return timeOfDay.morning;
    }
}
