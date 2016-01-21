using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;


public class Building : Object
{
    // Building information
    public string name;             // The name of the building
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

    /*
    // Adds a new product to the market
    public void AddGood()
    {
        // Check to make sure the market isn't already fully stocked
        if (items.Count < 10)
            items.Add(Goods.getRandomItem(itemType));
    }

    // Attempts to buy a product from the market
    public bool BuyGood(int i)
    {
        bool boughtItem = false;

        if (i >= 0 && i < items.Count)
        {
            if (map.player.money >= items[i].value)
            {
                boughtItem = true;
                map.player.money -= items[i].value;
                map.player.inventory.Add(items[i]);
                RemoveGood(i);
            }
        }
        return boughtItem;
    }
    */

    // Draws the inside of the building when Player enters
    public void Enter()
    {
        // Building can be entered
        if (tag != "MARKET")
        {
            player.insideBuilding = true;
            outside.Undraw();
            inside.Draw();
        }
        // Building is a market; display goods that can be bought
        else
        {
            player.insideMarket = true;
            /*
            print("Choose the # of the item you wish to purchase.");
            print("Press 'x' to exit.");

            for (int i = 0; i < items.Count; i++)
            {
                print("[" + i + "]   " + items[i].name + ": " + items[i].description + " " + items[i].value + "gold.");
            }
            */
        }
    }

    // Draws the outside of the building when Player exits
    public void Exit()
    {
        if (tag != "MARKET")
        {
            player.insideBuilding = false;
            inside.Undraw();
            outside.Draw();
        }
        else
        {
            player.insideMarket = false;
        }
    }


    /*
    // Sets up the initial products sold here if this is a market
    private void InitGoods()
    {
        // The type of product sold here
        string[] types = { "fish", "meat", "fruit", "vegetables", "flowers" }; // add wood and metal later for blacksmith quests
        itemType = types[Random.Range(0, types.Length)];
        items = new List<Item>();
        name = itemType + " market";

        // The number of products sold initially
        int numItems;
        if (map.season == "summer")
            numItems = 7;
        else if (map.season == "spring")
        {
            numItems = 5;
            if (itemType == "flowers")
                numItems = 10;
        }
        else if (map.season == "fall")
            numItems = 3;
        else // winter
            numItems = 1;

        // Stock the store
        for (int i = 0; i < numItems; i++)
        {
            items.Add(Goods.getRandomItem(itemType));
        }
    }

    // Removes a product from the market
    private void RemoveGood(int i)
    {
        items.RemoveAt(i);
    }
    */
}