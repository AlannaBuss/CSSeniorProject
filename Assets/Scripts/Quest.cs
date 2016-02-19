using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Quest : MonoBehaviour
{
	protected Boolean inUse;
	protected int speechCounter;
	//We want to wait some amount of quests before this one is used again
	protected int waitPeriod;
	//How many rotations do we want to wait?
	protected static int k_numRotations = 20000;
	protected NPC questGiver;
	protected NPC questGiver2;

	public Quest ()
	{
		inUse = false;
		waitPeriod = 0;
		speechCounter = 0;
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
		return !person.hasQuest;
	}
		

	//Returns true if this person matches what the quest needs for the second person and false if not
	public Boolean secondPersonCheck(NPC person)
	{
		return false;
	}


	//When the quest gets given out it takes in the NPC it is being given to
	public void startQuest (NPC person, NPC person2)
	{
		inUse = true;
		questGiver = person;
		questGiver2 = person2;
		speechCounter = 0;
	}
		

	//What happens when an NPC is interacted with when they have a quest
	virtual public string interact()
	{
		string forReturn;
		if (speechCounter == 0) {
			forReturn = "Hi, could you just talk to me for" + "\n" +  "awhile?";
		} else if (speechCounter == 1) {
			forReturn = "I guess the world is just getting" + "\n" + "so...chaotic.";
		} else if (speechCounter == 2) {
			forReturn = "And I guess I'm feeling like I'm" + "\n" +  "not important.";
		}else if (speechCounter == 3) {
			forReturn = "But...I guess I have to go" + "\n" + "and make myself important!";
		} else {
			World.AddChaos (World.QUEST_COMPLETE);
			questGiver.hasQuest = false;
			questGiver.mission = null;
			questGiver.quest.active = false;
			inUse = false;
			waitPeriod = k_numRotations;
			forReturn = "Thanks for listening. You've really" + "\n" + "helped.";
		}
		speechCounter++;
		return forReturn;
		
	}
}


