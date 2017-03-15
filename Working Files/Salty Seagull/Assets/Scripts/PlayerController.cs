using System.Collections;
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
    public int count;
    private float yaw, pitch;
    private bool holding;
	protected InputValues input;
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
		input.moveForward = Input.GetAxis("Forward");
		input.turnUD = FABS (Input.GetAxis ("Vertical"), Input.GetAxis ("Mouse Y"));
		input.turnLR = FABS (Input.GetAxis("Horizontal"), Input.GetAxis("Mouse X"));
		input.drop = Input.GetAxis ("Drop");
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
		
		rb.rotation = Quaternion.Euler(pitch, yaw, input.turnLR * -game.seagullLimits.tilt);

		rb.position = new Vector3
			(
				Mathf.Clamp(rb.position.x, game.boundary.xMin, game.boundary.xMax),
				Mathf.Clamp(rb.position.y, game.sea.position.y, game.boundary.yMax),
				Mathf.Clamp(rb.position.z, game.boundary.zMin, game.boundary.zMax)
			);


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
					//that player loses points!
					GameObject.Find("Player" + (playerNumber % 2 + 1)).GetComponent<PlayerController>().count -= other.gameObject.GetComponent<Pickups>().pointVal;
					GameObject.Find("Player" + (playerNumber % 2 + 1)).GetComponent<PlayerController>().updateScore();
				}
            }
        }
        if(other.CompareTag("nest"))
        {
			if(holding && other.gameObject.GetComponent<NEST>().nestId == playerNumber)
			{
				holding = false;
				updateScore();
				other.gameObject.GetComponent<NEST>().addObject(heldObject.gameObject);
			}
        }
    }

    public void updateScore()
    {
		if(count < 0)
		{
			count = 0;
		}
        scoreText.text = "Score: " + count.ToString();
		if (count >= game.autowinScore)
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

	public bool isHolding()
	{
		return holding;
	}

	public void setHolding(bool state)
	{
		holding = state;
	}
}