using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class PsychopathegeneQuest : Quest
{
	private GameObject questPrefab;
	private QuestItem Wilbard;
	private NPC psycho1;
	private NPC psycho2;
	private int numPsycho;
	private String[] hints;

	public PsychopathegeneQuest (GameObject questprefab)
	{
		this.questPrefab = questprefab;

		numPsycho = 0;
		inUse = false;
		waitPeriod = 0;
		speechCounter = 0;

		hints = String[5];
		hints [0] = "If you come close to a true" + "\n" + "psychopath they'll follow you" + 
			"\n" + "around.";
		hints [1] = "True psychopaths don't look" + "\n" + "crazy like other psychopaths.";
		hints [2] = "If you see someone become" + "\n" + "psychotic then the person they were" +
			"\n" + "last near is a psychopath!";
		hints [3] = "I hope my pig Wilbur is" + "\n" + "doing okay in the Wizard Realm.";
		hints [4] = "True pyschopaths will follow" + "\n" + "people close to them.";

		//Find psychos
		GameObject[] npcs = World.map.npcs.ToArray();
		for (int count = 0; count < npcs.Length; count++) {
			NPC temp = npcs [count].GetComponent<NPC>();
			if (temp.personality == Personality.psychotic && !temp.states.Contains (State.psychotic)) {
				if (numPsycho == 0) {
					numPsycho++;
					psycho1 = temp;
				} else {
					numPsycho++;
					psycho2 = temp;
					count = npcs.Length;
				}
			}
		}
	}

	public void start() 
	{
		questPrefab = Instantiate(questPrefab) as GameObject;
		Wilbard = questPrefab.GetComponent<QuestItem>();
		Wilbard.Setup();
		Wilbard.PlaceAt (World.player.mapX, World.player.mapY, World.player.tileX + 1, World.player.tileY);
		Wilbard.mission = this;
		Wilbard.hasQuest = true;
		Wilbard.initQuest (this);
		Wilbard.Draw ();
	}

	public override string interact (Items.Item item)
	{
		string forReturn = "";
		if (speechCounter == 0) {
			forReturn = "Hero! I am Wilbard the Wizard!" + "\n" + "I know how you can" + "\n" +
			"defeat the Psychopathegen!";
		} else if (speechCounter == 1) {
			forReturn = "There are " + numPsycho + " true psychopaths" + "\n" + "who are infecting the " + "\n" +
			"townspeople!";
		} else if (speechCounter == 2) {
			forReturn = "If you kill them both the" + "\n" + "Psychopathegen will die out" + "\n" +
			"and the day will be saved!";
		} else {
			forReturn = hints[Random.Range(0,5)];
		}
		speechCounter++;
		return forReturn;
	}

	public bool checkDone() {
		if (numPsycho == 2)
			return (psycho1.states.Contains(State.dead) && 
				psycho2.states.Contains(State.dead));
		return psycho1.states.Contains(State.dead);
	}
}


