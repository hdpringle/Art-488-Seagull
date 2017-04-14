using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPlayerSelect : MenuController {

	GameObject[] players;
	// Use this for initialization
	void Start ()
	{
		players = new GameObject[4];
		for(int i = 0; i < players.Length; i++)
		{
			players[i] = GameObject.Find("Player" + (i + 1) + " Loader");
			if(settings.numPlayers < (i+1))
			{
				players[i].gameObject.SetActive(false);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		for(int i = 0; i < settings.numPlayers; i++)
		{
			settings.skinNumbers[i] = players[i].GetComponent<PlayerSelecter>().currentSeagull;
		}

		for(int i = 0; i < settings.numPlayers; i++)
		{
			if(!players[i].GetComponent<PlayerSelecter>().chosen)
			{
				return;
			}
		}
		ChangeScene("BigIsland");
	}
}
