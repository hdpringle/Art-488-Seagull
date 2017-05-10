using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class GameSelect : MenuController {

	private const int NUMFIELDS = 3;
	
	// map - see below
	// field:
	//		0 - map select
	//		1 - player count select
	private int field;
	private Slider players, playTime, winScore;
	private Text numText, timeText, scoreText;

	private GameObject[] fieldHighlights;

	private class GSI
	{
		public float updown, leftright;
	}
	private GSI inputs;

	// Use this for initialization
	void Start ()
	{
		field = 0;
		inputs = new GSI ();

		players = GameObject.Find ("PlayerSlider").GetComponent<Slider> ();
		playTime = GameObject.Find ("TimeSlider").GetComponent<Slider> ();
		winScore = GameObject.Find ("ScoreSlider").GetComponent<Slider> ();

		numText = GameObject.Find ("NumPlayerText").GetComponent<Text> ();
		timeText = GameObject.Find ("TimeText").GetComponent<Text> ();
		scoreText = GameObject.Find ("ScoreText").GetComponent<Text> ();

		players.onValueChanged.AddListener (PlayerNumChange);
		playTime.onValueChanged.AddListener (TimeChange);
		winScore.onValueChanged.AddListener (ScoreChange);

		players.value = settings.numPlayers;
		playTime.value = settings.matchLengthSeconds / 60;
		winScore.value = settings.autoWinScore / 10;
		
		fieldHighlights = new GameObject[NUMFIELDS];
		for (int i = 0; i < NUMFIELDS; i++)
		{
			fieldHighlights [i] = GameObject.Find ("Panel" + i);
		}

		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioSource> ().volume = settings.musicVolume;
		UpdateField ();
		InvokeRepeating("BestUpdate", 0f, 0.25f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetControls ();

		if (Input.GetButtonDown ("Submit"))
		{
			ChangeScene ("MapSetup");
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
				players.value = Mathf.Clamp (players.value + (inputs.leftright > 0 ? 1 : -1), players.minValue, players.maxValue);
				break;
			case 1:
				playTime.value = Mathf.Clamp (playTime.value + (inputs.leftright > 0 ? 1 : -1), playTime.minValue, playTime.maxValue);
				break;
			case 2:
				winScore.value = Mathf.Clamp (winScore.value + (inputs.leftright > 0 ? 1 : -1), winScore.minValue, winScore.maxValue);
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

	protected virtual void GetControls()
	{
		inputs.updown = -Input.GetAxis("PauseUp");
		inputs.leftright = Input.GetAxis("Sideways");
	}

	private void PlayerNumChange (float val)
	{
		settings.numPlayers = (int) players.value;
		numText.text = settings.numPlayers.ToString ();
	}

	private void TimeChange (float val)
	{
		settings.matchLengthSeconds = (int) playTime.value * 60;
		timeText.text = ((int) val).ToString ();
	}

	private void ScoreChange (float val)
	{
		settings.autoWinScore = winScore.value > 10 ? 9001 : (int) winScore.value * 10;
		scoreText.text = settings.autoWinScore > 9000 ? "No Limit" : settings.autoWinScore.ToString ();
	}
}
