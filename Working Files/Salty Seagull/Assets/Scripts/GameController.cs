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
	public Camera camera;
	public GameObject playerObject;
	public PlayerController player;
	public NEST nest;
	public int id;
}

public class GameController : MenuController
{
	public GameObject playerPrefab, nestPrefab;
	public int autowinScore;
	public float timeLimitSeconds, warmupTime;
	public Transform sea;
	public Boundary boundary;
	public SeagullLimits seagullLimits;
	public int numberOfNests; //used so the seagulls know whos nest they stole from (see playerController)

	private float currentTime, currentWarmup;
	private int minutes, seconds;
	private int lastMinutes, lastSeconds; //needed to not recheck spawn times
	private bool paused;
	private Dictionary<int, PlayerSpawnInfo> playerInfo;

	// Use this for initialization
	void Start ()
	{
		currentTime = timeLimitSeconds;
		currentWarmup = warmupTime;

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
				pointNumbers.Add (i - 1);
			}

			// Assure we don't try to use more players than spawnpoints
			settings.numPlayers = Mathf.Min (settings.numPlayers, pointNumbers.Count);

			for (int i = 1; i <= settings.numPlayers; i++)
			{
				Debug.Log ("About to setup player " + i);
				PlayerSpawnInfo info = new PlayerSpawnInfo ();

				// Randomly select an available spawn position
				int sp;
				do {
					sp = Random.Range (0, pointNumbers.Count);
					Debug.Log ("Spawn position: " + sp);
				} while (!pointNumbers.Contains (sp));

				GameObject newplayer = GameObject.Instantiate (playerPrefab, pspawnpoints[sp].position, pspawnpoints[sp].rotation);
				GameObject newnest = GameObject.Instantiate (nestPrefab, nspawnpoints[sp].position, nspawnpoints[sp].rotation);
				List<Material> beacons = new List<Material>(Resources.LoadAll<Material> ("Materials"));

				newplayer.name = "Player" + i;
				newnest.name = "Nest" + i;
				info.id = i;
				info.playerObject = newplayer;
				info.player = newplayer.GetComponent<PlayerController> ();
				info.player.playerNumber = i;
				info.nest = newnest.GetComponent<NEST> ();
				info.nest.nestId = i;
				newnest.transform.FindChild ("Beacon").GetComponent<MeshRenderer> ().material = beacons[sp];
				playerInfo [i] = info;
				pointNumbers.Remove (sp);
				Debug.Log ("Done setting up player " + i);
			}

			// Set up cameras
			Camera mainCamera = GameObject.FindObjectOfType<Camera> ();
			playerInfo [1].camera = mainCamera;
			mainCamera.GetComponent<TransformFollower> ().target = playerInfo[1].playerObject.transform;
			for (int i = 2; i <= settings.numPlayers; i++)
			{
				Debug.Log ("About to set up camera " + i);
				Camera newCam = Instantiate<Camera> (mainCamera);
				newCam.name = "Camera(P" + i + ")";
				newCam.GetComponent<AudioListener> ().enabled = false;
				newCam.GetComponent<TransformFollower> ().target = playerInfo[i].playerObject.transform;
				playerInfo [i].camera = newCam;
				Debug.Log ("Done setting up camera " + i);
			}

			switch (settings.numPlayers)
			{
			case 1:
				Debug.Log ("Resizing cameras for 1 player.");
				playerInfo [1].camera.rect = new Rect (0f, 0f, 1f, 1f);
				break;
			case 2:
				Debug.Log ("Resizing cameras for 2 players.");
				playerInfo [1].camera.rect = new Rect (0f, 0f, 0.5f, 1f);
				playerInfo [2].camera.rect = new Rect (0.5f, 0f, 0.5f, 1f);
				break;
			case 3:
				Debug.Log ("Resizing cameras for 3 players.");
				playerInfo [1].camera.rect = new Rect (0f, 0.5f, 1f, 0.5f);
				playerInfo [2].camera.rect = new Rect (0f, 0f, 0.5f, 0.5f);
				playerInfo [3].camera.rect = new Rect (0.5f, 0f, 0.5f, 0.5f);
				break;
			case 4:
				Debug.Log ("Resizing cameras for 4 players.");
				playerInfo [1].camera.rect = new Rect (0f, 0.5f, 0.5f, 0.5f);
				playerInfo [2].camera.rect = new Rect (0.5f, 0.5f, 0.5f, 0.5f);
				playerInfo [3].camera.rect = new Rect (0f, 0f, 0.5f, 0.5f);
				playerInfo [4].camera.rect = new Rect (0.5f, 0f, 0.5f, 0.5f);
				break;
			}
		}

		rootMenu = GameObject.Find("Pause Menu");
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
					GameObject.Find ("Win Text").GetComponent<Text> ().text = "Player " + topScorer + " wins!";
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

	//sets the pause value to whatever state is
	//shows the pause menu if it says to
	public void Pause(bool state)
	{
		print(state);
		paused = state;
		//should toggle the pause menu
		ShowRootMenu (state);
	}

	/*public bool RegisterPlayer(PlayerController player)
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
	}*/

	public int GetScore(int id)
	{
		return playerInfo[id].nest.GetScore ();
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
		settings.numPlayers = (settings.numPlayers + 1) % 4;
		ChangeScene ("MainMenu");
	}
}
