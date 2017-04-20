using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MenuController {

	// The game controller on the current scene
	public GameController GC
	{
		set
		{
			if (gc == null)
			{
				gc = value;
			}
		}
		get { return gc; }
	}

	private GameController gc;
	private Button resumeButton, settingsButton, mainMenuButton, quitButton;

	void Start ()
	{
		resumeButton = transform.Find ("ResumeButton").GetComponent<Button> ();
		resumeButton.onClick.AddListener (Resume);

		settingsButton = transform.Find ("SettingsButton").GetComponent<Button> ();
		settingsButton.onClick.AddListener (Settings);

		mainMenuButton = transform.Find ("MainMenuButton").GetComponent<Button> ();
		mainMenuButton.onClick.AddListener (MyQuit);

		quitButton = transform.Find ("QuitButton").GetComponent<Button> ();
		quitButton.onClick.AddListener (MyExit);
	}

	private void Resume ()
	{
		GC.Pause (false);
	}

	private void Settings ()
	{
		GC.ShowSettingsMenu (true);
	}

	private void MyQuit ()
	{
		GC.Quit ();
	}

	private void MyExit ()
	{
		Exit ();
	}
}
