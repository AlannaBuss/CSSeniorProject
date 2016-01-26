using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Quest : MonoBehaviour
{
	public Boolean inUse;

	public Quest ()
	{
		inUse = true;
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
		return "Thank you!";
	}
}


