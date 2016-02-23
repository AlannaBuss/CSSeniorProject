using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;


public class ItemQuest : Quest
{
    // Prefabs
    private GameObject questPrefab;

    private Items.Item item;
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
	public override string interact ()
	{
		string forReturn = "";

		if (speechCounter == 0) {
			forReturn = "Hey! Would you mind getting me an" + "\n" + "item?";
		}
        else if (speechCounter == 1) {

            // Decide which item we need to get
            item = Items.getRandomItem();
            questPrefab = Instantiate(questPrefab) as GameObject;
            questItem = questPrefab.GetComponent<QuestItem>();
            questItem.Setup(item);
            questItem.PlaceRandomly();

            string location = questItem.getLocationName();
			string correctArticle = "a";

            if (vowels.Contains(item.name.ToCharArray()[0].ToString()))
				correctArticle = "an";
			forReturn = "Thanks! Can you get me " + correctArticle + "\n" +
				item.name + " from " + location + "?";
		}
        else {
			if (World.player.inventory.Contains(item)) {
				forReturn = "Thanks for getting me that " +  item.name + "!" + 
					"\n" + "It helped me a lot!";
				World.AddChaos (World.QUEST_COMPLETE);
				questGiver.hasQuest = false;
				questGiver.mission = null;
				questGiver.quest.SetActive(false);
				inUse = false;
				waitPeriod = k_numRotations;

                // Remove the item from the player's inventory
                int itemIndex = World.player.inventory.IndexOf(item);
                World.player.inventory.RemoveAt(itemIndex);
			}
            else {
				forReturn = "Hey did you get me that "  + item.name +"?";
			}
		}
		speechCounter++;
		return forReturn;
	}
}