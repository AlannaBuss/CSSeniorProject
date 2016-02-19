using UnityEngine;
using System.Collections;

public class QuestItem : Object {

    private GameObject sprite;
    private Items.Item item;
    private Items.Item requiredItem;
    private bool requiresItem;

    // Create a new ground item that can be retrieved
    public QuestItem(GameObject sprite, Items.Item i)
    {
        this.sprite = Instantiate(sprite);
        tags = new System.Collections.Generic.List<string>();
        tags.Add("QUEST");
        item = i;
        requiresItem = false;
        Undraw();
    }

    // Create a new ground item that requires another item to be retrieved
    public QuestItem(GameObject sprite, Items.Item i, Items.Item required)
    {
        this.sprite = Instantiate(sprite);
        tags = new System.Collections.Generic.List<string>();
        tags.Add("QUEST");
        item = i;
        requiresItem = true;
        requiredItem = required;
        Undraw();
    }

    // Interact with this item
    public override Items.Item Interact(Items.Item item = null)
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
}
