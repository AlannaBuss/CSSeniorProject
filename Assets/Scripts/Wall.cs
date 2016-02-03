using UnityEngine;
using System.Collections;

/* TYPES OF TAGS:
 * WALL         
 *  BUSH         // grows into a ROSEBUSH every morning (50% chance)
 *   ROSEBUSH    // gives a rose -> becomes BUSH
 *  ROCKWALL     // grows into an OREWALL every night (10% chance)
 *   OREWALL     // gives ore -> becomes ROCKWALL
 *  BRICKWALL    // becomes VINEWALL once a day
 *   VINEWALL    // becomes BRICKWALL after hit with (WEAPON)
 *  DIRTWALL     
 *  FENCE        
 *   WOODFENCE    
 *   METALFENCE   
 */
public class Wall : Object {
	
	// Update is called once per frame
	void Update () {

	}

    // Handles wall interactions
    public override Items.Item Interact(Items.Item item = null) {
        Items.Item toReturn = null;

        // WALL is a ROSEBUSH
        if (tags.Contains("ROSEBUSH")) {
            World.textbox.Write("You took some roses.");
            toReturn = Items.getItemWithName("rose");
            TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
            World.AddChaos(World.TAKE_ITEM);
        }
        // WALL is a BUSH
        else if (tags.Contains("BUSH"))
            World.textbox.Write("This bush smells nice.");
        // WALL is a VINEWALL
        else if (tags.Contains("VINEWALL")) {
            if (item != null && item.tags.Contains("WEAPON")) {
                World.textbox.Write("You hacked the vines.");
                TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
                World.AddChaos(World.DESTROY);
            } else
                World.textbox.Write("What a lovely " + tags[tags.Count - 1]);
        }
        // WALL is an OREWALL
        else if (tags.Contains("OREWALL")) {
            World.textbox.Write("You took some ore.");
            TurnInto(afterInteraction[Random.Range(0, afterInteraction.Length)]);
            World.AddChaos(World.TAKE_ITEM);
        }
        // WALL is some other WALL
        else if (tags.Contains("WALL"))
            World.textbox.Write("What a lovely " + tags[tags.Count - 1]);

        return toReturn;
    }
}
