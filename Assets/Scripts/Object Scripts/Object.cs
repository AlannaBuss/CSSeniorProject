using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;


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
    public virtual void PlaceAt(int mX, int mY, int tX, int tY)
    {
        mapX = mX;
        mapY = mY;
        tileX = tX;
        tileY = tY;
        transform.position = new Vector3(tX, tY, 0);

        if (hasQuest)
            quest.GetComponent<Transform>().position = new Vector3(tX, tY + .8f, 0);
    }

	public virtual void initQuest(Quest givenMission)
    {
		mission = givenMission;
        hasQuest = true;
		quest.SetActive(false);
        quest = Instantiate(quest) as GameObject;

        PlaceAt(mapX, mapY, tileX, tileY);
		quest.SetActive(false);

    }

    // Called when an object is interacted with.
    public virtual Items.Item Interact(Items.Item item = null)
    {
        Items.Item toReturn = null;

        World.textbox.Write("It's a thingy.");
        
		if(hasQuest == false) 
		{
			quest.SetActive(false);
		}

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