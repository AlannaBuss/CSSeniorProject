using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;


public class Building : Object
{
    // Building information
    public string buildingType;                 // The type of this building
    public List<NPC> owners = new List<NPC>();  // Reference to the NPCs who live/work here


    // Gets the name of the building
    public String getName()
    {
        String name = "";

        foreach (NPC npc in owners)
            name += " " + npc.name;
        if (buildingType == "RESIDENTIAL")
            name += "'s home";
        else if (buildingType == "MARKET")
            name += "'s market";
        else if (buildingType == "FARM")
            name += "'s farm";
        else if (buildingType == "CAVE")
            name += "cave";

        return name;
    }

    // 
    public Vector3 getLocation()
    {
        return new Vector3(tileX + mapX * 10, tileY + mapY * 10, 0);
    }

    // Draws the inside of the building when Player enters
    public void Enter()
    {
        World.textbox.Write("Entered " + getName());

        // Building can be entered
        if (buildingType == "MARKET")
            World.player.insideMarket = true;
        // Building is a market; display goods that can be bought
        else {
            World.player.insideBuilding = true;
            World.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // Draws the outside of the building when Player exits
    public void Exit()
    {
        World.textbox.Write("Exited " + getName());

        if (buildingType == "MARKET")
            World.player.insideMarket = false;
        else {
            World.player.insideBuilding = false;
            World.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    // Interact with the building (enter or exit)
    public override Items.Item Interact(Items.Item item = null)
    {
        Items.Item toReturn = null;

        if (World.player.insideBuilding || World.player.insideMarket)
            Exit();
        else
            Enter();

        return toReturn;
    }
}