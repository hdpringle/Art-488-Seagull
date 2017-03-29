﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System;

public class InputValues
{
	public float moveForward, turnUD, turnLR, drop;
}

public class PlayerController : MonoBehaviour
{

    public Text scoreText, winText;
	public GameController game;
	public int playerNumber;

    private Rigidbody rb;
    private float yaw, pitch;
    private bool holding, flying;
	protected InputValues input;
    Transform heldObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        yaw = transform.rotation.eulerAngles.y;
        pitch = transform.rotation.eulerAngles.x;
        winText.text = "";
        rb.freezeRotation = true;
		holding = false;
		flying = true;
		input = new InputValues ();
    }

	void Update()
	{
		int score = game.GetScore (playerNumber);
		scoreText.text = "Score: " + score.ToString();
		if (score >= game.autowinScore)
		{
			winText.text = "YOU WIN!!";
		}
	}

    // Called for physics
    void FixedUpdate()
    {
		if (!game.GameStarted () || game.GameEnded () || game.isPaused ())
		{
			rb.velocity = Vector3.zero;
			return;
		}

		GetControls ();

		DoMotion ();
    }

	protected virtual void GetControls()
	{
		switch (playerNumber)
		{
		case 2:
			input.moveForward = Input.GetAxis ("P2 Forward");
			input.turnUD = Input.GetAxis ("P2 Vertical");
			input.turnLR = FABS (Input.GetAxis ("P2 Horizontal"), Input.GetAxis ("P2 Sideways"));
			input.drop = Input.GetAxis ("P2 Drop");
			break;
		default:
			input.moveForward = Input.GetAxis ("Forward");
			input.turnUD = Input.GetAxis ("Vertical");
			input.turnLR = FABS (Input.GetAxis ("Horizontal"), Input.GetAxis ("Sideways"));
			input.drop = Input.GetAxis ("Drop");
			break;
		}
	}

	protected void DoMotion()
	{
		yaw += game.seagullLimits.rotationLR * input.turnLR;

		if (flying) {
			pitch -= game.seagullLimits.rotationUD * input.turnUD;
			pitch = Mathf.Clamp (pitch, -game.seagullLimits.upAngle, game.seagullLimits.downAngle);

			Vector3 movement = -transform.InverseTransformDirection (rb.velocity) * game.seagullLimits.antiDrift;
			if (input.moveForward > 0 || (input.moveForward < 0 && movement.z < 0)) {
				movement.z = input.moveForward * game.seagullLimits.accelSpeed;
			} else {
				movement.z = movement.z / game.seagullLimits.antiDrift * game.seagullLimits.glideDecel;
			}
			rb.AddRelativeForce (movement);
			
			rb.rotation = Quaternion.Euler (pitch, yaw, input.turnLR * -game.seagullLimits.tilt);

			rb.position = new Vector3 (
				Mathf.Clamp (rb.position.x, game.boundary.xMin, game.boundary.xMax),
				Mathf.Clamp (rb.position.y, game.sea.position.y, game.boundary.yMax),
				Mathf.Clamp (rb.position.z, game.boundary.zMin, game.boundary.zMax)
			);

			Vector3 speed = rb.velocity;

			//if we get to slow, start falling slowly
			if(speed.magnitude < 1)
			{
				rb.AddRelativeForce(new Vector3(0,-0.5f,0));
			}

			//increase flapping speed!
			GetComponent<Animator>().speed = Mathf.Max(speed.magnitude, 0.75f);
		}
		else { // walking
			
		}



		//move the held object with you
		if(holding)
		{
			heldObject.position = transform.FindChild("MountPoint").transform.position - (heldObject.FindChild("MountPoint").transform.position - heldObject.position);
			heldObject.eulerAngles = heldObject.gameObject.GetComponent<Pickups>().startingRotation + transform.eulerAngles;
		}

		if (input.drop > 0 && holding)
		{
			holding = false;
			heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("pickup"))
        {
            if(holding)
            {
                // Display warning message?
            }
            else
			{
				//Check to see if the object we collided with is in our own nest
				if (!GameObject.Find("NEST" + playerNumber).GetComponent<NEST>().isInNest(other.gameObject))
				{
					//other.gameObject.SetActive(false);
					heldObject = other.transform;
					heldObject.gameObject.GetComponent<Pickups>().heldByPlayer = playerNumber;
					holding = true;
				}

				//if you stole this item, let the nest know its gone
				if(playerNumber != 0 && GameObject.Find("NEST"+(playerNumber%2+1)).GetComponent<NEST>().isInNest(other.gameObject))
				{
					GameObject.Find("NEST" + (playerNumber % 2 + 1)).GetComponent<NEST>().removeFromNest(other.gameObject);
				}
            }
        }
        if(other.CompareTag("nest"))
        {
			if(holding && other.gameObject.GetComponent<NEST>().nestId == playerNumber)
			{
				holding = false;
				other.gameObject.GetComponent<NEST>().addObject(heldObject.gameObject);
			}
        }
		else if(other.CompareTag("Player"))
		{
			if(!holding)
			{
				//You get the item
				heldObject = other.gameObject.GetComponent<PlayerController>().heldObject;
				heldObject.gameObject.GetComponent<Pickups>().heldByPlayer = playerNumber;
				holding = true;

				//they lose the item
				other.gameObject.GetComponent<PlayerController>().holding = false;
			}
		}
    }

	/**
	 * Max absolute value function for floats
	 */
	public float FABS (float a, float b)
	{
		float asq = Mathf.Pow (a, 2);
		float bsq = Mathf.Pow (b, 2);

		if (asq > bsq)
			return a;
		return b;
	}

	public bool isHolding()
	{
		return holding;
	}

	public void setHolding(bool state)
	{
		holding = state;
	}
}