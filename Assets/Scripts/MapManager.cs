using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    // Constants
    private int columns = 10;       // # of columns in a tile
    private int rows = 10;          // # of rows in a tile
    private int townSize = 7;       // size of the town

    // Map information
    public string season;           // current season
    public Vector2 townCenter;      // center of the town on this map
    private int numCave;            // # of caves on this map
    private int numMarket;          // # of markets in the town
    private int numFarm;            // # of farms on this map
    public TileManager tileManager;
    public TileManager[][] map = new TileManager[10][];

    // Prefab objects
    public GameObject[] caveOutOuterWall, forestOuterWall, townOuterWall, marketOuterWall, farmOuterWall;
    public GameObject[] caveOutWalls, forestWalls, townWalls, marketWalls, farmWalls;
    public GameObject[] caveOutFloors, forestFloors, townFloors, marketFloors, farmFloors;
    public GameObject[] caveOutBuildings, forestBuildings, townBuildings, marketBuildings, farmBuildings;

    // Grid information
    private Grid grid;
    public Dictionary<string, List<Building>> buildings = new Dictionary<string, List<Building>>();
    public List<GameObject> npcs = new List<GameObject>();          // List of all NPCs on all tiles



    // Called every game loop
    void Update()
    {
        // Clear the previous npc locations on the tiles
        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                if (map != null && map[x] != null) {
                    map[x][y].ClearNpcs();
                }
            }
        }

        // Update all of the NPCs
        foreach (GameObject npc in npcs) {
            // Update the NPC
            NPC character = npc.GetComponent<NPC>();
            character.tile = map[character.mapX][character.mapY];
            character.Update();

            // Update tile locations
            map[character.mapX][character.mapY].npcs.Add(npc);

            // Check for interactions
            if (character.talking == true) {
                character.speak(character.mapX == World.player.mapX && character.mapY == World.player.mapY);
                character.talking = false;
            }
        }

        // Redraw the map tile
        if (map != null && World.player != null && map[World.player.mapX] != null)
            map[World.player.mapX][World.player.mapY].Draw();
    }

    // Sets up the map
    public void SetupScene()
    {
        string[] seasons = { "WINTER", "SPRING", "SUMMER", "FALL" };
        season = seasons[Random.Range(0, 4)];

        numCave = Random.Range(2, 4);       // 2 to 3 caves
        numMarket = Random.Range(7, 11);    // 7 to 10 markets
        numFarm = Random.Range(7, 10);      // 7 to 10 farms
        tileManager = GetComponent<TileManager>();

        grid = new Grid(columns, rows);
        initMap();
        MapSetup();
    }

    // Finds and stores all references to buildings and npcs on all tiles
    public void GetReferences()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Find all the buildings
                foreach (GameObject bldg in map[x][y].buildings)
                {
                    String type = "";
                    Vector3 loc = bldg.transform.position;
                    Building building = bldg.GetComponent<Building>();
                    building.map = this;

                    // Market place; buildings are stalls
                    if (map[x][y].tileType == "Market")
                        type = "MARKET";
                    // Town; buildings are homes
                    if (map[x][y].tileType == "Town")
                        type = "RESIDENTIAL";
                    // Caves are workplaces (miners)
                    if (map[x][y].tileType == "Cave")
                        type = "CAVE";
                    // Farms are workplaces
                    if (map[x][y].tileType == "Farm")
                        type = "FARM";

                    building.SetUp(map[x][y], type);
                    building.loc = new Vector3(loc.x + 10 * x, loc.y + 10 * y, loc.z);
                    if (!buildings.ContainsKey(type))
                        buildings[type] = new List<Building>();
                    buildings[type].Add(building);
                }
                // Find all the NPCs
                foreach (GameObject npc in map[x][y].npcs)
                {
                    Vector3 loc = npc.transform.position;
                    npcs.Add(npc);
                }
            }
        }

        SetupNPCs();
    }

    // Draws the tile at the given location to the screen
    public void Draw(int x, int y)
    {
        map[x][y].Draw();
    }

    // Removes the tile at the location from the screen
    public void Undraw(int x, int y)
    {
        map[x][y].Undraw();
    }



    // Instantiates all of the tiles on the map
    private void initMap()
    {
        for (int x = 0; x < columns; x++)
        {
            map[x] = new TileManager[11];
            for (int y = 0; y < rows; y++)
            {
                map[x][y] = (TileManager)Instantiate(tileManager, new Vector3(5, 5, 0), Quaternion.identity);
            }
        }
    }

    // Determines where the town will be located
    private List<Vector3> getTown()
    {
        List<Vector3> town = new List<Vector3>();
        int townX = Random.Range(3, 7);
        int townY = Random.Range(3, 7);
        townCenter = new Vector2(townX, townY);

        // Locations that will be the town
        for (int x = townX - 3; x <= townX + 3; x++)
        {
            for (int y = townY - 3; y <= townY + 3; y++)
            {
                town.Add(new Vector3(x, y, 0f));
            }
        }
        return town;
    }

    // Assigns random map locations for certain map tiles
    private List<Vector3> assignMapLocations(List<Vector3> mapLocs, int numTile)
    {
        // The list of where the map tiles are
        List<Vector3> tiles = new List<Vector3>();

        for (int i = 0; i < numTile; i++)
        {
            // Find a random location on the map
            int index = Random.Range(0, mapLocs.Count);
            Vector3 newLoc = mapLocs[index];
            mapLocs.RemoveAt(index);
            tiles.Add(newLoc);
        }
        return tiles;
    }

    // Sets up the map
    private void MapSetup()
    {
        // Figure out where the town will be located
        List<Vector3> town = getTown();
        // Figure out where the markets will be located in the town
        List<Vector3> markets = assignMapLocations(town, numMarket);
        // Figure out where the caves will be located
        List<Vector3> caves = assignMapLocations(grid.gridPositions, numCave);
        // Figure out where the farms will be located
        List<Vector3> farms = assignMapLocations(grid.gridPositions, numFarm);

        // Layout the map and setup each map tile
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                map[x][y].map = this;

                // Market
                if (grid.findTile(markets, x, y) != -1)
                {
                    map[x][y].SetupSprites(marketOuterWall, marketFloors, marketBuildings, marketWalls);
                    map[x][y].SetupScene(y, x, "Market");
                }
                // Town
                else if (grid.findTile(town, x, y) != -1)
                {
                    map[x][y].SetupSprites(townOuterWall, townFloors, townBuildings, townWalls);
                    map[x][y].SetupScene(y, x, "Town");
                }
                // Cave Exterior
                else if (grid.findTile(caves, x, y) != -1)
                {
                    map[x][y].SetupSprites(caveOutOuterWall, caveOutFloors, caveOutBuildings, caveOutWalls);
                    map[x][y].SetupScene(y, x, "Cave");
                }
                // Farm
                else if (grid.findTile(farms, x, y) != -1)
                {
                    map[x][y].SetupSprites(farmOuterWall, farmFloors, farmBuildings, farmWalls);
                    map[x][y].SetupScene(y, x, "Farm");
                }
                // Forest
                else
                {
                    map[x][y].SetupSprites(forestOuterWall, forestFloors, forestBuildings, forestWalls);
                    map[x][y].SetupScene(y, x, "Forest");
                }
            }
        }
    }

    // Sets up the NPCs
    private void SetupNPCs()
    {
        // Give each of the NPCs a home and a workplace
        for (int i = 0; i < npcs.Count; i++)
        {
            // Choose a random house and workplace
            NPC character = npcs[i].GetComponent<NPC>();
            character.job = Jobs.getRandomJob();
            string workType = character.job.work_tag;
            string homeType = character.job.home_tag;
            int homeLoc = Random.Range(0, buildings[homeType].Count);
            int workLoc = Random.Range(0, buildings[workType].Count);
            Building homeBldg = buildings[homeType][homeLoc];
            Building workBldg = buildings[workType][workLoc];

            // Workplace is already occupied, choose another one
            while (workBldg.tag.Contains("MARKET") && workBldg.owners.Count != 0)
            {
                workLoc = Random.Range(0, buildings[workType].Count);
                workBldg = workBldg = buildings[workType][workLoc];
            }

            // Initialize the NPC
            character.map = this;
            character.init(homeBldg, workBldg, i);
        }
    }

    public NPC getRandomNpcWithTag(string tag, bool work)
    {
        List<NPC> npcsWithTag = new List<NPC>();
        foreach (GameObject obj in npcs)
        {
            NPC npc = (NPC) obj.GetComponent(typeof(NPC));
            if (npc.job.work_tag.Contains(tag) && work || !work && npc.job.home_tag.Contains(tag))
            {
                npcsWithTag.Add(npc);
            }
        }
        return npcsWithTag[Random.Range(0, npcsWithTag.Count)];
    }

}