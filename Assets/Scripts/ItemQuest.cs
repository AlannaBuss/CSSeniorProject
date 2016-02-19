using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ItemQuest : Quest
{
	//TODO Make the number and type of this array correct.
	Items[] objects = new Items[8];
	
	public ItemQuest ()
	{
		inUse = false;
		waitPeriod = 0;
		speechCounter = 0;
		//TODO Make it so that all the types of object are in the array
	}

	//What happens to this NPC when it is interacted with.
	public override string interact ()
	{
		string forReturn = "";
		if (speechCounter == 0) {
			forReturn = "Hey! Would you mind getting me an item?";
		}
		speechCounter++;
		return forReturn;
	}
}


