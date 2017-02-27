using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public float timeLimitSeconds;
	public Text timer;

	private float currentTime;
	private int minutes, seconds;

	// Use this for initialization
	void Start () {
		currentTime = timeLimitSeconds;
	}
	
	// Update is called once per frame
	void Update () {
		while (currentTime > 0) {
			currentTime -= Time.deltaTime;
			minutes = ((int)currentTime) / 60;
			seconds = ((int)currentTime) % 60;

			timer.text = minutes + ":" + seconds;
		}
	}
}
