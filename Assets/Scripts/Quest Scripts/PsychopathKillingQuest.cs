using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PsychopathKillingQuest : Quest 
{
	public PsychopathKillingQuest()
	{
		inUse = false;
		waitPeriod = 0;
		speechCounter = 0;
	}

	public override bool personCheck (NPC person)
	{
		return (person.states.Contains (State.psychotic)
			&& !person.states.Contains(State.dead));
	}

	public override string interact (Items.Item item)
	{
		string forReturn = "";
		if (item != null && item.tags.Contains ("WEAPON")) {
			forReturn = "Oh! So you're killing me!" + "\n" +"Wonderful! You're one step closer" + 
				"\n" + "to going crazy too!";
			questGiver.SetState (State.dead);
			World.AddChaos (World.QUEST_COMPLETE);
			questGiver.hasQuest = false;
			questGiver.mission = null;
			questGiver.quest.SetActive(false);
			inUse = false;
			waitPeriod = 10;
		}
		else if (speechCounter == 0) {
			forReturn = "Hahahaha! Killing is so much" + "\n" + "FUN isn't it?! Why don't you try?";
		} else if (speechCounter == 1) {
			forReturn = "Okay! Go do it! Kill anyone!!" + "\n" + "You could even kill me!!";
		} else{
			forReturn = "Hurry up and KILL SOMEONE!!";
		}
		speechCounter++;
		return forReturn;
	}
}