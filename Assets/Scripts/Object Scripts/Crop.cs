using UnityEngine;
using System.Collections;

public class Crop : Object
{

    // Update is called once per frame
    void Update()
    {

    }

    // Called when an object is interacted with.
    public override Items.Item Interact(Items.Item item = null)
    {
        Items.Item toReturn = null;

        if (tags.Contains("CARROT"))
        {
            World.textbox.Write("You got some carrots.");
            toReturn = Items.getItemWithName("carrot");
            TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
            World.AddChaos(World.STEAL);
        }
        else if (tags.Contains("CORN"))
        {
            World.textbox.Write("You got some corn.");
            toReturn = Items.getItemWithName("corn");
            TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
            World.AddChaos(World.STEAL);
        }
        else if (tags.Contains("LETTUCE"))
        {
            World.textbox.Write("You got some lettuce.");
            toReturn = Items.getItemWithName("lettuce");
            TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
            World.AddChaos(World.STEAL);
        }
        else if (tags.Contains("TOMATO"))
        {
            World.textbox.Write("You got some tomato.");
            toReturn = Items.getItemWithName("tomato");
            TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
            World.AddChaos(World.STEAL);
        }

        return toReturn;
    }
}