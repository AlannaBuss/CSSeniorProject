using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    static System.IO.StreamWriter writer;

    // Prefab objects
    public GameObject player;
    public GameObject map;
    public GameObject textbox;
    public GameObject inventory;
    public GameObject questGen;

    // References
    private MapManager mapManager;
    private Player playerManager;
    private Textbox textManager;
    private Inventory inventoryManager;
    private QuestGenerator questGenManager;
    private TextMesh textMesh;


    void Update()
    {
        int days = WorldTime.GetDaysPassed();
        timeOfDay time = WorldTime.GetTimeOfDay();
        string season = mapManager.season;

        textMesh.text = "Day " + days + ", " + time + "\n        " + season;
    }

    public static void logger(String str)
    {
        if (writer == null)
        {
            if (!System.IO.File.Exists("log.txt"))
            {
                System.IO.File.Create("log.txt");
            }

            writer = new System.IO.StreamWriter(System.IO.File.OpenWrite("log.txt"));
        }
        print(str);
        writer.WriteLine(str);
        writer.Flush();
    }

    // Use this for initialization
    void Awake()
    {
        // Check if GameManager instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Don't destroy when reloading scene
        DontDestroyOnLoad(gameObject);

        InitStuff();
        InitGame();
    }

    void InitStuff()
    {
        Items.Start();
        Jobs.Start();
        Dialogue.Start();
        WorldTime.Start();

        // Create the textbox
        textbox = Instantiate(textbox, new Vector3(4.5f, 0.5f, 0f), Quaternion.identity) as GameObject;
        textManager = textbox.GetComponent<Textbox>();
        textManager.Draw();

        // Create the inventory box
        inventory = Instantiate(inventory, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        inventoryManager = inventory.GetComponent<Inventory>();
        inventoryManager.Setup();

        // Create the info box
        GameObject textContainer = new GameObject();
        textContainer = Instantiate(textContainer, new Vector3(6f, 9.5f, -1f), Quaternion.identity) as GameObject;
        textContainer.AddComponent<TextMesh>();
        Vector3 oldScale = textContainer.transform.localScale;
        Vector3 newScale = new Vector3(oldScale.x * .25f, oldScale.y * .25f, oldScale.z * .25f);
        textContainer.transform.localScale = newScale;
        textMesh = textContainer.GetComponent<TextMesh>();
        textMesh.alignment = TextAlignment.Left;
        textMesh.color = Color.white;
        textMesh.fontSize = 20;
    }

    // Initializes the game
    void InitGame()
    {
        // Create the map
        map = Instantiate(map, new Vector3(5.5f, 5.5f, 0), Quaternion.identity) as GameObject;
        mapManager = map.GetComponent<MapManager>();
        mapManager.textbox = textManager;
        mapManager.inventory = inventoryManager;
        mapManager.SetupScene();

		//Set up the Quest generator
        questGen = Instantiate(questGen) as GameObject;
        questGenManager = questGen.GetComponent<QuestGenerator>();
        questGenManager.setMap(mapManager);

        // Where are we on the map?
        Vector2 townCenter = mapManager.townCenter;
        Vector3 tile = new Vector3(4, 4, 0); // Place on the road, where there are no objects

        // Create the player
        player = Instantiate(player, new Vector3(tile.x, tile.y, 10f), Quaternion.identity) as GameObject;
        playerManager = player.GetComponent<Player>();
        playerManager.map = mapManager;
        playerManager.textbox = textManager;
        playerManager.inventoryBox = inventoryManager;
        playerManager.PlaceAt((int)townCenter.x, (int)townCenter.y, (int)tile.x, (int)tile.y, (int)(10 - tile.y));

        // Draw our area on the map
        mapManager.player = playerManager;
        mapManager.GetReferences();
        mapManager.Draw(playerManager.mapX, playerManager.mapY);
    }
}