using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum timeOfDay
{
    morning, evening, night
}

public class World : MonoBehaviour {
    // WORLD CONSTANTS
    public static string BUILDING_TAVERN = "TAVERN";
    public static string BUILDING_HOUSE = "RESIDENTIAL";
    public static string BUILDING_MARKET = "MARKET";
    public static string BUILDING_FARM = "FARM";
    public static string BUILDING_CAVE = "CAVE";
    public static string AREA_FARM = "Farm";
    public static string AREA_FOREST = "Forest";
    public static string AREA_CAVE = "Cave";
    public static string AREA_TOWN = "Town";
    public static string AREA_MARKET = "Market";

    // CHAOS CONSTANTS
    public static int NPC_PSYCHOTIC = 5;
    public static int NPC_DIES = 5; // chaos
    public static int NPC_UPSET = 1;
    public static int NPC_HAPPY = -1;
    public static int QUEST_COMPLETE = -5;
    public static int KILL_ANIMAL = 2;
    public static int STEAL = 1;
    public static int DESTROY = 1;
    public static int TAKE_ITEM = 0;

    // World Chaos
    private static int chaos;
    private static int numPsychopaths;
    public static int numKilled;
    public static int numUnhappy;

    // World Time
    private static float timeOfDayLength = 1.5f; // length of morning
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
        numKilled = 0;
        numUnhappy = 0;
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
        if (delta > 0)
            AddChaos(NPC_PSYCHOTIC);
    }

    public static int PsychopathInfectChance()
    {
        int chance = chaos / 20;
        if (chance <= 0)
            chance = 1;
        return chance;
    }

    public static int PsychopathKillChance()
    {
        return PsychopathInfectChance() * 2;
    }

    // Find places in the world
    public static Vector3 findRandomBuilding(string type)
    {
        List<Building> buildings = map.buildings[type];
        int random = Random.Range(0, buildings.Count);
        return buildings[random].getLocation();
    }

    public static Vector3 findRandomArea(string type)
    {
        List<TileManager> areas = new List<TileManager>();
        List<Vector2> areaLocs = new List<Vector2>();
        for (int x = 0; x < 10; x++) {
            for (int y = 0; y < 10; y++) {
                if (map.map[x][y].tileType == type) {
                    areas.Add(map.map[x][y]);
                    areaLocs.Add(new Vector2(x, y));
                }
            }
        }

        int random = Random.Range(0, areas.Count);
        TileManager tile = areas[random];
        Vector2 tileLoc = areaLocs[random];
        Vector3 area = tile.EmptyLocation();
        area.x += tileLoc.x * 10;
        area.y += tileLoc.y * 10;

        return area;
    }

    // Finds how many living people are on a tile
    public static int numPeopleOnTile(int mapX, int mapY)
    {
        TileManager tile = map.map[mapX][mapY];
        List<GameObject> npcs = tile.npcs;
        int numPeople = 0;

        foreach (GameObject npc in npcs) {
            if (!npc.GetComponent<NPC>().states.Contains(State.dead))
                numPeople++;
        }
        return numPeople;
    }
}