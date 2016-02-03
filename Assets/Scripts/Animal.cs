using UnityEngine;
using System.Collections;

/* TYPES OF TAGS:
 * ANIMAL   
 *  COW         // can be killed (WEAPON) for beef; becomes MILKCOW every evening
 *   MILKCOW    // can be milked once a day for milk
 *  PIG         // can be killed (WEAPON) for pork
 */
public class Animal : Object {
	
	// Update is called once per frame
	void Update () {
	
	}

    // Called when an object is interacted with.
    public override Items.Item Interact(Items.Item with = null)
    {
        Items.Item toReturn = null;


        // ANIMAL has died
        if (tags.Contains("DEAD"))
            World.textbox.Write("It's dead.");
        // ANIMAL is a PIG
        else if (tags.Contains("PIG")) {
            if (with != null && with.tags.Contains("WEAPON")) {
                World.textbox.Write("You butchered your own brother.");
                toReturn = Items.getItemWithName("pork");
                TurnInto(afterInteraction[0]);
                World.AddChaos(World.KILL_ANIMAL);
            }
            else
                World.textbox.Write("Oink!");
        }
        // ANIMAL is a milkable COW
        else if (tags.Contains("MILKCOW")) {
            if (with != null && with.tags.Contains("WEAPON")) {
                World.textbox.Write("You butchered this gentle cow.");
                toReturn = Items.getItemWithName("beef");
                TurnInto(afterInteraction[1]);
                World.AddChaos(World.KILL_ANIMAL);
            }
            else {
                World.textbox.Write("You took some milk.");
                toReturn = Items.getItemWithName("milk");
                TurnInto(afterInteraction[0]);
                World.AddChaos(World.TAKE_ITEM);
            }
        }
        // ANIMAL is a regular COW
        else if (tags.Contains("COW")) {
            if (with != null && with.tags.Contains("WEAPON")) {
                World.textbox.Write("You butchered this gentle cow.");
                toReturn = Items.getItemWithName("beef");
                TurnInto(afterInteraction[0]);
                World.AddChaos(World.KILL_ANIMAL);
            }
            else
                World.textbox.Write("Mooo!");
        }

        return toReturn;
    }
}
