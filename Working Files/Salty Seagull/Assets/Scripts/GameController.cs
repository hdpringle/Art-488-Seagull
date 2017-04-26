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

public class PlayerSpawnInfo
{
	public int id;
	public GameObject hud;
	public Camera camera;
	public GameObject playerObject;
	public PlayerController player;
	public NEST nest;
	public int seagullSkinNumber;
}

public class GameController : MenuController
{
	public GameObject playerPrefab, nestPrefab, hudPrefab;
	public int autowinScore;
	public float timeLimitSeconds, warmupTime;
	public Boundary boundary;
	public SeagullLimits seagullLimits;
	public int numberOfNests; //used so the seagulls know whos nest they stole from (see playerController)
	public Transform sea;
	public ArrayList filledPickups;

	private Text winText;
	private float currentTime, currentWarmup;
	private int minutes, seconds;
	private int lastChangeInSeconds;
	//private int lastMinutes, lastSeconds; //needed to not recheck spawn times
	private bool paused, gameOver;
	private Dictionary<int, PlayerSpawnInfo> playerInfo;
	private GameObject gameOverMenu;
	
	// Use this for initialization
	void Start ()
	{
		paused = false;
		gameOver = false;
		gameOverMenu = GameObject.Find("GameOverMenu");
		gameOverMenu.SetActive(false);
		currentTime = timeLimitSeconds;
		currentWarmup = warmupTime;
		numberOfNests = settings.numPlayers; 
		sea = GameObject.Find ("Sea").transform;
		winText = GameObject.Find ("WinText").GetComponent<Text> ();
		winText.text = "";
		filledPickups = new ArrayList();
		// This block randomly chooses a set of spawn points for each player and nest
		playerInfo = new Dictionary<int, PlayerSpawnInfo> ();
		GameObject pSpawnList = GameObject.Find ("PlayerSpawnPoints");
		GameObject nSpawnList = GameObject.Find ("NestSpawnPoints");
		// If the spawnpoint system does not exist, do not run this code
		if (pSpawnList != null)
		{
			Transform[] pspawnpoints = pSpawnList.GetComponentsInChildren<Transform> ();
			Transform[] nspawnpoints = nSpawnList.GetComponentsInChildren<Transform> ();

			List<int> pointNumbers = new List<int>();
			for (int i = 1; i < pspawnpoints.Length; i++)
			{
				pointNumbers.Add (i);
			}

			// Assure we don't try to use more players than spawnpoints
			settings.numPlayers = Mathf.Min (settings.numPlayers, pointNumbers.Count);
			for (int i = 1; i <= settings.numPlayers; i++)
			{
				PlayerSpawnInfo info = new PlayerSpawnInfo ();

				// Randomly select an available spawn position
				int sp;
				do {
					sp = Random.Range (1, pspawnpoints.Length);
				} while (!pointNumbers.Contains (sp));

				GameObject newHUD = GameObject.Instantiate (hudPrefab, GameObject.Find ("Canvas").transform);
				GameObject newplayer = GameObject.Instantiate (playerPrefab.transform.FindChild("Character"+settings.skinNumbers[i-1]).gameObject, pspawnpoints[sp].position, pspawnpoints[sp].rotation);
				GameObject newnest = GameObject.Instantiate (nestPrefab, nspawnpoints[sp].position, nspawnpoints[sp].rotation);
				Material[] beacons = Resources.LoadAll<Material> ("Materials");

				newHUD.name = "HUD" + i;
				newplayer.name = "Player" + i;
				newnest.name = "Nest" + i;
				info.id = i;
				info.playerObject = newplayer;
				info.player = newplayer.GetComponent<PlayerController> ();
				info.player.playerNumber = i;
				info.nest = newnest.GetComponent<NEST> ();
				info.nest.nestId = i;
				info.hud = newHUD;
				info.hud.transform.SetSiblingIndex (0);
				info.hud.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
				newplayer.transform.FindChild ("Identifier").GetComponent<MeshRenderer> ().material = beacons[info.player.playerNumber-1];
				newnest.transform.FindChild("Beacon").GetComponent<MeshRenderer>().material = beacons[info.player.playerNumber-1];
				playerInfo [i] = info;
				pointNumbers.Remove (sp);
			}


			// Set up cameras
			Camera mainCamera = GameObject.FindObjectOfType<Camera> ();
			playerInfo [1].camera = mainCamera;
			mainCamera.GetComponent<TransformFollower> ().target = playerInfo[1].playerObject.transform;
			for (int i = 2; i <= settings.numPlayers; i++)
			{
				Camera newCam = Instantiate<Camera> (mainCamera);
				newCam.name = "Camera(P" + i + ")";
				newCam.GetComponent<AudioListener> ().enabled = false;
				newCam.GetComponent<TransformFollower> ().target = playerInfo[i].playerObject.transform;
				playerInfo [i].camera = newCam;
			}

			// Scale cameras and HUDs
			switch (settings.numPlayers)
			{
			case 1:
				playerInfo [1].camera.rect = new Rect (0f, 0f, 1f, 1f);
				break;
			case 2:
				playerInfo [1].camera.rect = new Rect (0f, 0f, 0.5f, 1f);
				playerInfo [2].camera.rect = new Rect (0.5f, 0f, 0.5f, 1f);
				playerInfo [2].hud.GetComponent<RectTransform> ().anchorMin = new Vector2(0.5f, 1f);
				break;
			case 3:
				playerInfo [1].camera.rect = new Rect (0f, 0.5f, 1f, 0.5f);
				playerInfo [2].camera.rect = new Rect (0f, 0f, 0.5f, 0.5f);
				playerInfo [3].camera.rect = new Rect (0.5f, 0f, 0.5f, 0.5f);
				playerInfo [2].hud.GetComponent<RectTransform> ().anchorMax = new Vector2(0f, 0.5f);
				playerInfo [3].hud.GetComponent<RectTransform> ().anchorMin = new Vector2(0.5f, 0f);
				playerInfo [3].hud.GetComponent<RectTransform> ().anchorMax = new Vector2(1f, 0.5f);
				break;
			case 4:
				playerInfo [1].camera.rect = new Rect (0f, 0.5f, 0.5f, 0.5f);
				playerInfo [2].camera.rect = new Rect (0.5f, 0.5f, 0.5f, 0.5f);
				playerInfo [3].camera.rect = new Rect (0f, 0f, 0.5f, 0.5f);
				playerInfo [4].camera.rect = new Rect (0.5f, 0f, 0.5f, 0.5f);
				playerInfo [2].hud.GetComponent<RectTransform> ().anchorMin = new Vector2(0.5f, 0f);
				playerInfo [3].hud.GetComponent<RectTransform> ().anchorMax = new Vector2(0f, 0.5f);
				playerInfo [4].hud.GetComponent<RectTransform> ().anchorMin = new Vector2(0.5f, 0f);
				playerInfo [4].hud.GetComponent<RectTransform> ().anchorMax = new Vector2(1f, 0.5f);
				break;
			}
		}

		Pause (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if you hit the pause button, toggle "paused"
		if (Input.GetButtonDown("Pause") || Input.GetButtonDown("Cancel"))
		{
			Pause (!paused);
		}

		if (!(paused || gameOver))
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


					//check how many seconds have passed, if it has been a second since we tried to spawn something,
					//try to spawn something new!
					int s = (int)(timeLimitSeconds-currentTime);
					if (lastChangeInSeconds != s)
					{
						lastChangeInSeconds = s;
						SpawnPickups(s);
					}
					if((int)currentTime <= 10)
					{
						winText.text = ""+(int)currentTime;
					}
				}
				else
				{
					int topScore = -1;
					int topScorer = -1;
					foreach (PlayerSpawnInfo info in playerInfo.Values)
					{
						if (info.nest.GetScore () > topScore)
						{
							topScore = info.nest.GetScore ();
							topScorer = info.id;
						}
					}
					winText.text = "Player " + topScorer + " wins!";
					gameOver = true;
					gameOverMenu.SetActive(true); 

				}
			}
		}
	}

	/*
	 * Will check to see if the spawn time of a pickup prefab
	 * matches the current time. Then will spawn the item in one of 
	 * the given spawn locations
	 */ 
	public void SpawnPickups(int secs)
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
				if (secs % gObj.GetComponent<Pickups>().spawnTime == 0)
				{
					Transform[] spawnLocation = GameObject.Find(gObj.GetComponent<Pickups>().itemName + "SpawnPoints").transform.GetComponentsInChildren<Transform>();
					var chosenLocation = 0;
					var timesLoaded = 0;
					do
					{
						chosenLocation = Random.Range(0,spawnLocation.Length);
						//only try 20 times, if you cant spawn anything, just give up
						timesLoaded++;
					} while ((spawnLocation[chosenLocation] == GameObject.Find(gObj.GetComponent<Pickups>().itemName + "SpawnPoints").transform || filledPickups.Contains(spawnLocation[chosenLocation])) && timesLoaded < 20);
					if (timesLoaded < 20)
					{
						filledPickups.Add(spawnLocation[chosenLocation]);
						GameObject inGame = Instantiate(gObj);

						//make this part random later!
						//or we can have all of the instanses spawn in at once, if we use a variable to mark how many
						//of the object needs to be spawned as part of the prefab
						inGame.transform.position = spawnLocation[chosenLocation].position;
						inGame.transform.rotation = spawnLocation[chosenLocation].rotation;
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

	//sets the pause value to whatever state is
	//shows the pause menu if it says to
	public void Pause(bool state)
	{
		paused = state;
		//should toggle the pause menu
		ShowRootMenu (state);
		if (state)
			rootMenu.GetComponent<PauseMenu> ().GC = this;
	}

	public int GetScore(int id)
	{
		return playerInfo[id].nest.GetScore();
	}

	public float DTR (float degrees)
	{
		return DegreesToRadians (degrees);
	}

	public float RTD (float radians)
	{
		return RadiansToDegrees (radians);
	}

	public void Quit()
	{
		settings.numPlayers = ((settings.numPlayers + 1) % 4) + 1;
		ChangeScene ("MainMenu");
	}
}
