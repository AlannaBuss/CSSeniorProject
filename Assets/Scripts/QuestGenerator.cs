using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Collections;
using System.Linq;

public class QuestGenerator : MonoBehaviour
{
	private Quest[] questSet = new Quest[1];
	private MapManager map;
	private int numQuests;
	private const int k_maxQuests = 2;
	private const int k_pause = 500;
	private Boolean mapSet;

	public QuestGenerator ()
	{
		questSet[0] = new Quest();
		mapSet = false;
	}

	public void setMap (MapManager map2)
	{
		map = map2;
		mapSet = true;
	}

	//Tries to generate a quest starting on a given tile.
	public Boolean generateQuest(int mapX, int mapY)
	{
		int questNum = Random.Range (0, questSet.Length); 
		Quest ranQuest = questSet [questNum];
		if (ranQuest.canBeGivenOut()) {
			List<GameObject> npcs = map.map[mapX][mapY].npcs;
			int count;
			for (count = 0; count < npcs.Count; count++)
			{
				NPC ranPerson = npcs.ElementAt(count).GetComponent<NPC> ();
				if (ranQuest.personCheck(ranPerson)) {
					print ("Quest given out");
					ranQuest.startQuest (ranPerson);
					ranPerson.hasQuest = true;
					ranPerson.mission = ranQuest;
					ranPerson.draw();
					return true;
				}	
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


