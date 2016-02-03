using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

/* TYPES OF OBJECT TAGS:
 * Objects can have multiple tags, and a > denotes the following "child" tags also have that parent tag
 OBJECT > GEMSTONE > RUBY SAPPHIRE EMERALD    // objects can be interacted with/picked up (and disappear afterwards?)
        > ROCK                                // gemstones are mined to get gems, rocks are mined to get ore
 TREE > APPLETREE ORANGETREE                  // trees can be cut down to get wood, or have fruit collected from
      > STUMP                                 // stumps will grow back into trees over time
 CROP > CARROT CORN LETTUCE TOMATO            // crops can be collected to get vegetables
      > SOIL                                  // grows into a crop the next day
 FURNITURE > BED
 ITEM > goundItem
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

    public GameObject[] afterInteraction;
    public GameObject[] afterUpdate;


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

    // Called when an object is interacted with.
    public virtual Items.Item Interact(Items.Item item = null)
    {
        Items.Item toReturn = null;

        World.textbox.Write("It's a thingy.");
        
        return toReturn;
    }

    protected void TurnInto(GameObject becomes)
    {
        Object newObject = becomes.GetComponent<Object>();
        SpriteRenderer thisSprite = GetComponent<SpriteRenderer>();
        SpriteRenderer newSprite = becomes.GetComponent<SpriteRenderer>();

        thisSprite.sprite = newSprite.sprite;
        tags = newObject.tags;
        afterInteraction = newObject.afterInteraction;
        afterUpdate = newObject.afterUpdate;
    }
}