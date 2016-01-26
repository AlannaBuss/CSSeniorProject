using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class QuestGenerator : MonoBehaviour
{
	private Quest[] questSet = new Quest[1];

	public QuestGenerator ()
	{
		questSet[0] = new Quest();
	}
}


