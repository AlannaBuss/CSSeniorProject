using UnityEngine;
using System.Collections;

public enum timeOfDay
{
    morning, evening, night
}

public class World : MonoBehaviour {
    // CHAOS CONSTANTS
    public static int NPC_DIES = +5; // chaos
    public static int NPC_PSYCHOTIC = +5;
    public static int NPC_UPSET = +2;
    public static int NPC_HAPPY = -1;
    public static int QUEST_COMPLETE = -5;
    public static int KILL_ANIMAL = +2;
    public static int STEAL = +1;
    public static int DESTROY = +1;
    public static int TAKE_ITEM = 0;

    // World Chaos
    private static int chaos;
    private static int numPsychopaths;

    // World Time
    private static float timeOfDayLength = 3;    // length of morning
    private static float timeStart;              // Time the world was created
    private static timeOfDay dayTime;            // Current time of day
    private static float lengthOfDay = timeOfDayLength * 3;

    // World References
    public static Player player;
    public static MapManager map;
    public static Textbox textbox;
    public static Inventory inventoryBox;


	// Use this for initialization
	public static void Start ()
    {
        chaos = 0;
        numPsychopaths = 0;
        timeStart = Time.time;
        dayTime = timeOfDay.morning;
	}

    // Gets the # of days passed since the world's creation
    public static int GetDaysPassed()
    {
        float totalMinutes = (Time.time - timeStart) / 60;
        float totalDays = totalMinutes / lengthOfDay;

        return (int)totalDays;
    }
    
    // Gets the current time of the day (morning/evening/night)
    public static timeOfDay GetTimeOfDay()
    {
        float totalMinutes = (Time.time - timeStart) / 60;
        float dayTime = (totalMinutes / timeOfDayLength) % lengthOfDay;

        if (dayTime < timeOfDayLength)
            return timeOfDay.morning;
        if (dayTime < timeOfDayLength * 2)
            return timeOfDay.evening;
        if (dayTime < timeOfDayLength * 3)
            return timeOfDay.night;

        return timeOfDay.morning;
    }

    public static float GetTime()
    {
        return Time.time;
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

        if (chaos < 0)
            chaos = 0;
        else if (chaos > 100)
            chaos = 100;
    }

    // Gets the number of psychopaths in the world
    public static int GetNumPsychopaths()
    {
        return numPsychopaths;
    }

    // Adds or decreases number of psychopaths
    public static void AddPsychopath(int delta)
    {
        numPsychopaths += delta;

        if (numPsychopaths < 0)
            numPsychopaths = 0;
    }
}
