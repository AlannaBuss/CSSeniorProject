using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class SlaughterQuest : Quest
{
	string animal = "";
	Items.Item meat;
	public SlaughterQuest ()
	{
		inUse = false;
		waitPeriod = 0;
		speechCounter = 0;

	}

	public override bool personCheck (NPC person)
	{
		if (Random.Range (0, 1) == 0) {
			animal = "COW";
			meat = Items.getItemWithName ("beef");
		} else {
			animal = "PIG";
			meat = Items.getItemWithName("pork");
		}

		return (base.personCheck (person) && (World.getItemCount("Farm", animal) > 0));
	}

	public override string interact (Items.Item item)
	{
		string forReturn = "";

		if (speechCounter == 0) {
			forReturn = "Oh...hey yeah I could use some help";
		} else if (speechCounter == 1) {
			forReturn = "I...can't bring myself to kill the" + "\n" + "" +
				"poor little " + animal + " would you be able" + "\n" +
				" to do it for me?";
		} else if (World.player.inventory.Contains (meat)) {
			forReturn = "Thanks...please don't tell anyone...";
			World.AddChaos (World.QUEST_COMPLETE);
			deactivateQuest (questGiver);
			inUse = false;
			waitPeriod = k_numRotations;
			questGiver.SetState (State.happy);
		} else {
			forReturn = "Did you...get the meat yet?";
		}

		speechCounter++;
		return forReturn;
	}
}
