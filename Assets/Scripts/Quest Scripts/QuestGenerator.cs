using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Collections;
using System.Linq;

public class QuestGenerator : MonoBehaviour
{
    // Prefabs
    public GameObject itemquest;

	private Quest[] questSet = new Quest[5];
	private MapManager map;
	private int numQuests;
	private const int k_maxQuests = 2;
	private const int k_pause = 500;
	private Boolean mapSet;
	private int numPsychoKilled = 0;
	private Boolean finalQuestStarted;
	private PsychopathKillingQuest psychoKilling;
	private PsychopathegeneQuest finalQuest;

	public void Setup()
	{
		questSet [0] = new Quest();
        questSet [1] = new ItemQuest(itemquest);
		questSet [2] = psychoKilling = new PsychopathKillingQuest ();
		questSet [3] = new ItemDeliveryQuest ();
		questSet [4] = new SlaughterQuest ();
		finalQuest = new PsychopathegeneQuest (itemquest);
		mapSet = false;
		finalQuestStarted = false;
	}

	public void setMap(MapManager map2)
	{
		map = map2;
		mapSet = true;
	}

	//Tries to generate a quest starting on a given tile.
	public Boolean generateQuest(int mapX, int mapY)
	{
		if (!finalQuestStarted) {
			
			int questNum = Random.Range (0, questSet.Length + 1); 
			Quest ranQuest = questSet [questNum];
			if (questNum == 5) {
				if (World.GetChaos () <= 10 && psychoKilling.numKilled >= 6) {
					finalQuest.start ();
				}
			} else if (ranQuest.canBeGivenOut ()) {
				List<GameObject> npcs = map.map [mapX] [mapY].npcs;
				int count;
				for (count = 0; count < npcs.Count; count++) {
					NPC ranPerson = npcs.ElementAt (count).GetComponent<NPC> ();
					if (ranQuest.personCheck (ranPerson)) {
						if (ranQuest.numPerson () == 2) {
							int changeX = Random.Range (-1, 2);
							int changeY = Random.Range (-1, 2);
							List<GameObject> npcs2 = map.map [mapX + changeX] [mapY + changeY].npcs;
							int count2;
							ranPerson.hasQuest = true;
							for (count2 = 0; count2 < npcs2.Count; count2++) {
								NPC ranPerson2 = npcs.ElementAt (count2).GetComponent<NPC> ();
								if (ranQuest.secondPersonCheck (ranPerson2)) {
									print ("Quest number " + questNum + " given out");
									ranQuest.startQuest (ranPerson, ranPerson2);

									ranPerson2.hasQuest = true;
									ranPerson.mission = ranQuest;
									ranPerson2.mission = ranQuest;
									ranPerson.draw ();
									ranPerson2.draw ();
									ranPerson.initQuest (ranQuest);
									ranPerson.draw ();
									return true;
								}
							}
							ranPerson.hasQuest = false;
						} else {
							print ("Quest number " + questNum + " given out");
							ranQuest.startQuest (ranPerson, null);
							ranPerson.hasQuest = true;
							ranPerson.mission = ranQuest;
							ranPerson.draw ();
							ranPerson.initQuest (ranQuest);
							ranPerson.draw ();
							return true;
						}
					}	
				}

			}
		} else {
			if (finalQuest.checkDone ()) {
				World.textbox.Write ("Congratulations! You defeated the Psychopathegen!");
				//TODO MICHAEL put game end stuff here.
			}
		}
		return false;
		
	}

	//A Coroutine so that update only happens every 5 seconds.
	IEnumerator SlowDown() {

		yield return new WaitForSeconds(k_pause);

	}


	// Called every game loop
	void Update()
	{
		StartCoroutine (SlowDown());

		int count = 0;
		numQuests = 0;

		while (count < questSet.Length)
		{
			if(questSet[count].questInUse())
			{
				numQuests++;
			}
			count++;
		}

		if(numQuests < k_maxQuests)
		{
            if (mapSet && generateQuest(World.player.mapX, World.player.mapY))
			{
					numQuests++;
			}
		}
	}
}


