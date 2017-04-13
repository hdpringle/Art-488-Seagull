using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Configuration;

public class SessionSettings
{
	public int numPlayers = 3;
}

public class MenuController : MonoBehaviour {

	public static SessionSettings settings = new SessionSettings ();

	//rootMenu is the pause screen in-game and the main menu in the MainMenu scene
	//settings menu is a nonexistent settings screen
	public GameObject rootMenu, settingsMenu;

	// currentMenu is the currently open menu, if there is one
	// parentMenu is the menu from which this one was accessed, in the case of nesting
	private GameObject currentMenu, parentMenu;

	void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			
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
		Debug.Log ("num: " + settings.numPlayers);
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
			settingsMenu.SetActive (state);
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
