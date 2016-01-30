using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Collections;

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
		if (questSet [questNum].canBeGivenOut()) {
			return false;
		}
		else 
		{
			return true;
		}
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
			if(mapSet && generateQuest(map.player.mapX, map.player.mapY))
			{
					numQuests++;
			}
		}
	}
}


