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
	public float upAngle = 50f, downAngle = 35f, accelSpeed = 6f, glideDecel = 0.005f, antiDrift = 3f, rotationLR = 3f, rotationUD = 3f, maxSpeed = 10f, tilt = 35f;
	// public float upAngle = 50, downAngle = 35, accelSpeed = 6, glideDecel = 0.005, antiDrift = 3, rotationLR = 3, rotationUD = 3, maxSpeed = 10, tilt = 35;
}

public class GameController : MonoBehaviour
{
	public int autowinScore;
	public float timeLimitSeconds, warmupTime;
	public Transform sea;
	public Boundary boundary;
	public SeagullLimits seagullLimits;

	private float currentTime, currentWarmup;
	private int minutes, seconds;
	private bool paused;
	private Dictionary<int, PlayerController> players;
	private Dictionary<int, int> scores;

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
		if (currentWarmup > 0) {
			currentWarmup -= Time.deltaTime;
		} else {
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
		ShowMenu (state);
	}

	public void ShowMenu(bool state)
	{
		GameObject pauseMenu = GameObject.Find ("PauseMenu");
		if (pauseMenu != null)
		{
			pauseMenu.SetActive (state);
		}
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
			scores.Add (player.playerNumber, 0); 
			return true;
		}
	}
}
