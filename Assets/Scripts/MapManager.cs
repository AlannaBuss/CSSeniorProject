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
    private int numCave;            // # of caves on this map
    private int numMarket;          // # of markets in the town
    private int numFarm;            // # of farms on this map
    public TileManager tileManager;
    public TileManager[][] map = new TileManager[10][];

    // Outside references
    public Player player;

    // Prefab objects
    public GameObject[] caveOutOuterWall, forestOuterWall, townOuterWall, marketOuterWall, farmOuterWall;
    public GameObject[] caveOutWalls, forestWalls, townWalls, marketWalls, farmWalls;
    public GameObject[] caveOutFloors, forestFloors, townFloors, marketFloors, farmFloors;
    public GameObject[] caveOutBuildings, forestBuildings, townBuildings, marketBuildings, farmBuildings;

    // Grid information
    private Grid grid;
    public List<GameObject> workBldgs = new List<GameObject>();     // List of all work buildings on all tiles
    public List<Vector3> workLocs = new List<Vector3>();            // List of all work locations (xy: x = map, y = tile)
    public List<GameObject> homeBldgs = new List<GameObject>();     // List of all home buildings on all tiles
    public List<Vector3> homeLocs = new List<Vector3>();            // List of all home locations (xy: x = map, y = tile)
    public List<GameObject> npcs = new List<GameObject>();          // List of all NPCs on all tiles
    public List<Vector3> npcLocs = new List<Vector3>();             // List of all NPC locations



    // Called every game loop
    void Update()
    {
        // Clear the previous npc locations on the tiles
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                map[x][y].ClearNpcs();
            }
        }

        // Update all of the NPCs
        for (int i = 0; i < npcs.Count; i++)
        {
            // Update the NPC
            NPC character = npcs[i].GetComponent<NPC>();
            character.Update();

            // Update map locations
            Vector3 newLoc = new Vector3(character.tileX, character.tileY, 0);
            npcLocs[i] = newLoc;

            // Update tile locations
            map[character.mapX][character.mapY].npcs.Add(npcs[i]);
            map[character.mapX][character.mapY].npcLocations.Add(newLoc);
        }

        // Redraw the map tile
        map[player.mapX][player.mapY].Draw();
    }

    // Sets up the map
    public void SetupScene()
    {
        string[] seasons = { "winter", "spring", "summer", "fall" };
        season = seasons[Random.Range(0, 4)];

        numCave = Random.Range(1, 3);       // 1 to 2 caves
        numMarket = Random.Range(7, 11);    // 7 to 10 markets
        numFarm = Random.Range(7, 10);      // 7 to 10 farms
        tileManager = GetComponent<TileManager>();

        grid = new Grid(columns, rows);
        initMap();
        MapSetup();
        GetReferences();
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

            character.job = Jobs.getRandomJob(); //do something to get a better type of home and work loc need to talk to kayla

            int homeLoc = Random.Range(0, homeLocs.Count);
            int workLoc = Random.Range(0, workLocs.Count);
            Vector3 home = homeLocs[homeLoc];
            Vector3 work = workLocs[workLoc];
            Building homeBldg = homeBldgs[homeLoc].GetComponent<Building>();
            Building workBldg = workBldgs[workLoc].GetComponent<Building>();

            // Workplace is already occupied, choose another one
            while (workBldg.type == "market" && workBldg.owners.Count != 0)
            {
                workLoc = Random.Range(0, workLocs.Count);
                work = workLocs[workLoc];
                workBldg = workBldgs[workLoc].GetComponent<Building>();
            }

            // Initialize the NPC
            character.home = homeBldg;
            character.work = workBldg;
            character.map = this;
            character.init(home, work, i);
        }

    }

    // Finds and stores all references to buildings and npcs on all tiles
    private void GetReferences()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Find all the buildings
                for (int i = 0; i < map[x][y].buildings.Count; i++)
                {
                    Vector3 loc = map[x][y].buildingLocations[i];
                    Building building = map[x][y].buildings[i].GetComponent<Building>();
                    building.map = this;

                    // Market place; buildings are stalls
                    if (map[x][y].tileType == "Market")
                    {
                        building.SetUp(map[x][y], "market");
                        workLocs.Add(new Vector3(loc.x + 10 * x, loc.y + 10 * y, loc.z));
                        workBldgs.Add(map[x][y].buildings[i]);
                    }
                    // Town; buildings are homes
                    if (map[x][y].tileType == "Town")
                    {
                        building.SetUp(map[x][y], "home");
                        homeLocs.Add(new Vector3(loc.x + 10 * x, loc.y + 10 * y, loc.z));
                        homeBldgs.Add(map[x][y].buildings[i]);
                    }
                    // Caves are workplaces (miners)
                    if (map[x][y].tileType == "Cave")
                    {
                        building.SetUp(map[x][y], "cave");
                        workLocs.Add(new Vector3(loc.x + 10 * x, loc.y + 10 * y, loc.z));
                        workBldgs.Add(map[x][y].buildings[i]);
                    }
                    // Farms are workplaces
                    if (map[x][y].tileType == "Farm")
                    {
                        building.SetUp(map[x][y], "barn");
                        workLocs.Add(new Vector3(loc.x + 10 * x, loc.y + 10 * y, loc.z));
                        workBldgs.Add(map[x][y].buildings[i]);
                    }
                }
                // Find all the NPCs
                for (int i = 0; i < map[x][y].npcs.Count; i++)
                {
                    Vector3 loc = map[x][y].npcLocations[i];
                    npcLocs.Add(new Vector3(loc.x + 10 * x, loc.y + 10 * y, loc.z));
                    npcs.Add(map[x][y].npcs[i]);
                }
            }
        }
    }
}