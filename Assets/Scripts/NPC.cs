using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class NPC : MovingObject
{
    // Sprite representing this NPC
    private npcSprite sprite;

    // States this npc currently has
    public string name;
    public string personality;
    public List<string> states = new List<string>();
    public Boolean atWork = false;
    public Boolean atHome = false;
    public Boolean asleep = false;
    public Jobs.Job job;
    public Dictionary<Items.Item, int> inventory = new Dictionary<Items.Item, int>();

    // NPC this npc is talking to
    public Boolean talking = false;
    public string interactionType;
    private NPC interactingWith;

    // Places this npc goes
    public Building home, work;
    public Vector3 homeTile, workTile;

    // Movement stuff
    int ID; // npc's id on the map
    private float timeloc;
    private float movementSpeed;
    
    // Outside references
    public TileManager tile;    // tile the npc is on

    //Economy Stuff
    private int eaten = 0;
    private bool hasFood = true;
    int gold;
    // Use this for initialization
    public void Start()
    {
        timeloc = Time.time;
        fillInventoy();
        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        timeStep();       // walking
        checkGreeting();  // check for interactions
		hasQuest = true;
    }

    // Create the initial NPC
    public void init(Building h, Building w, int ID)
    {
        // Decide where the npc works and lives
        home = h;
        work = w;
        homeTile = home.loc;
        workTile = work.loc;
        this.ID = ID;
        home.owners.Add(this);
        work.owners.Add(this);

        // Create a random sprite and initialize it
        sprite = GetComponent<npcSprite>();
        sprite.init();

        // Create a random state and personality for the npc
        initState();
        initPersonality();

        // We're done with the sprite for now
        sprite.placeAt(new Vector3(tileX, tileY, 0));
        sprite.undraw();

        // How fast the NPC moves initially
        if (personality == "lazy")
            movementSpeed = Random.Range(1.5f, 2.0f);
        else
            movementSpeed = Random.Range(0.5f, 1.0f);
    }

    // Places the npc at the given location
    public void PlaceAt(int mX, int mY, int tX, int tY, int tZ)
    {
        Vector3 pos = new Vector3(tX, tY, tZ);
        mapX = mX;
        mapY = mY;
        tileX = tX;
        tileY = tY;
        transform.position = pos;

        if (hasQuest)
        {
            quest.GetComponent<Transform>().position = new Vector3(tX, tY + .8f, tZ);
        }
        if (sprite != null)
            sprite.placeAt(pos);
    }

    //So we still know how to init our quests.
	public override void initQuest(Quest givenMission)
    {
		base.initQuest(givenMission);
    }

    // Draws the NPC to the screen
    public void draw()
    {
        sprite.draw();

        if (hasQuest)
        {
            quest.SetActive(true);
        }
    }

    // Removes the npc from the screen
    public void undraw()
    {
        sprite.undraw();

        if (hasQuest)
        {
            quest.SetActive(false);
        }
    }

    // NPC is saying something
    public void speak(Boolean display)
    {
        string dialogue = Dialogue.getDialogue(personality, interactionType);

        // Write the text if the player is on the same screen
        if (display)
            World.textbox.Write(dialogue, sprite, false);

        // 10% chance to become sad
        if (personality == "shy") {
            if (Random.Range(0, 10) == 0)
                MakeSad();
        }
        // Becomes happy
        else if (personality == "outgoing")
            MakeHappy();
    }



	// NPCS should have an interaction special to them
    public override Items.Item Interact(Items.Item item = null)
	{
        Items.Item toReturn = null;

		if (hasQuest) {
            World.textbox.Write(mission.questSpeech(), sprite);
            World.textbox.Write(mission.finishQuest(), sprite); 
		} else {
            World.textbox.Write(Dialogue.getDialogue(personality, "player_response"), sprite);
		}

        return toReturn;
	}


    // Called when the object cannot move
    protected override void OnCantMove<T>(T component)
    {

    }

    // Attempts to move to the given tile
    protected override bool MoveToTile(int xDir, int yDir)
    {
        bool moved;
        int pMapX = World.player.mapX;
        int pMapY = World.player.mapY;
        int pTileX = World.player.tileX;
        int pTileY = World.player.tileY;

        // Collision checking
        if (distance(tileX, pTileX, tileY, pTileY) <= 1 && pMapX == mapX && pMapY == mapY)
            moved = false;
        else if (pTileX == tileX + xDir && pTileY == tileY + yDir)
        {
            // On the same map
            if (pMapY == mapY && pMapX == mapX) {
                moved = false;
            }
            // Heading to a different map
            else if ( (tileX == 0 && xDir == -1 && mapX - 1 == pMapX && mapY == pMapY) || // map left
                 (tileX == 9 && xDir == 1 && mapX + 1 == pMapX && mapY == pMapY) ||  // map right
                 (tileY == 0 && yDir == -1 && mapY - 1 == pMapY && mapX == pMapX) || // map below
                 (tileY == 9 && yDir == 1 && mapY + 1 == pMapY && mapX == pMapX) )   // map above
            {
                moved = false;
            }
            // No collision
            else
            {
                moved = base.MoveToTile(xDir, yDir);
                PlaceAt(mapX, mapY, tileX, tileY, 0);
            }
        }
        else
        {
            moved = base.MoveToTile(xDir, yDir);
            PlaceAt(mapX, mapY, tileX, tileY, 0);
        }

        return moved;
    }



    // Decides what the NPC's initial state will be
    private void initState()
    {
        int mood = 0;
        string[] default_states = { "normal", "happy", "sad", "angry" };

        // 25% chance to be something other than neutral state on default
        if (Random.Range(0, 4) == 0)
            mood = Random.Range(0, default_states.Length);
        if (default_states[mood] == "happy")
            World.AddChaos(World.NPC_HAPPY);
        else if (default_states[mood] == "sad" || default_states[mood] == "angry")
            World.AddChaos(World.NPC_UPSET);

        // update sprite to match state
        states.Add(default_states[mood]);
        sprite.setState(states[0]);
    }

    // Decides what the NPC's initial personality will be
    private void initPersonality()
    {
        // Initiate the personality
        string[] personalities = { "helpful", "aggressive", "outgoing", "alcoholic", "greedy", "shy", "brave", "amoral", "lazy"};
        personality = personalities[Random.Range(0, personalities.Length)];

        // 5% chance of being psychotic
        if (Random.Range(0, 100) < 5) {
            personality = "psychotic";
            World.AddChaos(World.NPC_PSYCHOTIC);
        }

        // update sprite to match personality
        sprite.setState(personality);
    }

    private void fillInventoy()
    {
        gold = Random.Range(5, 30); //need to add base gold and pay rates to jobs.
        int invSize = Random.Range(job.inventoryMin, job.inventoryMax);
        for (int i = 0; i < invSize; i++)
        {
            Items.Item item = Jobs.getRandomItem(job);
            if (inventory.ContainsKey(item))
                inventory[item]++;
            else
                inventory.Add(item, 1);
        }
    }

    // NPC starts walking towards the given location
    private bool goTowards(Vector3 tile)
    {
        // Returns true if the NPC has reached the location
        bool reachedDestination = false;

        // location of the tile on the map
        int tMapX = (int)(tile.x / 10);
        int tMapY = (int)(tile.y / 10);
        int tTileX = (int)(tile.x % 10);
        int tTileY = (int)(tile.y % 10);

        // delta x and y of the npc tile and the destination tile
        int mx = tMapX - mapX;
        int my = tMapY - mapY;
        int tx = tTileX - tileX;
        int ty = tTileY - tileY;

        // Move up or down to the next map tile
        if (my != 0)
        {
            // Walk along the road
            if (tileX != 4 && tileX != 5)
            {
                int dx = Math.Min(4 - tileX, 5 - tileX);
                MoveToTile(dx / (Math.Abs(dx)), 0);
            }
            else
                MoveToTile(0, my / (Math.Abs(my)));
        }
        // Move left or right to the next map tile
        else if (mx != 0)
        {
            // Walk along the road
            if (tileY != 4 && tileY != 5)
            {
                int dy = Math.Min(4 - tileY, 5 - tileY);
                MoveToTile(0, dy / (Math.Abs(dy)));
            }
            else
                MoveToTile(mx / (Math.Abs(mx)), 0);
        }
        // Move up or down to the next tile
        else if (ty != 0)
            MoveToTile(0, ty / (Math.Abs(ty)));
        // Move left or right to the next tile
        else if (tx != 0)
            MoveToTile(tx / (Math.Abs(tx)), 0);
        // Reached the target destination
        else
            reachedDestination = true;

        return reachedDestination;
    }

    // Timebased checking for what NPC is doing
    private void timeStep()
    {
        // NPC goes to work
        if (World.GetTimeOfDay() == timeOfDay.morning && Time.time - timeloc > movementSpeed)
        {
            // NPC wakes up
            asleep = false;
            atHome = false;
            timeloc = Time.time;
            if (eaten < 1)
            {
                eat();
            }
            // NPC begins walking to work
            if (!atWork && goTowards(workTile))
            {
                // enter work
                atWork = true;
            }
        }
        // NPC goes home
        if (World.GetTimeOfDay() == timeOfDay.evening && Time.time - timeloc > movementSpeed)
        {
            // NPC stops working
            atWork = false;
            timeloc = Time.time;
            if (eaten < 2)
            {
                eat();
            }
            // NPC begins walking home
            if (!atHome && goTowards(homeTile))
            {
                // enter home
                atHome = true;
            }
        }
        // NPC goes to sleep
        if (World.GetTimeOfDay() == timeOfDay.night)
        {
            atWork = false;
            timeloc = Time.time;
            eaten = 0;
            // NPC begins walking home (if not already there)
            if (!atHome && goTowards(homeTile))
            {
                // enter home and start sleeping
                atHome = true;
                asleep = true;
            }
        }
    }

    private void eat()
    {
        int i;
        eaten++;
        Items.Item toEat = Items.getRandomItemOfTag("FOOD", inventory);
        if (toEat == null)
        {
            hasFood = false;
            return;
        }
        i = inventory[toEat];
        if (i < 2)
        {
            inventory.Remove(toEat);
        }
        else
        {
            inventory[toEat]--;
        }

    }

    // Sees if it can interact with anyone
    private void checkGreeting()
    {
        List<GameObject> npcs = tile.npcs;

        // NPC is already talking to someone else
        if (talking)
            return;

        // Check for people to interact with
        for (int i = 0; i < npcs.Count; i++)
        {
            NPC npc = npcs[i].GetComponent<NPC>();

            // Check in a 2 square radius
            if (distance(npc.tileX, npc.tileY, tileX, tileY) <= 2 && tileX != npc.tileX && tileY != npc.tileY)
            {
                // Check to make sure we haven't already talked to each other
                if (interactingWith == npc)
                    return;

                // Check if this NPC wants to initiate interaction
                if (personality == "outgoing") // 100% chance
                {
                    talking = true;
                    interactionType = "greeting";
                }
                else if (personality == "shy" && Random.Range(0, 10) == 0) // 10% chance
                {
                    talking = true;
                    interactionType = "greeting";
                }
                else if (Random.Range(0, 2) == 0) // 50% chance
                {
                    talking = true;
                    interactionType = "greeting";
                }

                interactingWith = npc;
                interactingWith.talking = true;
                interactingWith.interactionType = "greeting_response";
            }
        }
    }

    // Distance formula
    private double distance(int x1, int x2, int y1, int y2)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2));
    }

    // Removes the given state from the NPC
    private void RemoveState(string toRemove)
    {
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i] == toRemove)
                states.RemoveAt(i);
        }
    }

    private void MakeHappy()
    {
        if (states.Contains("happy"))
            return;

        World.AddChaos(World.NPC_HAPPY);
        RemoveState("normal");
        RemoveState("sad");
        RemoveState("angry");
        states.Add("happy");
        sprite.setState("happy");
    }

    private void MakeSad()
    {
        if (states.Contains("sad"))
            return;

        World.AddChaos(World.NPC_UPSET);
        RemoveState("normal");
        RemoveState("happy");
        states.Add("sad");
        sprite.setState("sad");
    }

    private void MakeAngry()
    {
        if (states.Contains("angry"))
            return;

        World.AddChaos(World.NPC_UPSET);
        RemoveState("normal");
        RemoveState("happy");
        states.Add("angry");
        sprite.setState("angry");
    }
}