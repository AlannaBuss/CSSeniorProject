using UnityEngine;
using System.Collections;

public enum timeOfDay
{
    morning, evening, night

    // morning = 0-5
    // evening = 5-10
    // night = 10-15
}

public class World : MonoBehaviour {
    // 
    public static int QUEST_COMPLETE = -10;
    public static int NPC_HAPPY = -5;
    public static int WORLD_GROWS = +2;
    // 
    public static int NPC_DIES = +20;
    public static int NPC_PSYCHOTIC = +10;
    public static int NPC_UPSET = +5;
    public static int KILL_ANIMAL = +5;
    public static int STEAL = +2;
    public static int DESTROY = +2;
    public static int TAKE_ITEM = +1;

    private static int chaos;               // Current chaos in the world
    private static float timeStart;         // Time the world was created
    private static timeOfDay dayTime;       // Current time of day

    // Length of each time of day in minutes
    private static float timeOfDayLength = 5;
    private static float lengthOfDay = timeOfDayLength * 3;

    // References
    public static Player player;
    public static Textbox textbox;
    public static Inventory inventoryBox;


	// Use this for initialization
	public static void Start ()
    {
        chaos = 0;
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

    // Gets the chaos in the world
    public static int GetChaos()
    {
        return chaos;
    }

    // Adds or decreases world chaos
    public static void AddChaos(int delta)
    {
        chaos += delta;

        if (chaos <= 0)
            chaos = 0;
        else if (chaos >= 100)
            chaos = 100;
    }
}
