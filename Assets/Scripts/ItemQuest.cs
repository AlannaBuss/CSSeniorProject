using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class ItemQuest : Quest
{
	//TODO Make the number and type of this array correct.
	Items.Item[] objects = new Items.Item[8];
	//TODO type of this needs correction to :)
	Items.Item questItem;
	string vowels = "a e i o u";

	
	public ItemQuest ()
	{
		inUse = false;
		waitPeriod = 0;
		speechCounter = 0;
		//TODO Make it so that all the types of object are in the array
	}

	//What happens to this NPC when it is interacted with.
	override public string interact ()
	{
		string forReturn = "";
		if (speechCounter == 0) {
			forReturn = "Hey! Would you mind getting me an" + "\n" + "item?";
		} else if (speechCounter == 1) {
			//TODO Put the right number in for the range.
			questItem = objects [Random.Range (0, 8)];

			//TODO Place questItem on a tile in map. Save the location string
			string location = "";

			string correctArticle = "a";

			if (vowels.Contains (questItem.name.ToCharArray () [0].ToString ())) {
				correctArticle = "an";
			}
			forReturn = "Thanks! Can you get me " + correctArticle + "\n" +
				questItem.name + " from " + location + "?";
		} else {
			if (World.player.inventory.Contains(questItem)) {
				forReturn = "Thanks for getting me that " +  questItem.name + "!" + 
					"\n" + "It helped me a lot!";
				World.AddChaos (World.QUEST_COMPLETE);
				questGiver.hasQuest = false;
				questGiver.mission = null;
				questGiver.quest.active = false;
				inUse = false;
				waitPeriod = k_numRotations;
			} else {
				forReturn = "Hey did you get me that "  + questItem.name +"?";
			}
		}
		speechCounter++;
		return forReturn;
	}
}


