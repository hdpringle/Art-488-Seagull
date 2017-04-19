﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuController {

	private Button back;
	private Toggle[] inverts;

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
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void ExitSettings ()
	{
		gameObject.SetActive (false);
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
}
