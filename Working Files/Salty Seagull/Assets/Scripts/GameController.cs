﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Boundary
{
	public float xMin = -50, xMax = 50, yMax = 35, zMin = -40, zMax = 40;
}

[System.Serializable]
public class SeagullLimits
{
	public float upAngle = 50f,
			downAngle = 35f,
			accelSpeed = 6f,
			glideDecel = 0.005f,
			antiDrift = 3f,
			rotationLR = 3f,
			rotationUD = 3f,
			maxSpeed = 10f,
			tilt = 35f,
			walkSpeed = 0.125f,
			walkGravity = 9.8f,
			flyGravity = 9.8f,
			itemGravity = 9.8f;
}

public class GameController : MainMenu
{
	public int autowinScore;
	public float timeLimitSeconds, warmupTime;
	public Transform sea;
	public Boundary boundary;
	public SeagullLimits seagullLimits;

	private float currentTime, currentWarmup;
	private int minutes, seconds;
	private int lastMinutes, lastSeconds; //needed to not recheck spawn times
	private bool paused;
	private Dictionary<int, PlayerController> players;
	private Dictionary<int, NEST> nests;

	// Use this for initialization
	void Start ()
	{
		currentTime = timeLimitSeconds;
		currentWarmup = warmupTime;
		Pause (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetAxis ("Pause") != 0 || Input.GetAxis ("Cancel") != 0)
		{
			Pause (true);
		}

		if (!paused)
		{
			//lets the game count down from 5
			if (currentWarmup > 0) {
				currentWarmup -= Time.deltaTime;
			} else {

				//only play while timer ticks down
				if (currentTime > 0) {
					currentTime -= Time.deltaTime;
					minutes = ((int)currentTime) / 60;
					seconds = ((int)currentTime) % 60;

					Text[] timers = GameObject.FindObjectsOfType (typeof(Text)) as Text[];
					foreach (Text timer in timers) {
						if (timer.CompareTag ("timer")) {
							timer.text = minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
						}
					}
					

					//This section works, but we should not use spawning until the world map is full of stuff
					/*
					//Spawn Any pick ups that use this time
					if (!(minutes == lastMinutes && seconds == lastSeconds))
					{
						SpawnPickups(minutes, seconds);
					}
					*/
					lastMinutes = minutes;
					lastSeconds = seconds;
				}
			}
		}
	}

	/*
	 * Will check to see if the spawn time of a pickup prefab
	 * matches the current time. Then will spawn the item in one of 
	 * the given spawn locations
	 */ 
	public void SpawnPickups(int mins, int secs)
	{
		//I found code here that helped me with this: http://answers.unity3d.com/questions/35509/how-can-i-select-all-prefabs-that-contain-a-certai.html
		var allPrefabs = Resources.LoadAll<UnityEngine.Object>("Prefabs/Pickups/");
		foreach (var obj in allPrefabs)
		{
			GameObject gObj = obj as GameObject;

			//look at all of the prefab's pickups script
			if(gObj.GetComponent<Pickups>() != null)
			{
				//check if the spawn time of the prefab matches the current time, then if so spawn one
				if (gObj.GetComponent<Pickups>().spawnTime == (mins*60+secs))
				{
					GameObject inGame = Instantiate(gObj);

					//make this part random later!
					//or we can have all of the instanses spawn in at once, if we use a variable to mark how many
					//of the object needs to be spawned as part of the prefab
					inGame.transform.position = inGame.GetComponent<Pickups>().startingPosition0;
				}
			}
		}
	}
	public void Restart()
	{
		// Move along, nothing to see here
	}

	public bool GameEnded()
	{
		return currentTime <= 0;
	}

	public bool GameStarted()
	{
		return currentWarmup <= 0;
	}

	public bool isPaused()
	{
		return paused;
	}

	public void Pause(bool state)
	{
		paused = state;
		ShowRootMenu (state);
	}

	public bool RegisterPlayer(PlayerController player)
	{
		if (players.ContainsKey (player.playerNumber))
		{
			return false;
		}
		else
		{
			players.Add (player.playerNumber, player);
			return true;
		}
	}

	public int GetScore(int id)
	{
		return GameObject.Find ("NEST" + id).GetComponent<NEST> ().GetScore ();
	}

	public float DTR (float degrees)
	{
		return DegreesToRadians (degrees);
	}

	public float RTD (float radians)
	{
		return RadiansToDegrees (radians);
	}
}
