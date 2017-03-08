﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System;

public class InputValues
{
	public float moveForward, turnUD, turnLR;
}

public class PlayerController : MonoBehaviour
{

    public Text scoreText, winText, warnText, timer;
    public int requiredScore = 10;
	public GameController game;

    private Rigidbody rb;
    public int count;
    private float yaw, pitch;
    public bool holding;
	private InputValues input;

    Transform heldObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        yaw = transform.rotation.eulerAngles.y;
        pitch = transform.rotation.eulerAngles.x;
        updateScore();
        winText.text = "";
        rb.freezeRotation = true;
		holding = false;
		input = new InputValues ();
    }

	void Update()
	{
		if ((Input.GetKeyDown(("joystick 1 button " + 5.ToString()))) || Input.GetKeyDown(KeyCode.Space) && holding)
		{
			holding = false;
			heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
		}
	}

    // Called for physics
    void FixedUpdate()
    {
		if (!game.GameStarted () || game.GameEnded () || game.isPaused ())
		{
			return;
		}

		GetControls ();

		DoMotion ();
    }

	protected virtual void GetControls()
	{
		input.moveForward = Input.GetAxis("Forward");
		input.turnUD = FABS (Input.GetAxis ("Vertical"), Input.GetAxis ("Mouse Y"));
		input.turnLR = FABS (Input.GetAxis("Horizontal"), Input.GetAxis("Mouse X"));
	}

	protected void DoMotion()
	{
		yaw += game.seagullLimits.rotationLR * input.turnLR;
		pitch -= game.seagullLimits.rotationUD * input.turnUD;

		pitch = Mathf.Clamp(pitch, -game.seagullLimits.upAngle, game.seagullLimits.downAngle);

		Vector3 movement = -transform.InverseTransformDirection(rb.velocity) * game.seagullLimits.antiDrift;
		if (input.moveForward > 0 || (input.moveForward < 0 && movement.z < 0))
		{
			movement.z = input.moveForward * game.seagullLimits.accelSpeed;
		}
		else
		{
			movement.z = movement.z / game.seagullLimits.antiDrift * game.seagullLimits.glideDecel;
		}

		rb.AddRelativeForce(movement);

		// rb.MoveRotation (rb.rotation * Quaternion.Euler( new Vector3 (pitch, yaw, 0)));
		rb.rotation = Quaternion.Euler(pitch, yaw, input.turnLR * -game.seagullLimits.tilt);
		// rb.rotation = Quaternion.Euler (pitch, yaw, 0);

		rb.position = new Vector3
			(
				Mathf.Clamp(rb.position.x, game.boundary.xMin, game.boundary.xMax),
				Mathf.Clamp(rb.position.y, game.sea.position.y, game.boundary.yMax),
				Mathf.Clamp(rb.position.z, game.boundary.zMin, game.boundary.zMax)
			);


		//move the held object with you
		if(holding)
		{
			/*
			//should set the held objects mount point's position equal to the seagull's mount point position
            heldObject.FindChild("MountPoint").transform.position = transform.FindChild("MountPoint").transform.position;

			//should update the object to move with it's mount point
			heldObject.position = heldObject.Find("MountPoint").transform.position - heldObject.GetComponent<Pickups>().offset;
			*/
			heldObject.position = transform.FindChild("MountPoint").transform.position - (heldObject.FindChild("MountPoint").transform.position - heldObject.position);
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("pickup"))
        {
            if(holding)
            {
                // Display warning message
            }
            else
            {
                //other.gameObject.SetActive(false);
                heldObject = other.transform;
                holding = true;
            }
        }
        if(other.CompareTag("nest"))
        {
            holding = false;
            //count++;
            updateScore();
        }
    }

    public void updateScore()
    {
        scoreText.text = "Score: " + count.ToString();
        if (count >= requiredScore)
        {
            winText.text = "YOU WIN!!";
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
}