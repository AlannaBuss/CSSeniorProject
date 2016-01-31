using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* TYPES OF OBJECT TAGS:
 * Objects can have multiple tags, and a > denotes the following "child" tags also have that parent tag
 * 
 ANIMAL > COW PIG                             // animals can be killed to collect meat, cows can be milked
 WALL > BUSH >  ROSEBUSH                      // walls block the player
      > ROCKWALL > OREWALL                    // ore walls have ore in them which can be mined
      > BRICKWALL                             // rose bushes have roses in them which can be collected
      > DIRTWALL
      > FENCE > WOODFENCE METALFENCE
 OBJECT > GEMSTONE > RUBY SAPPHIRE EMERALD    // objects can be interacted with/picked up (and disappear afterwards?)
        > ROCK                                // gemstones are mined to get gems, rocks are mined to get ore
 TREE > APPLETREE ORANGETREE                  // trees can be cut down to get wood, or have fruit collected from
      > STUMP                                 // stumps will grow back into trees over time
 CROP > CARROT CORN LETTUCE TOMATO            // crops can be collected to get vegetables
 FURNITURE > BED
 * 
 */

public class Object : MonoBehaviour
{
    public List<String> tags;       // Represents what type of object this is
    public int mapX, mapY;          // object's location on the map
    public int tileX, tileY;        // object's location on the tile

    public Boolean hasQuest;	    // Quest flag
    public GameObject quest;		// Object representing a quest
	public Quest mission;

    public Textbox textbox;         // Used for displaying text to screen


    // Places the object at the given map location
    public void PlaceAt(int mX, int mY, int tX, int tY, int tZ)
    {
        mapX = mX;
        mapY = mY;
        tileX = tX;
        tileY = tY;
        transform.position = new Vector3(tX, tY, tZ);

        if (hasQuest)
        {
            quest.GetComponent<Transform>().position = new Vector3(tX, tY + .8f, tZ);
        }
    }

	public virtual void initQuest(Quest givenMission)
    {
		mission = givenMission;
        hasQuest = true;
        quest = Instantiate(quest) as GameObject;

        PlaceAt(mapX, mapY, tileX, tileY, 0);
        quest.SetActive(false);
    }

    //Called when an object is interacted with.
    public virtual Items.Item Interact()
    {
        Items.Item toReturn = null;

        // WALLS
        if (tags.Contains("ROSEBUSH")) {
            textbox.Write("You picked some roses");
            toReturn = Items.getItemWithName("rose");
        }
        else if (tags.Contains("BUSH")) {
            textbox.Write("Smells nice");
        }
        else if (tags.Contains("WALL")) {
            textbox.Write("What a lovely " + tags[tags.Count - 1]);
        }
        // ANIMALS
        else if (tags.Contains("PIG"))
        {
            textbox.Write("Oink!");
        }
        else if (tags.Contains("COW")) {
            textbox.Write("The COW gave you some milk");
            toReturn = Items.getItemWithName("milk");
        }
        // TREES
        else if (tags.Contains("APPLETREE"))
        {
            textbox.Write("You got some apples");
            toReturn = Items.getItemWithName("apple");
        }
        else if (tags.Contains("ORANGETREE")) {
            textbox.Write("You got some...oranges...");
            toReturn = Items.getItemWithName("orange");
        }
        else if (tags.Contains("STUMP")) {
            textbox.Write("It's a stump.");
        }
        else if (tags.Contains("TREE")) {
            textbox.Write("It's a tree.");
        }
        // CROPS
        else if (tags.Contains("CARROT")) {
            textbox.Write("You got some carrots.");
            toReturn = Items.getItemWithName("carrot");
        }
        else if (tags.Contains("CORN")) {
            textbox.Write("You got some corn.");
            toReturn = Items.getItemWithName("corn");
        }
        else if (tags.Contains("LETTUCE")) {
            textbox.Write("You got some lettuce.");
            toReturn = Items.getItemWithName("lettuce");
        }
        else if (tags.Contains("TOMATO")) {
            textbox.Write("You got some tomato.");
            toReturn = Items.getItemWithName("tomato");
        }

        return toReturn;
    }
}