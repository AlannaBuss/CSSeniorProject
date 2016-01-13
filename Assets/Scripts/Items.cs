using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
public class Items : MonoBehaviour {

    static List<Item> items;

	// Use this for initialization
	public static void Start () {
        items = new List<Item>();
       string[] files = Directory.GetFiles("items");
        foreach (string file in files)
        {
            Item item = new Item();
            string[] lines = File.ReadAllLines(file);
            
            foreach(string line in lines)
            {
                int x = 0;
                bool b = false; ;
                string[] l = line.Split(' ');
                print(l[0]);
                try
                {
                    if (l[0].Equals("tags"))
                    {
                        List<string> tags = new List<string>();
                        for (int i = 2; i < l.Length; i++)
                            tags.Add(l[2]);
                        item.tags = tags;
                    }
                    else if (int.TryParse(l[2], out x))
                        item.GetType().GetProperty(l[0]).SetValue(item, x, null);
                    else if (bool.TryParse(l[2], out b))
                        item.GetType().GetProperty(l[0]).SetValue(item, b, null);
                    else
                        item.GetType().GetProperty(l[0]).SetValue(item, l[2], null);

                }catch(NullReferenceException)
                {
                    print("Error with item " + file + " property " + l[0] + " is invalid");
                }
            }
            items.Add(item);
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public class Item
    {
        public Item() { }

        

        public int value { get ; set; }
        public string name { get; set; }
        public List<string> tags { get; set; }
        public bool canWield { get; set; }
        public bool isArmor { get; set; }
        public int damage { get; set; }
        public int armor { get; set; }
    }
    //maybe make this more effecient by keeping lists of all items with each tag
    internal static Item getRandomItemOfTag(string key)
    {
        
        List<Item> itemsWithTag = new List<Item>();
        foreach(Item item in items)
        {
            if(item.tags.Contains(key))
            {
                itemsWithTag.Add(item);
            }
        }
        return itemsWithTag[Random.Range(0, itemsWithTag.Count)];
    }
}
