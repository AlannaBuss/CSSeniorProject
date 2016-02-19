using UnityEngine;
using System.Collections;

public class QuestItem : Object {

    // Prefabs
    public GameObject fish, meat, fruit, vegetable, flower, misc;

    private GameObject sprite;
    private Items.Item item;
    private Items.Item requiredItem;
    private bool requiresItem;


    // Create a new ground item that can be retrieved
    public void Setup(Items.Item i)
    {
        for (int n = 0; n < i.tags.Count; n++)
        {
            print(i.tags[n] + "\n");
        }

        tags = new System.Collections.Generic.List<string>();
        tags.Add("QUEST");
        item = i;
        requiresItem = false;
        sprite = Instantiate(decideSprite());
        Undraw();
    }

    // Create a new ground item that requires another item to be retrieved
    public void Setup(Items.Item i, Items.Item required)
    {
        Setup(i);
        requiresItem = true;
        requiredItem = required;
    }

    // Interact with this item
    public override Items.Item Interact(Items.Item i = null)
    {
        // Check if this item can be retrieved
        if (requiresItem && requiredItem.Equals(item) || !requiresItem)
        {
            World.textbox.Write("You found a " + item.name + "!");
            Remove();
            return item;
        }
        return null;
    }

    // Place the quest item and the given location in the map
    public override void PlaceAt(int mapX, int mapY, int tileX, int tileY)
    {
        Vector3 pos = new Vector3(tileX, tileY, 0f);
        this.mapX = mapX;
        this.mapY = mapY;
        this.tileX = tileX;
        this.tileY = tileY;

        transform.position = pos;
        sprite.transform.position = pos;
        if (hasQuest)
            quest.GetComponent<Transform>().position = new Vector3(tileX, tileY + .8f, 0);
        World.map.map[mapX][mapY].questItem = this.gameObject;
    }

    // Places the item at a random location in the world
    public void PlaceRandomly()
    {
        int x = Random.Range(0, 11);
        int y = Random.Range(0, 11);

        Vector3 loc = World.map.map[x][y].EmptyLocation();
        while (loc.x == -1)
        {
            x = Random.Range(0, 11);
            y = Random.Range(0, 11);
            loc = World.map.map[x][y].EmptyLocation();
        }

        PlaceAt(x, y, (int)loc.x, (int)loc.y);
    }

    // Removes the quest item from the map
    public void Remove()
    {
        Undraw();
        World.map.map[mapX][mapY].questItem = null;
    }

    // Draw the item to the map
    public void Draw()
    {
        sprite.SetActive(true);
        this.gameObject.SetActive(true);
    }

    // Undraw the item from the map
    public void Undraw()
    {
        sprite.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public string getLocationName()
    {
        TileManager tile = World.map.map[mapX][mapY];
        return tile.tileType;
    }


    // Decides which sprite to use for the item
    private GameObject decideSprite()
    {
        GameObject sprite = misc;

        if (item.tags.Contains("MEAT"))
            sprite = meat;
        else if (item.tags.Contains("VEGETABLE"))
            sprite = vegetable;
        else if (item.tags.Contains("FISH"))
            sprite = fish;
        else if (item.tags.Contains("FLOWER"))
            sprite = flower;
        else if (item.tags.Contains("FRUIT"))
            sprite = fruit;

        return sprite;
    }
}
