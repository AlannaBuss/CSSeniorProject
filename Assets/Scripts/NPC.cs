using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public enum timeOfDay
{
    morning, evening, night
}

public class NPC : MovingObject
{
    // Sprite representing this NPC
    private npcSprite sprite;
    public GameObject quest;

    // States this npc currently has
    public string name;
    public string personality;
    public List<string> states = new List<string>();
    public Boolean hasQuest = false;
    public Boolean atWork = false;
    public Boolean atHome = false;
    public Jobs.Job job;
    public Dictionary<Items.Item, int> inventory = new Dictionary<Items.Item, int>(); 

    public Boolean asleep = false;

    // Places this npc goes
    public Building home, work;
    public Vector3 homeTile, workTile;
    int workTimeStart, workTimeEnd;
    int sleepTimeStart, sleepTimeEnd;

    // Movement stuff
    int ID; // npc's id on the map
    private float timeloc;
    private float time;
    private float movementSpeed;
    int timeOfDayLength = 5; // in minutes; 15 in total
    timeOfDay currentTime;



    // Use this for initialization
    public void Start()
    {
        time = Time.time;
        timeloc = time;
        currentTime = timeOfDay.morning;
        fillInventoy();
        base.Start();
    }

    private void fillInventoy()
    {
        int invSize = Random.Range(job.inventoryMin, job.inventoryMax);
        for(int i = 0; i < invSize; i ++)
        {
            Items.Item item = Jobs.getRandomItem(job);
            if (inventory.ContainsKey(item))
            {
                inventory[item]++;
            }
            else
            {
                inventory.Add(item, 1);
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {

        // NPC goes to work
        if (currentTime == timeOfDay.morning && Time.time - timeloc > movementSpeed)
        {
            // NPC wakes up
            asleep = false;
            atHome = false;
            timeloc = Time.time;

            // NPC begins walking to work
            if (!atWork && goTowards(workTile))
            {
                // enter work
                atWork = true;
            }
        }
        // NPC goes home
        if (currentTime == timeOfDay.evening && Time.time - timeloc > movementSpeed)
        {
            // NPC stops working
            atWork = false;
            timeloc = Time.time;

            // NPC begins walking home
            if (!atHome && goTowards(homeTile))
            {
                // enter home
                atHome = true;
            }
        }
        // NPC goes to sleep
        if (currentTime == timeOfDay.night)
        {
            atWork = false;
            timeloc = Time.time;

            // NPC begins walking home (if not already there)
            if (!atHome && goTowards(homeTile))
            {
                // enter home and start sleeping
                atHome = true;
                asleep = true;
            }
        }

        // Change the time of day
        if (Time.time - time > timeOfDayLength * 60)
        {
            time = Time.time;
            if (currentTime == timeOfDay.morning)
                currentTime = timeOfDay.evening;
            else if (currentTime == timeOfDay.evening)
                currentTime = timeOfDay.night;
            else
                currentTime = timeOfDay.morning;
        }
    }

    // Create the initial NPC
    public void init(Vector3 homeTile, Vector3 workTile, int ID)
    {
        // Decide where the npc works and lives
        this.homeTile = homeTile;
        this.workTile = workTile;
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

    // Instantiate a quest
    public void initQuest()
    {
        hasQuest = true;
        quest = Instantiate(quest) as GameObject;

        if (sprite != null)
        {
            PlaceAt(mapX, mapY, tileX, tileY, 0);
            quest.SetActive(false);
        }
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



    // Called when the object cannot move
    protected override void OnCantMove<T>(T component)
    {

    }

    // Attempts to move to the given tile
    protected override bool MoveToTile(int xDir, int yDir)
    {
        bool moved;
        int pMapX = map.player.mapX;
        int pMapY = map.player.mapY;
        int pTileX = map.player.tileX;
        int pTileY = map.player.tileY;

        // Collision checking
        if (pTileX == tileX + xDir && pTileY == tileY + yDir)
        {
            // On the same map
            if (pMapY == mapY && pMapX == mapX)
            {
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
            mood = Random.Range(0, 4);

        // update sprite to match state
        states.Add(default_states[mood]);
        sprite.setState(states[0]);
    }

    // Decides what the NPC's initial personality will be
    private void initPersonality()
    {
        // Initiate the personality
        int persona = Random.Range(0, 10);
        string[] personalities = { "helpful", "aggressive", "outgoing", "alcoholic", "greedy", "shy", "brave", "amoral", "lazy", "psychotic" };

        // update sprite to match personality
        personality = personalities[persona];
        sprite.setState(personality);
    }

    // 
    private void startWorking()
    {

    }

    private void stopWorking()
    {

    }

    private void startSleeping()
    {

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
            {
                MoveToTile(0, my / (Math.Abs(my)));
            }
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
            {
                MoveToTile(mx / (Math.Abs(mx)), 0);
            }
        }
        // Move up or down to the next tile
        else if (ty != 0)
        {
            MoveToTile(0, ty / (Math.Abs(ty)));
        }
        // Move left or right to the next tile
        else if (tx != 0)
        {
            MoveToTile(tx / (Math.Abs(tx)), 0);
        }
        // Reached the target destination
        else
        {
            reachedDestination = true;
        }

        return reachedDestination;
    }
}