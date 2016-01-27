using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Message
{
    public List<string> text;
    public npcSprite sprite;

    public Message(List<string> t, npcSprite s)
    {
        text = t;
        sprite = s;
    }

    public Message(string t, npcSprite s)
    {
        text = new List<string>();
        text.Add(t);
        sprite = s;
    }
}

public class Textbox : MonoBehaviour
{
    public GameObject textbox;      // Sprite for the textbox
    private GameObject container;   // Placeholder for containing the text
    private TextMesh mesh;          // Text on screen
    private npcSprite sprite;       // Sprite of speaking npc
    private List<string> text = new List<string>();    // Actual text
    private List<Message> queue = new List<Message>(); // Queue of text messages

    // Text scrolling stuff
    private float time;             // Time created in seconds
    private float timeLoc;          // Current time in seconds
    private float textSpeed = 1;    // Scroll and stagger speed of text
    private int textScroll = 0;     // Current location of text

    // Update is called once per frame
    void Update()
    {
        // Scroll the text
        if (text.Count > 3 && text.Count - textScroll > 3 && Time.time - timeLoc > textSpeed)
        {
            string temp = text[0];
            text.RemoveAt(0);
            text.Add(temp);
            WriteAll(text, sprite);

            timeLoc = Time.time;
            textScroll++;
        }
        // Show the next text message if there is one in queue
        else if (Time.time - timeLoc > textSpeed && queue.Count != 0)
        {
            WriteAll(queue[0].text, queue[0].sprite);
            queue.RemoveAt(0);
        }
        // Remove the text
        if (Time.time - timeLoc > 5)
        {
            text.Clear();
            mesh.text = "";
        }

        RedrawNPC();
    }

    // Draws the textbox to the screen (initial setup)
    public void Draw()
    {
        // Draw the textbox image
        textbox = Instantiate(textbox, new Vector3(4.5f, 0.5f, 1f), Quaternion.identity) as GameObject;
        textbox.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

        // Create an empty game object to draw text on
        container = new GameObject();
        container = Instantiate(container, new Vector3(1.5f, 1.3f, -1f), Quaternion.identity) as GameObject;
        container.AddComponent<TextMesh>();
        Vector3 oldScale = container.transform.localScale;
        Vector3 newScale = new Vector3(oldScale.x * .25f, oldScale.y * .25f, oldScale.z * .25f);
        container.transform.localScale = newScale;

        // Set up the text mesh renderer
        mesh = container.GetComponent<TextMesh>();
        mesh.alignment = TextAlignment.Left;
        mesh.color = Color.black;
        mesh.fontSize = 20;

        // Set up time stuff for scrolling
        time = Time.time;
        timeLoc = time;

        // First message
        text.Add("Welcome!");
        text.Add("Use 'z' to interact");
        text.Add("Use 'h' to hide and display text");
        text.Add("Use the arrow keys to move");
        WriteAll(text, null);
    }

    // Writes the given text to the screen
    public void Write(string t, npcSprite npc)
    {
        if (Time.time - timeLoc < textSpeed)
        {
            queue.Add(new Message(t, npc));
            return;
        }

        if (sprite != null)
            sprite.undrawZoom();
        if (npc == null)
            sprite = null;
        else
        {
            sprite = npc;
            sprite.drawZoom();
        }

        text.Clear();
        text.Add(t);
        textScroll = 0;
        timeLoc = Time.time;
        mesh.text = t;
    }

    // Writes the given text strings to the screen
    public void WriteAll(List<string> t, npcSprite npc)
    {
        if (Time.time - timeLoc < textSpeed)
        {
            queue.Add(new Message(t, npc));
            return;
        }

        if (sprite != null)
            sprite.undrawZoom();
        if (npc == null)
            sprite = null;
        else
        {
            sprite = npc;
            sprite.drawZoom();
        }

        mesh.text = "";
        text = t;
        textScroll = 0;
        timeLoc = Time.time;

        // Write out all of the text
        for (int i = 0; i < t.Count; i++)
            mesh.text += t[i] + "\n";
    }

    // Hides or shows the textbox
    public void Hide()
    {
        SpriteRenderer renderer = textbox.GetComponent<SpriteRenderer>();

        // Show textbox
        if (renderer.color.a == 0f)
        {
            textbox.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            if (sprite != null)
                sprite.drawZoom();
            container.SetActive(true);
        }
        // Hide textbox
        else if (renderer.color.a == 0.5f)
        {
            textbox.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            if (sprite != null)
                sprite.undrawZoom();
            container.SetActive(false);
        }
        // Make textbox transparant
        else if (renderer.color.a == 1f)
        {
            textbox.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    // Redraws the NPC zoom in face
    public void RedrawNPC()
    {
        if (sprite != null && mesh.text != "")
        {
            sprite.undrawZoom();
            sprite.drawZoom();
        }
    }
}