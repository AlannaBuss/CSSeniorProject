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

    // Tells player if it is inside a building, market, or checking inventory
    public bool insideBuilding;
    public bool insideMarket;
    public bool inInventory;



    // Use this for initialization
    void Start()
    {
        base.Start();

        // Put default items in inventory
        inventory.Add(Items.getItemWithName("sword"));
        inventory.Add(Items.getItemWithName("axe"));
    }

    // Update is called once per frame
    void Update()
    {
        // Check number keys
        if (Input.GetKeyDown(KeyCode.Alpha0))
            checkNumberKey(0);
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            checkNumberKey(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            checkNumberKey(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            checkNumberKey(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            checkNumberKey(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            checkNumberKey(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            checkNumberKey(6);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            checkNumberKey(7);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            checkNumberKey(8);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            checkNumberKey(9);

        // Move left
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
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
        // Interaction
        else if (Input.GetKeyDown(KeyCode.Z))
            tryToInteract();
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

    private void checkNumberKey(int num)
    {
        if (inventory.Count >= num)
        {
            if (inInventory || insideMarket) {
                List<string> text = new List<string>();
                text.Add(inventory[num].name);
                text.Add(inventory[num].description);
                World.textbox.WriteAll(text);
            }
            else if (inventory[num].tags.Contains("FOOD")) {
                World.textbox.Write("You ate the " + inventory[num].name);
                inventory.RemoveAt(num);
            }
            else
                tryToInteract(inventory[num]);
        }
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

        //We don't want to have the player be considered something to interact with so
        //we turn this off temporarily
        boxCollider.enabled = false;

        RaycastHit2D leftHit = Physics2D.Linecast(start, left, blockingLayer);
        RaycastHit2D rightHit = Physics2D.Linecast(start, right, blockingLayer);
        RaycastHit2D downHit = Physics2D.Linecast(start, down, blockingLayer);
        RaycastHit2D upHit = Physics2D.Linecast(start, up, blockingLayer);
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

        if (target != null)
        {
            doInteraction(with, target);
            return true;
        }

        return false;
    }

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

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // Reference to the previous map we were on
        int oldX = mapX;
        int oldY = mapY;

        // Try to move in the direction of the input
        base.AttemptMove<T>(xDir, yDir);

        // Draw the new tile we're on
        map.Undraw(oldX, oldY);
        map.Draw(mapX, mapY);
    }

    protected override void OnCantMove<T>(T component)
    {

    }

    // Another object entered a trigger collider attached to this object
    private void OnTriggerEnter2D(Collider2D other)
    {
        // We collided with a building
        if (other.tag == "Building")
        {
            GameObject touching = other.gameObject;
            Building building = touching.GetComponent<Building>();
            World.textbox.Write("Press z to enter " + building.getName());
        }
        // We collided with an npc
        else if (other.tag == "NPC")
        {

        }
    }
}