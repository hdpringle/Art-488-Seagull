using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuController {

	private Button back;
	private Toggle[] inverts;
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

		for (int i = 0; i < 4; i++)
		{
			inverts [i] = transform.Find ("P" + (i + 1) + "InvertToggle").GetComponent<Toggle> ();
			inverts [i].isOn = settings.inversions [i];
		}

		inverts[0].onValueChanged.AddListener (Toggle1);
		inverts[1].onValueChanged.AddListener (Toggle2);
		inverts[2].onValueChanged.AddListener (Toggle3);
		inverts[3].onValueChanged.AddListener (Toggle4);

		musicVolume = transform.Find ("MusicSlider").GetComponent<Slider> ();
		musicVolume.value = (int) (settings.musicVolume * 100 / 5);
		musicVolume.onValueChanged.AddListener (MusicChange);

		mvDisplay = transform.Find ("MusicText").GetComponent<Text> ();
		mvDisplay.text = "Music Volume: " + (int) (settings.musicVolume * 100);

		music = Component.FindObjectOfType<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
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

	private void MusicChange (float val)
	{
		settings.musicVolume = musicVolume.value * 5 / 100;
		mvDisplay.text = "Music Volume: " + (int) (settings.musicVolume * 100);
		music.volume = settings.musicVolume;
	}
}
