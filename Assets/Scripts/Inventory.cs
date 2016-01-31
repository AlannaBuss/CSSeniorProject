using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

    public GameObject inventoryBox;
    private GameObject container;
    private TextMesh mesh;

    // Can only have up to 10 items in inventory
    private List<Items.Item> items = new List<Items.Item>();
    public bool isDrawn;

	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void Setup()
    {
        // Create the inventory image
        inventoryBox = Instantiate(inventoryBox, new Vector3(1.5f, 4.5f, 0f), Quaternion.identity) as GameObject;
        inventoryBox.SetActive(false);
        isDrawn = false;

        // Create an empty game object to draw text on
        container = new GameObject();
        container = Instantiate(container, new Vector3(-0.5f, 9.3f, -1f), Quaternion.identity) as GameObject;
        container.AddComponent<TextMesh>();
        Vector3 oldScale = container.transform.localScale;
        Vector3 newScale = new Vector3(oldScale.x * .25f, oldScale.y * .25f, oldScale.z * .25f);
        container.transform.localScale = newScale;
        container.SetActive(false);

        // Set up the text mesh renderer
        mesh = container.GetComponent<TextMesh>();
        mesh.alignment = TextAlignment.Left;
        mesh.color = Color.black;
        mesh.fontSize = 17;
    }

    public void ShowInventory(List<Items.Item> i, bool shop = false)
    {
        if (isDrawn)
        {
            Undraw();
            if (items != i)
                Draw(i, shop);
        }
        else
            Draw(i, shop);
    }

    // Draws the inventory to the screen
    private void Draw(List<Items.Item> i, bool shop)
    {
        isDrawn = true;
        items = i;
        mesh.text = "";
        inventoryBox.SetActive(true);
        container.SetActive(true);

        for (int n = 0; n < items.Count; n++)
        {
            //items[n].sprite.SetActive(true);
            mesh.text += n + " " + items[n].name;
            if (shop)
                mesh.text += " [" + items[n].value + "G]";
            mesh.text += "\n\n";
        }
    }

    // Removes the inventory from the screen
    private void Undraw()
    {
        isDrawn = false;
        mesh.text = "";
        inventoryBox.SetActive(false);
        container.SetActive(false);

        foreach (Items.Item item in items)
        {
            //item.sprite.SetActive(false);
        }
    }
}