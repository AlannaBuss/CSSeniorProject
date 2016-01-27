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

    // References
    private MapManager mapManager;
    private Player playerManager;
    private Textbox textManager;

	//Quest information
	private QuestGenerator questGenerator;

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
        InitGame();
    }

    // Initializes the game
    void InitGame()
    {
        Items.Start();
        Jobs.Start();
        Dialogue.Start();

        // Draw the textbox
        textbox = Instantiate(textbox, new Vector3(4.5f, 0.5f, 0f), Quaternion.identity) as GameObject;
        textManager = textbox.GetComponent<Textbox>();
        textManager.Draw();

        // Create the map
        map = Instantiate(map, new Vector3(5.5f, 5.5f, 0), Quaternion.identity) as GameObject;
        mapManager = map.GetComponent<MapManager>();
        mapManager.textbox = textManager;
        mapManager.SetupScene();

		//Set up the Quest generator
		questGenerator = new QuestGenerator(mapManager);

        // Where are we on the map?
        int mapX = Random.Range(0, 10);
        int mapY = Random.Range(0, 10);
        Vector3 tile = new Vector3(4, 4, 0); // Place on the road, where there are no objects

        // Create the player
        player = Instantiate(player, new Vector3(tile.x, tile.y, 10f), Quaternion.identity) as GameObject;
        playerManager = player.GetComponent<Player>();
        playerManager.map = mapManager;
        playerManager.textbox = textManager;
        playerManager.PlaceAt(mapX, mapY, (int)tile.x, (int)tile.y, (int)(10 - tile.y));

        // Draw our area on the map
        mapManager.player = playerManager;
        mapManager.Draw(playerManager.mapX, playerManager.mapY);
    }
}