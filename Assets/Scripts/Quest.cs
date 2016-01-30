using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Quest : MonoBehaviour
{
	private Boolean inUse;
	//We want to wait some amount of quests before this one is used again
	private int waitPeriod;
	//How many rotations do we want to wait?
	private static int k_numRotations = 2;

	public Quest ()
	{
		inUse = false;
		waitPeriod = 0;
	}

	//When the quest is given out it is in use
	public void startQuest()
	{
		inUse = true;
	}

	//Checks if the quest is currently in use for the quest count
	public Boolean questInUse ()
	{
		if(waitPeriod == 0)
		{
			return inUse;
		}
		else
		{
			if(waitPeriod > 0)
			{
				waitPeriod--;
			}
			else
			{
				waitPeriod = 0;
			}

			return false;
		}
	}

	//Checks if the quest can be used as the next quest.
	public Boolean canBeGivenOut ()
	{
		return (!inUse && (waitPeriod == 0));
	}

	//Returns true if the quest is reliant on a person type
	public Boolean needPerson ()
	{
		return true;
	}

	//Returns true if the person matches what the quest needs and false if not
	public Boolean personCheck (NPC person)
	{
		return true;
	}

	//Returns true if the quest needs a building
	public Boolean needBuilding ()
	{
		return false;
	}

	//Returns true if the building matches the criteria that the quest needs
	public Boolean buildingCheck(Building building)
	{
		return false;
	}

	//Currently returns what should be said by someone with this quest
	//TODO Make it so it is multiple speech bubbles and such
	public string questSpeech ()
	{
		return "I need some help!";
	}
		
	//Called when the quest completes. 
	public string finishQuest ()
	{
		inUse = false;
		waitPeriod = k_numRotations;
		return "Thank you!";
	}
}


