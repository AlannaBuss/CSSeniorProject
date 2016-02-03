using UnityEngine;
using System.Collections;

public class GroundItem : Object {

    Items.Item item;
    Items.Item requiredItem;
    bool requiresItem;

    public GroundItem(Items.Item i)
    {
        tags = new System.Collections.Generic.List<string>();
        tags.Add("ITEM");
        item = i;
        requiresItem = false;
    }

    public GroundItem(Items.Item i, Items.Item required)
    {
        requiresItem = true;
        requiredItem = required;
        tags = new System.Collections.Generic.List<string>();
        tags.Add("ITEM");
        item = i;
    }

	// Use this for initialization
	void Start () {
	    

	}

    public override Items.Item Interact(Items.Item item = null)
    {
        if (requiresItem && item.Equals(this.item) || !requiresItem)
        {
            return item;
        }
        return null;
    }
        
    // Update is called once per frame
    void Update () {
	
	}
}
