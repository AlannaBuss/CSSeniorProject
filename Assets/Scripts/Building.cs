using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class Item
{
    public string name;
    public string description;
    public int value;

    public Item(string n, string d, int v)
    {
        name = n;
        description = d;
        value = v;
    }
}

public static class Goods
{
    // Fish
    static Item[] fish = { new Item("Tuna", "Not that fresh but you're hungry so heals 10 hp anyway.", 25),         // 50% chance
                           new Item("Halibutt", "The butt of a Halibut. Pretty tasty though. Heals 15 hp.", 50),    // 37% chance
                           new Item("Salmon", "The greatest of all fish. Heals full hp.", 200)                      // 12% chance
                         };
    static int[] fish_probability = { 0, 0, 0, 0, 1, 1, 1, 2 };

    // Meat
    static Item[] meat = { new Item("Lamb", "Still a baby, so it's juicy and tender. Heals 25 hp.", 75),
                           new Item("Pork", "Delicious. Hopefully this didn't used to be a villager. Heals 50 hp", 100), 
                           new Item("Chicken", "Are actually quite brave. You feel like you've been lied to. Heals 15 hp", 50),
                           new Item("Beef", "A whole cow, actually. Heals full hp.", 200)
                         };
    static int[] meat_probability = { 0, 1, 1, 2, 2, 3 };

    // Fruit
    static Item[] fruit = { new Item("Apple", "Crunchy and goes good in pie. Heals status.", 10), 
                            new Item("Orange", "Comes pre-sliced. Thanks, nature. Heals status.", 10), 
                            new Item("Starfruit", "Heavenly. Heals 20 hp and status", 50),
                            new Item("Dragonfruit", "Is actually a bomb. Use it to deal 50 damage. Not recommended for eating.", 50)
                          };
    static int[] fruit_probability = { 0, 0, 0, 1, 1, 1, 2, 3 };

    // Vegetables
    static Item[] vegetables = { new Item("Tomato", "Is actually a fruit. You feel like you've been lied to. Heals 15 hp and status.", 25),
                                 new Item("Corn", "Grown fresh. Heals 15 hp and status.", 25),
                                 new Item("Lettuce", "Grown fresh. Heals 15 hp and status.", 25),
                                 new Item("Carrots", "Grown fresh. Heals 15 hp and status.", 25)
                               };
    static int[] vegetable_probability = { 0, 1, 2, 3 };

    // Flowers
    static Item[] flowers = { new Item("Rose", "The most romantic of flowers.", 50),                                // 15% chance
                              new Item("Daisy", "Bright and yellow. Like the bee that sat on it.", 10),             // 23% chance
                              new Item("Dandelion", "Actually a weed, but they won't care. Probably.", 5),          // 38% chance
                              new Item("Violet", "Is actually blue. You feel like you've been lied to.", 15)        // 23% chance
                            };
    static int[] flower_probability = { 2, 2, 2, 2, 2, 1, 1, 1, 3, 3, 3, 0, 0 };

    // Return a random good of the given type
    public static Item getRandomItem(string type)
    {
        if (type == "fish")
            return fish[fish_probability[Random.Range(0, fish_probability.Length)]];
        else if (type == "meat")
            return meat[meat_probability[Random.Range(0, meat_probability.Length)]];
        else if (type == "fruit")
            return fruit[fruit_probability[Random.Range(0, fruit_probability.Length)]];
        else if (type == "vegetables")
            return vegetables[vegetable_probability[Random.Range(0, vegetable_probability.Length)]];
        else if (type == "flowers")
            return flowers[flower_probability[Random.Range(0, flower_probability.Length)]];
        else return null;
    }
}

public class Building : MonoBehaviour
{
    // Building information
    public string name;             // The name of the building
    public string type;             // The type of this building
    public TileManager inside;      // The inside of the building
    public TileManager outside;     // Reference to the tile the building is on
    public List<NPC> owners;        // Reference to the NPCs who live/work here

    // Prefab objects
    public GameObject[] outerWalls, walls, floors, objects;

    // Used for if the building is a market
    public List<Item> items;
    public string itemType;     // types of items that are generally sold here

    // Outside references
    public Player player;
    public MapManager map;



    // Sets up the building
    public void SetUp(TileManager o, string t)
    {
        outside = o;
        type = t;
        owners = new List<NPC>();

        if (type == "market")
        {
            InitGoods();
        }
    }

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
    
    // Draws the inside of the building when Player enters
    public void Enter()
    {
        // Building can be entered
        if (type != "market")
        {
            player.insideBuilding = true;
            outside.Undraw();
            inside.Draw();
        }
        // Building is a market; display goods that can be bought
        else
        {
            player.insideMarket = true;
            print("Choose the # of the item you wish to purchase.");
            print("Press 'x' to exit.");

            for (int i = 0; i < items.Count; i++)
            {
                print("[" + i + "]   " + items[i].name + ": " + items[i].description + " " + items[i].value + "gold.");
            }
        }
    }

    // Draws the outside of the building when Player exits
    public void Exit()
    {
        if (type != "market")
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
}