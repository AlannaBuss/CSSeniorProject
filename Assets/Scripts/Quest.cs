using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Quest : MonoBehaviour
{
	private Boolean inUse;
	private Boolean start;
	//We want to wait some amount of quests before this one is used again
	private int waitPeriod;
	//How many rotations do we want to wait?
	private static int k_numRotations = 2;
	private NPC questGiver;

	public Quest ()
	{
		inUse = false;
		waitPeriod = 0;
		start = false;
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
	public int numPerson ()
	{
		return 1;
	}

	//Returns true if the person matches what the quest needs and false if not
	public Boolean personCheck (NPC person)
	{
		return true;
	}


	//When the quest gets given out it takes in the NPC it is being given to
	public void startQuest (NPC person)
	{
		inUse = true;
		questGiver = person;
		start = false;
	}
		

	//What happens when an NPC is interacted with when they have a quest
	public string interact()
	{
		if (!start) 
		{
			start = true;
			return "Hi, this is a tutorial quest! Talk to me again to finish.";
		}
		World.AddChaos(World.QUEST_COMPLETE);
		questGiver.hasQuest = false;
		questGiver.mission = null;
		questGiver.quest.SetActive(false);
		inUse = false;
		waitPeriod = k_numRotations;
		return "Thank you!";
		
	}
}


