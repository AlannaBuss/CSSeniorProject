using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;


public class ItemDeliveryQuest : Quest
{
	Items.Item randomItem;

	public ItemDeliveryQuest ()
	{
		inUse = false;
		waitPeriod = 0;
		speechCounter = 0;
		randomItem = Items.getRandomItem ();
	}

	public override int numPerson ()
	{
		return 2;
	}

	public override bool secondPersonCheck (NPC person)
	{
		return personCheck (person);
	}

	public override string interact (Items.Item item)
	{
		String forReturn = "";

		if (speechCounter == 0) {
			forReturn = "Hey, I'm really busy would" + "\n" + "you deliver this " + 
				"\n" + randomItem.tags + " for me?";
		} else if (speechCounter == 1 && World.player.inventory.Count < 9) {
			forReturn = "Okay thanks, I think they should be near by.";
			World.player.inventory.Add (randomItem);
			deactivateQuest (questGiver);
			questGiver.SetState (State.happy);
			questGiver2.initQuest (this);
			questGiver2.draw ();
		} else if (speechCounter == 1) {
			forReturn = "Oh, it seems like your hands are full." + "\n" + "Come back when you have some room.";
			speechCounter -= 1;
		} else {
			forReturn = "Oh thanks so much. I really needed this!";
			World.AddChaos (World.QUEST_COMPLETE);
			deactivateQuest (questGiver2);
			inUse = false;
			waitPeriod = k_numRotations;
			questGiver2.SetState (State.happy);

			// Remove the item from the player's inventory
			int itemIndex = World.player.inventory.IndexOf(randomItem);
			World.player.inventory.RemoveAt(itemIndex);

			//Give it to the questGiver2
			questGiver2.inventory.Add(randomItem, 1);
		}

		speechCounter++;
		return forReturn;
	}
}


