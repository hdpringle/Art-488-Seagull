using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class MapSelect : MenuController
{
	// map - see below
	private int map;
	private Text titleText, capText, descText;

	private string[] maps = { "BigIsland", "StiltIsland", "CampIsland", "Cove" };
	private int[] capacities = { 4, 4, 4, 2 };
	private string[] descriptions = {
		"A large, polished island with lots of pickups.",
		"A smaller island with an extended pier system inspired by floating villages.",
		"A peaceful gettaway featuring a campsite.",
		"Our pre-alpha testing map, included as a monument to how far we have come."
	};

	private class GSI
	{
		public float updown, leftright;
	}
	private GSI inputs;

	private class MapRelations
	{
		public string name;
		public int capacity;
		public string description;
		public GameObject highlight;
		public Toggle toggle;
	}
	Dictionary<int, MapRelations> relations;

	// Use this for initialization
	void Start()
	{
		map = 0;
		inputs = new GSI();

		titleText = GameObject.Find("Title").GetComponent<Text>();
		capText = GameObject.Find("Capacity").GetComponent<Text>();
		descText = GameObject.Find("Description").GetComponent<Text>();

		relations = new Dictionary<int, MapRelations>();
		for (int i = 0; i < maps.Length; i++)
		{
			MapRelations relation = new MapRelations();
			relation.name = maps[i];
			relation.capacity = capacities[i];
			relation.description = descriptions[i];
			relation.highlight = GameObject.Find(relation.name + "Highlight");
			relation.toggle = GameObject.Find(relation.name + "Toggle").GetComponent<Toggle>();
			relations[i] = relation;
		}

		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioSource> ().volume = settings.musicVolume;
		UpdateMap();
		InvokeRepeating("BestUpdate", 0f, 0.25f);
	}

	// Update is called once per frame
	void Update()
	{
		GetControls();

		if (Input.GetButtonDown("Submit"))
		{
			ChangeScene("PlayerSelect");
		}

		if (Input.GetButtonDown("Cancel"))
		{
			ChangeScene("MainMenu");
		}
	}

	void BestUpdate()
	{
		if (inputs.leftright != 0)
		{
			map = Mathf.Clamp(map + (inputs.leftright > 0 ? 1 : -1), 0, maps.Length - 1);
			UpdateMap();
		}
	}

	void UpdateMap()
	{
		foreach (KeyValuePair<int, MapRelations> kvp in relations)
		{
			if (kvp.Key == map)
			{
				settings.mapChosen = kvp.Value.name;
				kvp.Value.highlight.SetActive(true);
				kvp.Value.toggle.isOn = true;

				titleText.text = kvp.Value.name;
				capText.text = "Max Players: " + kvp.Value.capacity;
				descText.text = kvp.Value.description;
			}
			else
			{
				kvp.Value.highlight.SetActive(false);
				kvp.Value.toggle.isOn = false;
			}
		}
	}

	protected virtual void GetControls()
	{
		inputs.updown = -Input.GetAxis("Forward");
		inputs.leftright = Input.GetAxis("Sideways");
	}
}
