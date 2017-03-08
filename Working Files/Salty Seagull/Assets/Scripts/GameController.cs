using System.Collections;
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

	public float timeLimitSeconds, warmupTime;
	public Text timer;
	public Transform sea;
	public Boundary boundary;
	public SeagullLimits seagullLimits;

	private float currentTime;
	private int minutes, seconds;
	private bool paused;

	// Use this for initialization
	void Start ()
	{
		currentTime = timeLimitSeconds;
		paused = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (currentTime > 0) {
			currentTime -= Time.deltaTime;
			minutes = ((int)currentTime) / 60;
			seconds = ((int)currentTime) % 60;

			timer.text = minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
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
		return true;
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
		
	}
}
