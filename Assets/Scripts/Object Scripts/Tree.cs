using UnityEngine;
using System.Collections;

public class Tree : Object {
	
	// Update is called once per frame
	void Update () {
	
	}

    // Called when an object is interacted with.
    public override Items.Item Interact(Items.Item item = null)
    {
        Items.Item toReturn = null;

        if (tags.Contains("APPLETREE"))
        {
            World.textbox.Write("You got some apples.");
            toReturn = Items.getItemWithName("apple");
            TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
            World.AddChaos(World.TAKE_ITEM);
        }
        else if (tags.Contains("ORANGETREE"))
        {
            World.textbox.Write("You got some 'oranges'.");
            toReturn = Items.getItemWithName("orange");
            TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
            World.AddChaos(World.TAKE_ITEM);
        }
        else if (tags.Contains("STUMP"))
        {
            World.textbox.Write("It's a stump.");
        }
        else if (tags.Contains("TREE"))
        {
            if (item != null && item.name == "axe")
            {
                World.textbox.Write("You hacked down this innocent tree.");
                TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
                World.AddChaos(World.DESTROY);
            }
            else
                World.textbox.Write("It's a tree.");
        }

        return toReturn;
    }
}
