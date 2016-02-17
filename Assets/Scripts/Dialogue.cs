using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;


public class Dialogue : MonoBehaviour
{
    public class DialogueTree
    {
        private Dictionary<String, List<String>> dialogue;

        public DialogueTree() {
            dialogue = new Dictionary<String, List<String>>();
        }

        public void addResponse(String responseType, String response) {
            if (!dialogue.ContainsKey(responseType))
                dialogue.Add(responseType, new List<String>());
            dialogue[responseType].Add(response);
        }

        public List<String> getResponses(String responseType) {
            return dialogue[responseType];
        }

        public String getRandomResponse(String responseType) {
            List<String> responses = getResponses(responseType);
            if (responses == null || responses.Count == 0)
                return "";
            return responses[Random.Range(0, responses.Count)];
        }
    }

    // Holds the different kinds of dialogues different kinds of personalities have
    public static Dictionary<String, DialogueTree> dialogueTree;

    // Initializes dialogue from text files
    public static void Start()
    {
        string[] files = Directory.GetFiles("dialogue");
        dialogueTree = new Dictionary<String, DialogueTree>();

        foreach (string file in files) {
            string[] lines = File.ReadAllLines(file);
            string personality = "";

            foreach (string line in lines) {
                string[] l = line.Split(' ');

                try {
                    if (l[0] == "~") {
                        personality = l[1];
                        dialogueTree.Add(l[1], new DialogueTree());
                    }
                    else
                        dialogueTree[personality].addResponse(l[0], l[2].Replace('_', ' '));
                }
                catch (NullReferenceException) {
                    print("Error with item " + file + " property " + l[0] + " is invalid");
                }
            }
        }
    }

    public static string getDialogue(string personality, string type)
    {
        return (dialogueTree[personality]).getRandomResponse(type);
    }
}