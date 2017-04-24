using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelect : MonoBehaviour {
	
	// map - see below
	// field:
	//		0 - map select
	//		1 - player count select
	private int map, field;

	private string[] maps = {"BigIsland", "Cove"};

	private class GSI
	{
		public float updown, leftright;
	}
	private GSI inputs;

	// Use this for initialization
	void Start ()
	{
		map = 0;
		field = 0;
		inputs = new GSI ();
		InvokeRepeating("BestUpdate", 0f, 0.25f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetControls ();
	}

	void BestUpdate()
	{
		if (inputs.updown > 0)
		{
			field++;
		}
		else if (inputs.updown < 0)
		{
			field--;
		}
		Mathf.Clamp (field, 0, maps.Length);
	}

	protected virtual void GetControls()
	{
		inputs.updown = Input.GetAxis("Forward");
		inputs.leftright = Input.GetAxis("Sideways");
	}
}
