using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Object : MonoBehaviour
{
    public Textbox textbox;         // Used for displaying text to screen
    public int mapX, mapY;          // object's location on the map
    public int tileX, tileY;        // object's location on the tile

    public Boolean hasQuest;	    // Quest flag
    public GameObject quest;		// Object representing a quest

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

    public virtual void initQuest()
    {
        hasQuest = true;
        quest = Instantiate(quest) as GameObject;

        PlaceAt(mapX, mapY, tileX, tileY, 0);
        quest.SetActive(false);
    }

    //Called when an object is interacted with.
    public virtual void Interact()
    {
        print("Interacted with a generic object.");
    }
}