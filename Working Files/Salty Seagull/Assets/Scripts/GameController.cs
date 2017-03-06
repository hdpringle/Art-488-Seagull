using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public float timeLimitSeconds, warmupTime;
	public Text timer;

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
