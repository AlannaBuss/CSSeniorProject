using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;


public class Building : Object
{
    // Building information
    public string tag;              // The type of this building
    public TileManager inside;      // The inside of the building
    public TileManager outside;     // Reference to the tile the building is on
    public List<NPC> owners;        // Reference to the NPCs who live/work here
    public Vector3 loc;             // x = map, y = tile

    // Prefab objects
    public GameObject[] outerWalls, walls, floors, objects;

    // Used for if the building is a market
    public string itemType;         // types of items that are generally sold here

    // Outside references
    public Player player;
    public MapManager map;



    // Sets up the building
    public void SetUp(TileManager o, string t)
    {
        outside = o;
        tag = t;
        owners = new List<NPC>();
    }

    public String getName()
    {
        String name = "";
        String type = "";

        foreach (NPC npc in owners)
            name += " " + npc.name;
        if (tag == "RESIDENTIAL")
            type = "'s home";
        else if (tag == "MARKET")
            type = "'s market";
        else if (tag == "FARM")
            type = "'s farm";
        else if (tag == "CAVE")
        {
            name = "";
            type = "cave";
        }

        return name + type;
    }

    // Draws the inside of the building when Player enters
    public void Enter()
    {
        textbox.Write("Entered " + getName());

        // Building can be entered
        if (tag != "MARKET")
        {
            player.insideBuilding = true;
            //outside.Undraw();
            //inside.Draw();
        }
        // Building is a market; display goods that can be bought
        else
        {
            player.insideMarket = true;
        }
    }

    // Draws the outside of the building when Player exits
    public void Exit()
    {
        textbox.Write("Exited " + getName());

        if (tag != "MARKET")
        {
            player.insideBuilding = false;
            //inside.Undraw();
            //outside.Draw();
        }
        else
        {
            player.insideMarket = false;
        }
    }

    public override Items.Item Interact()
    {
        Items.Item toReturn = null;

        if (player.insideBuilding || player.insideMarket)
            Exit();
        else
            Enter();

        return toReturn;
    }
}