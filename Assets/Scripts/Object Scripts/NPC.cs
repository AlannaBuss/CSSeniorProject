using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public enum Personality
{
    // interactive
    helpful,        // positive
    outgoing,
    aggressive,     // negtive
    greedy,
    // reactive
    brave,
    shy,
    // disactive
    lazy,
    // disruptive
    alcoholic,
    // bad
    amoral,
    // PSYCHOTIC
    psychotic
}

public enum State
{
    // moods
    normal,
    happy,
    sad,
    angry,
    // status
    drunk,
    tired,
    dead,
    psychotic
}

public enum Recreation
{
    // lazy
    sleeping,
    // shy
    staying_home,
    // outgoing, helpful
    exploring_town,
    // alcoholic, aggressive
    drinking,
    // brave
    exploring_outdoors,
    // psychotic
    killing,
    // amoral, greedy
    stealing,
}

public class NPC : MovingObject
{
    // Sprite representing this NPC
    private npcSprite sprite;
    // States this npc currently has
    public string name;
    public Personality personality;
    public List<State> states = new List<State>();
    public Dictionary<Items.Item, int> inventory = new Dictionary<Items.Item, int>();
    public Boolean asleep = false, stealing = false;
    // Talking stuff
    public Boolean talking = false;
    public string interactionType;
    private NPC interactingWith;
    // Movement stuff
    public Building home, work;
    public Boolean atWork = false, atHome = false;
    public Boolean atRecreation = false, hasRecreation = false;
    public Recreation recreation;
    private Vector3 recreationLoc;
    private float movementSpeed;
    private float timeloc, timeloc2;
    // Economy Stuff
    public Jobs.Job job;
    private int eaten = 0;
    private bool hasFood = true;
    private bool goingToStore = false;
    private bool payed = false;
    private NPC storeTarget;
    int gold;



    // Use this for initialization
    public void Start()
    {
        timeloc = Time.time;
        timeloc2 = Time.time;
        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        if (states.Contains(State.dead))
            return;

        timeStep();       // walking
        checkGreeting();  // check for interactions
    }

    // Create the initial NPC
    public void init(Building h, Building w)
    {
        // Decide where the npc works and lives
        home = h;
        work = w;
        home.owners.Add(this);
        work.owners.Add(this);

        // Create a random sprite and initialize it
        sprite = GetComponent<npcSprite>();
        sprite.init();
        fillInventoy();

        // Create a random state and personality for the npc
        initState();
        initPersonality();
        sprite.placeAt(new Vector3(tileX, tileY, 0));
        sprite.undraw();

        // How fast the NPC moves
        if (personality == Personality.lazy)
            movementSpeed = Random.Range(1.0f, 1.5f);
        else
            movementSpeed = Random.Range(0.5f, 1.0f);
    }

    // Places the npc at the given location
    public override void PlaceAt(int mX, int mY, int tX, int tY)
    {
        Vector3 pos = new Vector3(tX, tY, 0);
        mapX = mX;
        mapY = mY;
        tileX = tX;
        tileY = tY;
        transform.position = pos;

        if (hasQuest)
            quest.GetComponent<Transform>().position = new Vector3(tX, tY + .8f, 0);
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
            quest.SetActive(true);
    }

    // Removes the npc from the screen
    public void undraw()
    {
        sprite.undraw();
        if (hasQuest)
            quest.SetActive(false);
    }

    // NPC is interacting with another NPC
    public void speak(Boolean display)
    {
        string dialogue = Dialogue.getDialogue(Enum.GetName(typeof(Personality), personality), interactionType);

        // DEAD
        if (states.Contains(State.dead)) {
            dialogue = "...";
        }
        // Talking to a dead person
        else if (interactingWith.states.Contains(State.dead)) {
            if (personality != Personality.psychotic)
                dialogue = "eek!!";
        }
        // SHY: chance to become sad
        else if (personality == Personality.shy)
        {
            // Helpful person
            if (interactingWith.personality == Personality.helpful) {
                SetState(State.happy);
                print("TALKED TO SOMEONE AND BECAME HAPPY");
            }
            // Aggressive person, or NPC is just too shy
            else if (interactingWith.personality == Personality.aggressive || Random.Range(0, 10) == 0) {
                SetState(State.sad);
                print("TALKED TO SOMEONE AND BECAME SAD");
            }
        }
        // OUTGOING: Becomes happy
        else if (personality == Personality.outgoing) {
            SetState(State.happy);
            print("TALKED TO SOMEONE AND BECAME HAPPY");
        }
        // PSYCHOTIC: chance to infect or kill
        else if (personality == Personality.psychotic &&
            interactingWith.personality != Personality.psychotic && !interactingWith.states.Contains(State.psychotic)) {
            // Kill the NPC
            if (states.Contains(State.psychotic)) {
                if (Random.Range(0, 100) < World.PsychopathKillChance()) {
                    interactingWith.SetState(State.dead);
                    print("DEAD COUNT: " + World.numKilled);
                }
            }
            // Turn person into a psychopath
            else if (Random.Range(0, 100) < World.PsychopathInfectChance() &&
                (interactingWith.states.Contains(State.sad) || interactingWith.states.Contains(State.angry) || interactingWith.personality == Personality.amoral)) {
                interactingWith.personality = Personality.psychotic;
                interactingWith.SetState(State.psychotic);
                print("PSYCHOPATH COUNT: " + World.GetNumPsychopaths());
            }
        }

        // Write the text if the player is on the same screen
        if (display)
            World.textbox.Write(dialogue, sprite, false);
        talking = false;
    }

    // NPCS should have an interaction special to them
    public override Items.Item Interact(Items.Item item = null)
    {
        Items.Item toReturn = null;

		if (hasQuest)
			World.textbox.Write (mission.interact (item), sprite);
        else if (item != null && item.tags.Contains("WEAPON")) {
            if (personality == Personality.psychotic)
                World.textbox.Write("You've stopped a psycopath!");
            else if (World.player.killed >= 5) {
                World.textbox.Write("The true psychopath is you.");
                World.player.killed++;
            }
            else {
                World.textbox.Write("Murderer.");
                World.player.killed++;
            }
            SetState(State.dead);
        }
        else if (states.Contains(State.dead))
            World.textbox.Write("This NPC is dead...");
        else
            World.textbox.Write(Dialogue.getDialogue(Enum.GetName(typeof(Personality), personality), "player_response"), sprite);

        return toReturn;
    }

    public static void Transaction(string tag, int amount, NPC buyer, NPC seller)
    {
        do {
            Items.Item tobuy = Items.getRandomItemOfTag(tag, seller.inventory);
            if (tobuy == null)
                break;

            seller.inventory[tobuy]--;
            if (seller.inventory[tobuy] == 0)
                seller.inventory.Remove(tobuy);
            if (!buyer.inventory.ContainsKey(tobuy))
                buyer.inventory.Add(tobuy, 0);
            buyer.inventory[tobuy]++;
            buyer.gold -= tobuy.value;
            seller.gold += tobuy.value;
            print(tobuy.name + " bought for " + tobuy.value + " buyer now has " + buyer.gold + " gold and seller now has " + seller.gold);
            amount--;
        } while (buyer.gold > 0 && amount > 0);
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

        // Collision checking with player
        if (distance(tileX, pTileX, tileY, pTileY) <= 1 && pMapX == mapX && pMapY == mapY)
            moved = false;
        else
        {
            moved = base.MoveToTile(xDir, yDir);
            PlaceAt(mapX, mapY, tileX, tileY);
        }

        return moved;
    }



    // Decides what the NPC's initial state will be
    private void initState()
    {
        State state = (State)Random.Range(0, 4);
        SetState(state);
    }

    // Decides what the NPC's initial personality will be
    private void initPersonality()
    {
        // Initiate the personality
        personality = (Personality)Random.Range(0, 10);

        // 5% chance of being psychotic
        if (Random.Range(0, 100) < 5 && World.GetNumPsychopaths() < 2)
        {
            personality = Personality.psychotic;
            World.AddPsychopath(1);
            print("PSYCHOPATH COUNT: " + World.GetNumPsychopaths());
        }
    }

    private void fillInventoy()
    {
        gold = Random.Range(100, 400); //need to add base gold and pay rates to jobs.
        int invSize = Random.Range(job.inventoryMin, job.inventoryMax);
        for (int i = 0; i < invSize; i++)
        {
            Items.Item item = Jobs.getRandomItem(job);
            AddToInventory(item, 1);
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
        else if (my == 0 && mx == 0)
            reachedDestination = true;

        return reachedDestination;
    }

    // Timebased checking for what NPC is doing
    private void timeStep()
    {
        // NPC goes to the store
        if (goingToStore && Time.time - timeloc > movementSpeed) {
            timeloc = Time.time;
            atWork = atHome = atRecreation = hasRecreation = asleep = false;

            if (goTowards(storeTarget.work.getLocation())) {
                Transaction("FOOD", Random.Range(2, 6), this, storeTarget);
                hasFood = true;
                goingToStore = false;
            }
        }
        // NPC goes to work
        else if (World.GetTimeOfDay() == timeOfDay.morning && Time.time - timeloc > movementSpeed) {
            // NPC wakes up
            timeloc = Time.time;
            asleep = atHome = atRecreation = hasRecreation = payed = false;
            
            // NPC eats breakfast
			if (eaten < 1)
                eat();

            // Sends them to get food at a randomized time
            if (atWork && !hasFood && !goingToStore) {
                if (Random.Range(0, 5) == 0) {
                    goingToStore = true;
                    storeTarget = World.map.getRandomNpcWithTag("FARM", false);
                }
            }
            // NPC begins walking to work
            if (!atWork && goTowards(work.getLocation()))
                atWork = true;
        }

        // NPC goes to recreational activity
        if (World.GetTimeOfDay() == timeOfDay.evening && Time.time - timeloc > movementSpeed) {
            // NPC stops working
            timeloc = Time.time;
            atWork = false;

            // NPC gets paid for their job
            if (!payed) {
                gold += job.wages;
                payed = true;
            }
            // NPC eats dinner
            if (eaten < 2)
                eat();

            // NPC begins walking to recreational activity
            if (!hasRecreation)
                recreationLoc = getRecreation();
            if (hasRecreation && !atRecreation && goTowards(recreationLoc))
                doRecreation();
        }
        // NPC goes home to sleep
        if (World.GetTimeOfDay() == timeOfDay.night && Time.time - timeloc > movementSpeed) {
            atRecreation = hasRecreation = false;
            timeloc = Time.time;
            eaten = 0;

            // NPC begins walking home
            if (!atHome && goTowards(home.getLocation()))
                atHome = asleep = true;
        }
    }

    private Vector3 getRecreation()
    {
        Vector3 loc = new Vector3(0, 0, 0);
        hasRecreation = true;

        // Person wants to go to a tavern
        if (personality == Personality.alcoholic || personality == Personality.aggressive) {
            recreation = Recreation.drinking;
            loc = World.findRandomBuilding(World.BUILDING_TAVERN);
        }
        // Person wants to walk around town talking to people
        if (personality == Personality.outgoing || personality == Personality.helpful) {
            recreation = Recreation.exploring_town;
            String[] areas = { World.AREA_TOWN, World.AREA_MARKET, World.AREA_FARM };
            loc = World.findRandomArea(areas[Random.Range(0, areas.Length)]);
        }
        // Person wants to explore the outdoors
        if (personality == Personality.brave) {
            recreation = Recreation.exploring_outdoors;
            String[] areas = { World.AREA_CAVE, World.AREA_FOREST, World.AREA_FARM };
            loc = World.findRandomArea(areas[Random.Range(0, areas.Length)]);
        }
        // Person wants to go sleep
        if (personality == Personality.lazy) {
            recreation = Recreation.sleeping;
            loc = home.getLocation();
        }
        // Person wants to go home
        if (personality == Personality.shy) {
            recreation = Recreation.staying_home;
            loc = home.getLocation();
        }
        // Person wants to steal
        if (personality == Personality.greedy || personality == Personality.amoral) {
            recreation = Recreation.stealing;
            storeTarget = World.map.getRandomNpcWithTag("MARKET", true);
            loc = storeTarget.work.getLocation();
        }
        // Person wants to go looking for people to kill
        if (personality == Personality.psychotic) {
            String[] places = { World.AREA_TOWN, World.AREA_MARKET, World.AREA_FARM, World.AREA_FOREST, World.AREA_CAVE };
            recreation = Recreation.killing;
            loc = World.findRandomArea(places[Random.Range(0, places.Length)]);
        }

        return loc;
    }

    private void doRecreation()
    {
        // Start recreation
        if (!atRecreation) {
            atRecreation = true;
            timeloc2 = Time.time;
        }
        // Do recreation
        if (atRecreation) {
            // Get drunk
            if (recreation == Recreation.drinking && !states.Contains(State.drunk))
            {
                states.Add(State.drunk);
                print("GOT DRUNK");
            }
            // Explore the area
            if (recreation == Recreation.exploring_outdoors && Time.time - timeloc2 > movementSpeed * 3) {
                timeloc2 = Time.time;
                atRecreation = false;
                recreationLoc = World.map.map[mapX][mapY].EmptyLocation();
                print("EXPLORE MORE");
            }
            // Take a nap
            if (recreation == Recreation.sleeping || (recreation == Recreation.staying_home && Time.time - timeloc2 > 30))
            {
                asleep = true;
                print("TOOK A NAP");
            }
            // Find another person
            if ((recreation == Recreation.killing || recreation == Recreation.exploring_town) && World.numPeopleOnTile(mapX, mapY) <= 1)
            {
                hasRecreation = false;
                print("LOOK FOR MORE PEOPLE");
            }
            // Attempt to steal something
            if (recreation == Recreation.stealing) {
                if (!stealing)
                    stealing = true;
                if (stealing && Time.time - timeloc2 > movementSpeed * 2) {
                    timeloc2 = Time.time;
                    print("TRYING TO STEAL");

                    // Succeeded in stealing something
                    if (Random.Range(0, 20) == 0) {
                        List<Items.Item> items = new List<Items.Item>(storeTarget.inventory.Keys);
                        Items.Item stolen = items[Random.Range(0, items.Count)];
                        AddToInventory(stolen, 1);
                        storeTarget.AddToInventory(stolen, -1);
                        storeTarget.SetState(State.angry);
                        hasRecreation = stealing = false;
                        print("SOMETHING WAS STOLEN");
                    }
                }
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
            inventory.Remove(toEat);
        else
            inventory[toEat]--;
    }

    // Sees if it can interact with anyone
    private void checkGreeting()
    {
        List<GameObject> npcs = World.map.map[mapX][mapY].npcs;

        // NPC is already doing something else
        if (talking || atHome || atWork)
            return;

        // Check for people to interact with
        foreach (GameObject npcObject in npcs) {
            NPC npc = npcObject.GetComponent<NPC>();

            // Check if the npc can speak
            if (!npc.atHome && !npc.atWork && npc != this && npc != interactingWith && distance(npc.tileX, npc.tileY, tileX, tileY) <= 2) {

                // Check if we want to initiate interaction
                if (wantsToTalk()) {
                    talking = true;
                    interactionType = "greeting";
                    interactingWith = npc;

                    interactingWith.talking = true;
                    interactingWith.interactingWith = this;
                    interactingWith.interactionType = "greeting_response";
                }
            }
        }
    }

    // Checks whether the NPC wishes to talk to another NPC
    private Boolean wantsToTalk()
    {
        if (personality == Personality.outgoing)
            return true;
        else if (personality == Personality.shy)
            return Random.Range(0, 10) == 0;
        else
            return Random.Range(0, 2) == 0;
    }

    // Removes the given state from the NPC
    private void RemoveState(State toRemove)
    {
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i] == toRemove)
                states.RemoveAt(i);
        }
    }

    public void SetState(State state)
    {
        if (states.Contains(state))
            return;

        if (state == State.happy)
        {
            World.AddChaos(World.NPC_HAPPY);
            RemoveState(State.normal);
            RemoveState(State.sad);
            RemoveState(State.angry);
        }
        else if (state == State.angry || state == State.sad)
        {
            World.AddChaos(World.NPC_UPSET);
            RemoveState(State.normal);
            RemoveState(State.happy);
            World.numUnhappy++;
        }
        else if (state == State.normal)
        {
            RemoveState(State.happy);
            RemoveState(State.sad);
            RemoveState(State.angry);
        }
        else if (state == State.dead)
        {
            if (personality != Personality.psychotic)
                World.AddChaos(World.NPC_DIES);
            World.numKilled++;
        }
        else if (state == State.psychotic)
        {
            World.AddChaos(World.NPC_PSYCHOTIC);
            World.AddPsychopath(1);
        }

        states.Add(state);
        sprite.setState(Enum.GetName(typeof(State), state));
    }

    // Distance formula
    private double distance(int x1, int x2, int y1, int y2)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2));
    }

    // Adds or removes an item from the inventory
    private void AddToInventory(Items.Item item, int amount)
    {
        if (inventory.ContainsKey(item))
            inventory[item] += amount;
        else
            inventory.Add(item, amount);

        if (inventory[item] <= 0)
            inventory.Remove(item);
    }
}