using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Configuration;
using System;

public class SessionSettings
{
	public int numPlayers = 4;
	public String mapChosen = "BigIsland";
	public int matchLengthSeconds = 180;
	public int autoWinScore = 30;

	public static int MAX_PLAYERS_BIG_ISLAND
	{
		get { return 4; }
	}
	public static int MAX_PLAYERS_ISLAND
	{
		get { return 3; }
	}
	public static int MAX_PLAYERS_COVE
	{
		get { return 2; }
	}

	// State refers to whether the player's pitch controls are flipped
	public bool[] inversions = { false, false, false, false };

	//The number refers to which skin material to load ("seagullSkin#" is the name of each material)
	//the index is the player number
	public int[] skinNumbers = { 1, 1, 1, 1 };

	// 0-1 how pumped up are the jams?
	public float musicVolume = 1f;
}

public class MenuController : MonoBehaviour {

	public static SessionSettings settings = new SessionSettings ();

	//rootMenu is the pause screen in-game and the main menu in the MainMenu scene
	public GameObject rootMenu, settingsMenu;

	// currentMenu is the currently open menu, if there is one
	// parentMenu is the menu from which this one was accessed, in the case of nesting
	protected GameObject currentMenu;

	protected EventSystem navigator;

	void Start()
	{
		currentMenu = rootMenu;
		navigator = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioSource> ().volume = settings.musicVolume;
	}

	void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			if (currentMenu == settingsMenu)
			{
				ShowSettingsMenu (false);
			}
		}

		if (currentMenu != null && !navigator.currentSelectedGameObject.transform.IsChildOf (currentMenu.transform))
		{
			navigator.SetSelectedGameObject (currentMenu.transform.GetChild (1).gameObject);
		}
	}

	//changes the scene to some new screen
	public void ChangeScene(string name)
	{
		Scene myScene = SceneManager.GetSceneByName (name);
		if (!myScene.isLoaded)
		{
			SceneManager.LoadScene (name);
		}

		if (myScene.isLoaded && myScene.IsValid ())
		{
			SceneManager.SetActiveScene (myScene);
		}
	}

	public void Exit()
	{
		Application.Quit ();
	}

	//Shows the pause menu
	public void ShowRootMenu(bool state)
	{
		if (rootMenu != null)
		{
			rootMenu.SetActive(state);
		}
	}

	//Displays the settings
	public void ShowSettingsMenu(bool state)
	{
		if (settingsMenu != null)
		{
			if (state)
			{
				settingsMenu.SetActive (state);
				currentMenu = settingsMenu;
				settingsMenu.GetComponent<SettingsMenu> ().Master = this;
				navigator.SetSelectedGameObject (settingsMenu.transform.GetChild (1).gameObject);
			} else {
				settingsMenu.SetActive (false);
				currentMenu = rootMenu;
				navigator.SetSelectedGameObject (rootMenu.transform.GetChild (1).gameObject);
			}
		}
	}

	public float DegreesToRadians (float degrees)
	{
		return degrees * Mathf.PI / 180;
	}

	public float RadiansToDegrees (float radians)
	{
		return radians * 180 / Mathf.PI;
	}

	public void changePlayerNumber(int num)
	{
		settings.numPlayers = num;
	}
}
