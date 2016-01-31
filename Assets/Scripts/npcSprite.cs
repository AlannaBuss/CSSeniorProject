using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class npcSprite : MonoBehaviour
{
    // Defines the different pieces that an NPC's sprite can be constructed of
    public GameObject[] _outline;
    public GameObject[] _hair;
    public GameObject[] _eyes;
    public GameObject[] _skin;
    public GameObject[] _top;
    public GameObject[] _bottom;
    public GameObject[] _shoes;
    public GameObject[] _mouth;  // changes based on state
    public GameObject[] _extra;  // miscellaneous (see "constants for extras")

    // Defines the different pieces of the zoomed in NPC sprite
    public GameObject _zoom_outline;
    public GameObject _zoom_eyes;
    public GameObject _zoom_face;
    public GameObject _zoom_shirt;
    public GameObject[] _zoom_outline_hair;
    public GameObject[] _zoom_hair;
    public GameObject[] _zoom_outline_eyes;
    public GameObject[] _zoom_outline_mouth;

    // Constants
    private int _typesPeople = 5;
    private int _typesMouths = 2;
    private int _typesExtras = 2;
    // Constants for extras
    private int _blood = 0;     // covered in blood: person is injured or psychotic
    private int _hat1 = 1;      // it's a hat: person is wearing it
    private int _hat2 = 2;      // it's a hat: person is wearing it
    private int _hat3 = 3;      // it's a hat: person is wearing it
    private int _glasses = 4;   // a pair of glasses: person needs glasses
    private int _goatee = 5;    // it's a goatee: person has been converted to a follower of the wizard
    // Constants for mouths
    private int _neutral = 0;
    private int _smile = 1;
    private int _frown = 2;
    // Constants for eyes
    private int _normal = 0;
    private int _sad = 1;
    private int _angry = 2;

    // Information about our sprite
    private int spriteType;                                         // Defines what type of sprite this NPC is
    private List<GameObject> pieces = new List<GameObject>();       // Defines the static pieces this NPC is made of
    private List<GameObject> extras = new List<GameObject>();       // Defines the extra pieces this NPC is carrying
    private GameObject mouth, eyes;                                 // Defines the dynamic pieces that this NPC is made of
    // Zoomed in image
    GameObject zoom_mouth, zoom_eyes, zoom_eye_color, zoomHair, zoomHairOutline, zoomSkin, zoomTop, zoomOutline;



    // Returns a random color
    private Color randomColorPicker()
    {
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        return color;
    }

    // Returns a random skintone
    private Color skinColorPicker()
    {
        int skinType = Random.Range(0, 3);

        // Peach
        if (skinType == 0)
            return new Color(1f, .92f, .77f);
        // Tan
        else if (skinType == 1)
            return new Color(.66f, .53f, .36f);
        // Brown
        else
            return new Color(.4f, .29f, .14f);
    }

    // Define how the npc's sprite will look
    private void initSprite()
    {
        // Mouth
        int mouthType = (spriteType * _typesMouths) + _neutral;
        mouth = Instantiate(_mouth[mouthType]) as GameObject;
        zoom_mouth = Instantiate(_zoom_outline_mouth[_neutral], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;

        // Eyes
        Color eyecolor = randomColorPicker();
        eyes = Instantiate(_eyes[spriteType]) as GameObject;
        eyes.GetComponent<SpriteRenderer>().color = eyecolor;
        zoom_eyes = Instantiate(_zoom_outline_eyes[_normal], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        zoom_eye_color = Instantiate(_zoom_eyes, new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        zoom_eye_color.GetComponent<SpriteRenderer>().color = eyecolor;

        // Hair
        Color haircolor = randomColorPicker();
        GameObject npcHair = Instantiate(_hair[spriteType]) as GameObject;
        npcHair.GetComponent<SpriteRenderer>().color = haircolor;
        zoomHair = Instantiate(_zoom_hair[spriteType], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        zoomHair.GetComponent<SpriteRenderer>().color = haircolor;
        zoomHairOutline = Instantiate(_zoom_outline_hair[spriteType], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;

        // Skin
        Color skinColor = skinColorPicker();
        GameObject npcSkin = Instantiate(_skin[spriteType]) as GameObject;
        npcSkin.GetComponent<SpriteRenderer>().color = skinColor;
        zoomSkin = Instantiate(_zoom_face, new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        zoomSkin.GetComponent<SpriteRenderer>().color = skinColor;

        // Top
        Color topColor = randomColorPicker();
        GameObject npcTop = Instantiate(_top[spriteType]) as GameObject;
        npcTop.GetComponent<SpriteRenderer>().color = topColor;
        zoomTop = Instantiate(_zoom_shirt, new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        zoomTop.GetComponent<SpriteRenderer>().color = topColor;
        // Bottom
        GameObject npcBottom = Instantiate(_bottom[spriteType]) as GameObject;
        npcBottom.GetComponent<SpriteRenderer>().color = randomColorPicker();
        // Shoes
        GameObject npcShoes = Instantiate(_shoes[spriteType]) as GameObject;
        npcShoes.GetComponent<SpriteRenderer>().color = randomColorPicker();
        // Extras
        initExtras();

        zoomOutline = Instantiate(_zoom_outline, new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        pieces.Add(Instantiate(_outline[spriteType]));
        pieces.Add(npcHair);
        pieces.Add(npcSkin);
        pieces.Add(npcTop);
        pieces.Add(npcBottom);
        pieces.Add(npcShoes);
    }

    // Define if and what the sprite's extra features will be
    private void initExtras()
    {
        // Hat (30% chance)
        if (Random.Range(0, 10) <= 2)
        {
            int type = (_typesPeople * _hat1) + spriteType;
            GameObject hat = Instantiate(_extra[type], new Vector3(0f, 0f, 10f), Quaternion.identity) as GameObject;
            hat.GetComponent<SpriteRenderer>().color = randomColorPicker();
            extras.Add(hat);
        }
    }


    // Create the initial NPC sprite
    public void init()
    {
        spriteType = Random.Range(0, _typesPeople);
        initSprite();
        undrawZoom();
    }

    // Draws the NPC to the screen
    public void draw()
    {
        mouth.SetActive(true);
        eyes.SetActive(true);
        for (int i = 0; i < extras.Count; i++)
            extras[i].SetActive(true);
        for (int i = 0; i < pieces.Count; i++)
            pieces[i].SetActive(true);
    }

    // Removes the npc from the screen
    public void undraw()
    {
        mouth.SetActive(false);
        eyes.SetActive(false);
        for (int i = 0; i < extras.Count; i++)
            extras[i].SetActive(false);
        for (int i = 0; i < pieces.Count; i++)
            pieces[i].SetActive(false);
    }

    // Draws the zoomed NPC to the screen
    public void drawZoom()
    {
        zoom_eyes.SetActive(true);
        zoom_eye_color.SetActive(true);
        zoom_mouth.SetActive(true);
        zoomHairOutline.SetActive(true);
        zoomHair.SetActive(true);
        zoomOutline.SetActive(true);
        zoomSkin.SetActive(true);
        zoomTop.SetActive(true);
    }

    // Removes the zoomed NPC from the screen
    public void undrawZoom()
    {
        zoomHairOutline.SetActive(false);
        zoomHair.SetActive(false);
        zoomOutline.SetActive(false);
        zoomSkin.SetActive(false);
        zoomTop.SetActive(false);
        zoom_eyes.SetActive(false);
        zoom_eye_color.SetActive(false);
        zoom_mouth.SetActive(false);
    }

    // Places the sprite at the given location
    public void placeAt(Vector3 pos)
    {
        transform.position = pos;
        mouth.transform.position = pos;
        eyes.transform.position = pos;
        for (int i = 0; i < pieces.Count; i++)
            pieces[i].transform.position = pos;
        for (int i = 0; i < extras.Count; i++)
            extras[i].transform.position = pos;
    }

    // Changes the sprite to match a certain state
    public void setState(string state)
    {
        undraw();
        undrawZoom();

        // Set sprite mouth to "neutral"
        if (state.Equals("normal"))
        {
            int mouthType = (spriteType * _typesMouths) + _neutral;
            Destroy(mouth);
            Destroy(zoom_mouth);
            Destroy(zoom_eyes);
            mouth = Instantiate(_mouth[mouthType]) as GameObject;
            zoom_mouth = Instantiate(_zoom_outline_mouth[_neutral], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
            zoom_eyes = Instantiate(_zoom_outline_eyes[_normal], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        }
        // Set sprite mouth to "happy"
        else if (state.Equals("happy"))
        {
            int mouthType = (spriteType * _typesMouths) + _smile;
            Destroy(mouth);
            Destroy(zoom_mouth);
            Destroy(zoom_eyes);
            mouth = Instantiate(_mouth[mouthType]) as GameObject;
            zoom_mouth = Instantiate(_zoom_outline_mouth[_smile], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
            zoom_eyes = Instantiate(_zoom_outline_eyes[_normal], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        }
        // Set sprite mouth to "frown"
        else if (state.Equals("sad") || state.Equals("angry"))
        {
            Destroy(zoom_mouth);
            Destroy(zoom_eyes);
            zoom_mouth = Instantiate(_zoom_outline_mouth[_frown], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
            zoom_eyes = Instantiate(_zoom_outline_eyes[_sad], new Vector3(0.5f, 0.5f, -2f), Quaternion.identity) as GameObject;
        }
        // Set sprite state to "psychotic"
        else if (state.Equals("psychotic"))
        {
            // red eyes
            eyes.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);
            zoom_eye_color.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);
            // smile
            setState("happy");
            // blood
            int bloodType = (spriteType * _blood) + spriteType;
            extras.Add(Instantiate(_extra[bloodType]));
        }

        undraw();
        undrawZoom();
    }
}