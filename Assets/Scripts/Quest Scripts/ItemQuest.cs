﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;


public class ItemQuest : Quest
{
    // Prefabs
    private GameObject questPrefab;

    private Items.Item ranItem;
    private QuestItem questItem;
    private string vowels = "a e i o u";

	
	public ItemQuest(GameObject questprefab)
	{
        this.questPrefab = questprefab;

		inUse = false;
		waitPeriod = 0;
		speechCounter = 0;
	}

	//What happens to this NPC when it is interacted with.
	public override string interact (Items.Item item)
	{
		string forReturn = "";

		if (speechCounter == 0) {
			forReturn = "Hey! Would you mind getting me an" + "\n" + "item that I lost?";
		}
        else if (speechCounter == 1) {

            // Decide which item we need to get
            ranItem = Items.getRandomItem();
            questPrefab = Instantiate(questPrefab) as GameObject;
            questItem = questPrefab.GetComponent<QuestItem>();
            questItem.Setup(ranItem);
            questItem.PlaceRandomly();

			int itemX = World.player.mapX - questItem.mapX;
			int itemY = World.player.mapY - questItem.mapY;
			string locationX = "";
			string locationY = "";
			if (itemX > 0) {
				locationX = " " + itemX + " blocks west.";
			} else {
				locationX = " " + (itemX * -1) + " blocks east.";
			}
			if (itemY > 0) {
				locationY = " " + itemY + " blocks south ";
			} else {
				locationY = " " + (itemY * -1) + " blocks north ";
			}
				
			forReturn = "Thanks! I think I left it " + "\n" + locationY + 
				"and" + locationX + ".";
		}
        else {
			if (World.player.inventory.Contains(ranItem)) {
				forReturn = "Thanks for getting me that " +  ranItem.name + "!" + 
					"\n" + "It helped me a lot!";
				World.AddChaos (World.QUEST_COMPLETE);
				questGiver.inventory.Add (ranItem, 1);
				deactivateQuest (questGiver);
				questGiver.SetState (State.happy);
				inUse = false;
				waitPeriod = k_numRotations;

                // Remove the item from the player's inventory
                int itemIndex = World.player.inventory.IndexOf(ranItem);
                World.player.inventory.RemoveAt(itemIndex);
			}
            else {
				forReturn = "Hey did you get me that "  + ranItem.name +"?";
			}
		}
		speechCounter++;
		return forReturn;
	}
}