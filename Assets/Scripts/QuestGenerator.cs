using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class QuestGenerator : MonoBehaviour
{
	private Quest[] questSet = new Quest[1];
	private MapManager map;
	private int numQuests;
	private const int k_maxQuests = 2;

	public QuestGenerator (MapManager mapm)
	{
		questSet[0] = new Quest();
		map = mapm;
	}

	//Tries to generate a quest starting on a given tile.
	public Boolean generateQuest(int mapX, int mapY)
	{
		int questNum = Random.Range (0, questSet.Length); 
		if (questSet [questNum].inUse) {
			return false;
		}
		else 
		{
			return true;
		}
	}

	// Called every game loop
	void Update()
	{
        print("UPDATING YEA");

		int count = 0;
		numQuests = 0;

		while (count <= questSet.Length)
		{
			if(questSet[count++].inUse)
			{
				numQuests++;
			}
		}

		if(numQuests < k_maxQuests)
		{
			if(generateQuest(map.player.mapX, map.player.mapY))
			{
					numQuests++;
			}
		}
	}
}


