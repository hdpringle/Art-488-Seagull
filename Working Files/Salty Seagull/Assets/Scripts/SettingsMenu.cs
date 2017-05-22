using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuController {

	private Button back;
	private Toggle[] inverts;
	private Button[] controls;
	private Slider musicVolume;
	private Text mvDisplay;
	private AudioSource music;

	public MenuController Master
	{
		set
		{
			if (master == null)
			{
				master = value;
			}
		}
		get { return master; }
	}
	private MenuController master;

	// Use this for initialization
	void Start ()
	{
		back = GetComponentInChildren<Button> ();
		back.onClick.AddListener (ExitSettings);

		inverts = new Toggle[4];
		controls = new Button[4];

		for (int i = 0; i < 4; i++)
		{
			inverts [i] = transform.Find ("P" + (i + 1) + "InvertToggle").GetComponent<Toggle> ();
			inverts [i].isOn = settings.inversions [i];

			controls [i] = transform.Find ("P" + (i + 1) + "ControlToggle").GetComponent<Button> ();
			controls [i].GetComponentInChildren<Text> ().text = settings.schemes [i].ToString ();
		}

		inverts[0].onValueChanged.AddListener (Toggle1);
		inverts[1].onValueChanged.AddListener (Toggle2);
		inverts[2].onValueChanged.AddListener (Toggle3);
		inverts[3].onValueChanged.AddListener (Toggle4);

		controls[0].onClick.AddListener (Control1);
		controls[1].onClick.AddListener (Control2);
		controls[2].onClick.AddListener (Control3);
		controls[3].onClick.AddListener (Control4);


		music = Component.FindObjectOfType<AudioSource> ();
		mvDisplay = transform.Find ("MusicText").GetComponent<Text> ();
		musicVolume = transform.Find ("MusicSlider").GetComponent<Slider> ();
		musicVolume.onValueChanged.AddListener (MusicChange);
		musicVolume.value = (int) (settings.musicVolume / 5);
	}

	private void ExitSettings ()
	{
		master.ShowRootMenu (true);
		master.ShowSettingsMenu (false);
	}

	private void Toggle1 (bool value)
	{
		settings.inversions [0] = value;
	}

	private void Toggle2 (bool value)
	{
		settings.inversions [1] = value;
	}

	private void Toggle3 (bool value)
	{
		settings.inversions [2] = value;
	}

	private void Toggle4 (bool value)
	{
		settings.inversions [3] = value;
	}

	private void Control1 ()
	{
		settings.schemes [0] = CSCycle (settings.schemes[0]);
		controls [0].GetComponentInChildren<Text> ().text = settings.schemes [0].ToString ();
	}

	private void Control2 ()
	{
		settings.schemes [1] = CSCycle (settings.schemes[1]);
		controls [1].GetComponentInChildren<Text> ().text = settings.schemes [1].ToString ();
	}

	private void Control3 ()
	{
		settings.schemes [2] = CSCycle (settings.schemes[2]);
		controls [2].GetComponentInChildren<Text> ().text = settings.schemes [2].ToString ();
	}

	private void Control4 ()
	{
		settings.schemes [3] = CSCycle (settings.schemes[3]);
		controls [3].GetComponentInChildren<Text> ().text = settings.schemes [3].ToString ();
	}

	// This is done this way in case we add more control schemes in the future
	ControlSchemes CSCycle(ControlSchemes v)
	{
		switch (v)
		{
		case ControlSchemes.FLIGHTSIM:
			return ControlSchemes.MARIOKART;
		case ControlSchemes.MARIOKART:
			return ControlSchemes.FLIGHTSIM;
		}
		return ControlSchemes.MARIOKART;
	}

	private void MusicChange (float val)
	{
		settings.musicVolume = musicVolume.value * 5;
		mvDisplay.text = "Music Volume: " + (int) settings.musicVolume;
		music.volume = settings.musicVolume / 100f;
	}
}
