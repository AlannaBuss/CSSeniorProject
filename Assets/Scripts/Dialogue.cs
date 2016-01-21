using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;


public class Dialogue : MonoBehaviour
{
    // Holds the different kinds of dialogues different kinds of personalities have
    public static Dictionary<String, Dictionary<String, String>> dialogue;

    // Initializes dialogue from text files
    public static void Start()
    {
        string[] files = Directory.GetFiles("dialogue");
        dialogue = new Dictionary<String, Dictionary<String, String>>();

        foreach (string file in files)
        {
            string[] lines = File.ReadAllLines(file);
            string personality = "";

            foreach (string line in lines)
            {
                string[] l = line.Split(' ');

                try
                {
                    if (l[0] == "~")
                    {
                        personality = l[1];
                        dialogue.Add(l[1], new Dictionary<String, String>());
                    }
                    else
                        dialogue[personality].Add(l[0], l[2]);
                }
                catch (NullReferenceException)
                {
                    print("Error with item " + file + " property " + l[0] + " is invalid");
                }
            }
        }
    }

    public static string getDialogue(string personality, string type)
    {
        return (dialogue[personality])[type];
    }
}
