using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class Player : MovingObject
{
    // Player stats
    public int money = 0;
    public List<Items.Item> inventory = new List<Items.Item>();
    public int killed = 0;
    // Player sprite animation
    public Sprite[] anim;
    public Sprite[] evilAnim;
    private float timeLoc;
    private float timeAnim = 0.5f;
    private int curAnim = 0;

    // Tells player if it is inside a building, market, or checking inventory
    public bool insideBuilding;
    public bool insideMarket;
    public bool inInventory;
    // Detect but don't collide with NPCs
    public LayerMask npcLayer;


    // Use this for initialization
    void Start()
    {
        base.Start();

        // Put default items in inventory
        inventory.Add(Items.getItemWithName("sword"));
        inventory.Add(Items.getItemWithName("axe"));

        // Initialize time animation
        timeLoc = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Animate sprite
        if (Time.time - timeLoc > timeAnim)
        {
            curAnim = (curAnim + 1) % anim.Length;
            if (killed >= 5)
                GetComponent<SpriteRenderer>().sprite = evilAnim[curAnim];
            else
                GetComponent<SpriteRenderer>().sprite = anim[curAnim];
            timeLoc = Time.time;
        }

        // Check for a button press
        KeyPressCheck();
    }


    //Sees if there is building or NPC in the surrounding tiles 
    protected virtual bool tryToInteract(Items.Item with = null)
    {
        //Start is where the player is
        Vector2 start = transform.position;

        //Make vectors for the surrounding area.
        Vector2 left = start + new Vector2(-1, 0);
        Vector2 right = start + new Vector2(1, 0);
        Vector2 down = start + new Vector2(0, -1);
        Vector2 up = start + new Vector2(0, 1);
        boxCollider.enabled = false;

        RaycastHit2D leftHit = Physics2D.Linecast(start, left, blockingLayer | npcLayer);
        RaycastHit2D rightHit = Physics2D.Linecast(start, right, blockingLayer | npcLayer);
        RaycastHit2D downHit = Physics2D.Linecast(start, down, blockingLayer | npcLayer);
        RaycastHit2D upHit = Physics2D.Linecast(start, up, blockingLayer | npcLayer);
        Object target = null;

        //turn this back on since we have done all of our hit creation.
        boxCollider.enabled = true;

        //Return true if any of the surrounding area has an object you interact with it
        if (leftHit.transform != null)
            target = leftHit.collider.gameObject.GetComponent<Object>();
        else if (rightHit.transform != null)
            target = rightHit.collider.gameObject.GetComponent<Object>();
        else if (upHit.transform != null)
            target = upHit.collider.gameObject.GetComponent<Object>();
        else if (downHit.transform != null)
            target = downHit.collider.gameObject.GetComponent<Object>();

        if (target != null) {
            doInteraction(with, target);
            return true;
        }

        return false;
    }

    // Tries to move in the given direction
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // Reference to the previous map we were on
        int oldX = mapX;
        int oldY = mapY;

        // Try to move in the direction of the input
        base.AttemptMove<T>(xDir, yDir);

        // Draw the new tile we're on
        World.map.Undraw(oldX, oldY);
        World.map.Draw(mapX, mapY);
    }

    // Cannot move in that direction (blocked by something)
    protected override void OnCantMove<T>(T component)
    {
        //TODO: play sound effect
    }

    // Moves in the given direction
    protected override bool MoveToTile(int xDir, int yDir)
    {
        // Moving to a different map tile
        if (tileX == 0 && xDir == -1 && World.map.map[mapX - 1][mapY].ObjectAt(9, tileY) ||
            tileX == 9 && xDir == 1 && World.map.map[mapX + 1][mapY].ObjectAt(0, tileY) ||
            tileY == 0 && yDir == -1 && World.map.map[mapX][mapY - 1].ObjectAt(tileX, 9) ||
            tileY == 9 && yDir == 1 && World.map.map[mapX][mapY + 1].ObjectAt(tileX, 0))
        {
            World.textbox.Clear();
            int x = xDir == -1 ? 9 : xDir == 1 ? 0 : tileX;
            int y = yDir == -1 ? 9 : yDir == 1 ? 0 : tileY;

            // Something blocking on the other tile
            if (World.map.map[mapX + xDir][mapY + yDir].ObjectAt(x, y))
                return false;
        }

        return base.MoveToTile(xDir, yDir);
    }



    // Check for key press events
    private void KeyPressCheck()
    {
        // Check number (item) keys
        if (Input.GetKeyDown(KeyCode.Alpha0))
            useItem(0);
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            useItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            useItem(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            useItem(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            useItem(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            useItem(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            useItem(6);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            useItem(7);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            useItem(8);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            useItem(9);
        // Interaction
        else if (Input.GetKeyDown(KeyCode.Z))
            tryToInteract();
        if (insideMarket || insideBuilding)
            return;
        // Move left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            AttemptMove<Player>(-1, 0);
        // Move right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            AttemptMove<Player>(1, 0);
        // Move up
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            AttemptMove<Player>(0, 1);
        // Move down
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            AttemptMove<Player>(0, -1);
        // Hide or show textbox
        else if (Input.GetKeyDown(KeyCode.H))
            World.textbox.Hide();
        // Hide or show inventory
        else if (Input.GetKeyDown(KeyCode.I))
        {
            World.inventoryBox.ShowInventory(inventory);
            inInventory = World.inventoryBox.isDrawn;
        }
    }

    // Interact with the given target with the used item
    private void doInteraction(Items.Item with, Object target)
    {
        Items.Item received = target.Interact(with);

        if (received != null)
        {
            if (inventory.Count < 10)
                inventory.Add(received);
            else
                World.textbox.Write("Your inventory is full.");
        }
    }

    // Check if we are using or checking the selected item
    private void useItem(int num)
    {
        if (inventory.Count >= num)
        {
            if (inInventory || insideMarket)
            {
                List<string> text = new List<string>();
                text.Add(inventory[num].name);
                text.Add(inventory[num].description);
                World.textbox.WriteAll(text);
            }
            else if (inventory[num].tags.Contains("FOOD"))
            {
                World.textbox.Write("You ate the " + inventory[num].name);
                inventory.RemoveAt(num);
            }
            else
                tryToInteract(inventory[num]);
        }
    }

    // Another object entered a trigger collider attached to this object
    private void OnTriggerEnter2D(Collider2D other)
    {
        // We collided with a building
        if (other.tag == "Building") {
            GameObject touching = other.gameObject;
            Building building = touching.GetComponent<Building>();
            World.textbox.Write("Press z to enter " + building.getName());
        }
    }
}