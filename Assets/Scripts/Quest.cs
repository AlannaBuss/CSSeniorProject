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
	private NPC questGiver;

	public Quest ()
	{
		inUse = false;
		waitPeriod = 0;
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
	}

	//The interaction method that is called by the object with the quest when
	//it is interacted with.
	public void interact()
	{
		World.textbox.Write("Quest");
	}

	//Currently returns what should be said by someone with this quest
	//TODO Make next two methods into the interact
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


