using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Jobs : MonoBehaviour {

    static List<Job> jobs;
    static int totalWeight = 0;

	// Use this for initialization
	public static void Start () {
        jobs = new List<Job>();
        string[] files = Directory.GetFiles("jobs");
        foreach (string file in files)
        {
            Job job = new Job();
            string[] lines = File.ReadAllLines(file);
            Dictionary<string, string[]> jobFile = new Dictionary<string, string[]>();
            foreach (string line in lines)
            {
                string[] l = line.Split(' ');
                jobFile.Add(l[0], l);
            }
            try
            {
                job.name = jobFile["name"][2];
                job.home_tag = jobFile["home_tag"][2];
                job.work_tag = jobFile["work_tag"][2];
                string[] invrange = jobFile["inventory"][2].Split('-');
                job.inventoryMin = int.Parse(invrange[0]);
                job.inventoryMax = int.Parse(invrange[1]);
                string[] itemWeights = jobFile["item_weights"];
                job.item_weights = new Dictionary<string, int>();
                for (int i = 2; i < itemWeights.Length - 1; i++)
                {
                    string[] line = itemWeights[i].Split('(');
                    job.item_weights.Add(line[0], int.Parse(line[1].Split(')')[0]));
                }
                job.job_weight = int.Parse(jobFile["job_weight"][2]);
                job.wages = int.Parse(jobFile["wages"][2]);
                jobs.Add(job);
                totalWeight += job.job_weight;
            }
            catch (Exception)
            {
                print("error in Job file " + file);
            }
        }
    }

    internal static Items.Item getRandomItem(Job job)
    {
        int itemPoll = Random.Range(0, 100);
        KeyValuePair<string, int> lastItemWeight = new KeyValuePair<string, int>("FOOD", 1);
        foreach(KeyValuePair<string, int> itemWeight in job.item_weights)
        {
            itemPoll -= itemWeight.Value;
            if(itemPoll <= 0)
            {
                return (Items.getRandomItemOfTag(itemWeight.Key));
            }
            lastItemWeight = itemWeight;
        }
        return (Items.getRandomItemOfTag(lastItemWeight.Key));
    }

    public static Job getRandomJob()
    {
        int weight = Random.Range(0, totalWeight);
        foreach(Job job in jobs)
        {
            weight -= job.job_weight;
            if(weight < 0)
            {
                return job;
            }
        }
        return null;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public class Job
    {
        public string name;
        public string home_tag;
        public string work_tag;
        public int inventoryMin;
        public int inventoryMax;
        public Dictionary<string, int> item_weights;
        public int job_weight;
        public int wages;
    }
}
