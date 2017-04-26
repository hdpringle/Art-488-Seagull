using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class GameSelect : MenuController {

	private const int NUMFIELDS = 4;
	
	// map - see below
	// field:
	//		0 - map select
	//		1 - player count select
	private int map, field;
	private Slider players, playTime, winScore;

	private string[] maps = { "BigIsland" };
	private GameObject[] fieldHighlights;

	private class GSI
	{
		public float updown, leftright;
	}
	private GSI inputs;

	private class MapRelations
	{
		public string name;
		public GameObject highlight;
		public Toggle toggle;
	}
	Dictionary<int, MapRelations> relations;

	// Use this for initialization
	void Start ()
	{
		map = 0;
		field = 0;
		players = GameObject.Find ("PlayerSlider").GetComponent<Slider> ();
		playTime = GameObject.Find ("TimeSlider").GetComponent<Slider> ();
		winScore = GameObject.Find ("ScoreSlider").GetComponent<Slider> ();
		inputs = new GSI ();

		relations = new Dictionary<int, MapRelations> ();
		for (int i = 0; i < maps.Length; i++)
		{
			MapRelations relation = new MapRelations ();
			relation.name = maps [i];
			relation.highlight = GameObject.Find (relation.name + "Highlight");
			relation.toggle = GameObject.Find (relation.name + "Toggle").GetComponent<Toggle> ();
			relations [i] = relation;
		}

		fieldHighlights = new GameObject[NUMFIELDS];
		for (int i = 0; i < NUMFIELDS; i++)
		{
			fieldHighlights [i] = GameObject.Find ("Panel" + i);
		}

		UpdateField ();
		UpdateMap ();
		InvokeRepeating("BestUpdate", 0f, 0.25f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetControls ();

		if (Input.GetButtonDown ("Submit"))
		{
			ChangeScene ("PlayerSelect");
		}

		if (Input.GetButtonDown ("Cancel"))
		{
			ChangeScene ("MainMenu");
		}
	}

	void BestUpdate()
	{
		if (inputs.updown != 0)
		{
			if (inputs.updown > 0)
			{
				field++;
			}
			else
			{
				field--;
			}
			field = Mathf.Clamp (field, 0, NUMFIELDS - 1);
			UpdateField ();
		}

		if (inputs.leftright != 0)
		{
			switch (field)
			{
			case 0:
				map = Mathf.Clamp (map + (inputs.leftright > 0 ? 1 : -1), 0, maps.Length - 1);
				UpdateMap ();
				break;
			case 1:
				players.value = Mathf.Clamp (players.value + (inputs.leftright > 0 ? 1 : -1), players.minValue, players.maxValue);
				settings.numPlayers = (int) players.value;
				break;
			case 2:
				playTime.value = Mathf.Clamp (playTime.value + (inputs.leftright > 0 ? 1 : -1), playTime.minValue, playTime.maxValue);
				settings.matchLengthSeconds = (int) playTime.value * 60;
				break;
			case 3:
				winScore.value = Mathf.Clamp (winScore.value + (inputs.leftright > 0 ? 1 : -1), winScore.minValue, winScore.maxValue);
				settings.autoWinScore = (int) winScore.value * 10;
				break;
			}
		}
	}

	void UpdateField()
	{
		foreach (GameObject p in fieldHighlights)
		{
			p.SetActive (p.name.Equals ("Panel" + field));
		}
	}

	void UpdateMap()
	{
		foreach (KeyValuePair<int,MapRelations> kvp in relations)
		{
			if (kvp.Key == map)
			{
				settings.mapChosen = kvp.Value.name;
				kvp.Value.highlight.SetActive (true);
				kvp.Value.toggle.isOn = true;
			}
			else
			{
				kvp.Value.highlight.SetActive (false);
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
