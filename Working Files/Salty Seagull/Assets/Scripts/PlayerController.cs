using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Xml.Linq;

public class InputValues
{
	public float moveForward, moveSideways, turnUD, turnLR, drop;
	public bool flyWalk;
}

public class PlayerController : MonoBehaviour
{
    public int playerNumber;

	private GameController game;
    private Rigidbody rb;
	private CharacterController charCon;
	private Animator animator;
    private float yaw, pitch, worldy, sinr, cosr;
    private bool holding, flying;
	protected InputValues input;
    Transform heldObject;

    void Start()
    {
		game = GameObject.Find("GameController").GetComponent<GameController>();

		rb = GetComponent<Rigidbody>();
		charCon = GetComponent<CharacterController> ();
		animator = GetComponent<Animator> ();
        yaw = transform.rotation.eulerAngles.y;
        pitch = transform.rotation.eulerAngles.x;
        rb.freezeRotation = true;
		holding = false;
		flying = true;
		input = new InputValues ();
	}

    // Called for physics
    void FixedUpdate()
    {

		GetControls();

		if (!game.GameStarted () || game.GameEnded () || game.isPaused ())
		{
			rb.velocity = Vector3.zero;
			return;
		}


		DoMotion ();
    }

	protected virtual void GetControls()
	{
		switch (playerNumber)
		{
		case 1:
			input.moveForward = Input.GetAxis("Forward");
			input.moveSideways = Input.GetAxis("Sideways");
			input.turnUD = Input.GetAxis("Vertical");
			input.turnLR = Input.GetAxis("Horizontal");
			input.drop = Input.GetAxis("Drop");
			input.flyWalk = Input.GetButtonDown("Crouch");
			break;

		default:
			input.moveForward = Input.GetAxis("P" + playerNumber + " Forward");
			input.moveSideways = Input.GetAxis("P" + playerNumber + " Sideways");
			input.turnUD = Input.GetAxis("P" + playerNumber +" Vertical");
			input.turnLR = Input.GetAxis("P" + playerNumber +" Horizontal");
			input.drop = Input.GetAxis("P" + playerNumber +" Drop");
			input.flyWalk = Input.GetButtonDown("P" + playerNumber +" Crouch");
			break;
		}
		input.turnUD *= (GameController.settings.inversions [playerNumber - 1] ? -1 : 1);
	}

	protected void DoMotion()
	{
		Vector3 speed;
		yaw += game.seagullLimits.rotationLR * input.turnLR;

		if (input.flyWalk)
		{
			if (flying)
			{
				pitch = 0;
				//enables walking controls
				charCon.enabled = true;
			}
			else
			{
				charCon.enabled = false;
			}
			flying = !flying;
			animator.SetTrigger("Falling");
		}

		if (flying)
		{
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

			speed = rb.velocity;

			//if we get to slow, start falling slowly
			if(speed.magnitude < 1)
			{
				rb.AddForce(new Vector3(0,-0.5f,0));
			}

		}
		else
		{ // walking
			rb.rotation = Quaternion.Euler (0, yaw, 0);
			worldy = game.DTR (transform.eulerAngles.y) ;
			sinr = Mathf.Sin (worldy);
			cosr = Mathf.Cos (worldy);

			charCon.Move (new Vector3(
				(cosr * input.moveSideways + sinr * input.moveForward) * game.seagullLimits.walkSpeed,
				-game.seagullLimits.walkGravity * Time.deltaTime,
				(sinr * -input.moveSideways + cosr * input.moveForward) * game.seagullLimits.walkSpeed
			));
		}

		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, game.boundary.xMin, game.boundary.xMax),
			Mathf.Clamp (rb.position.y, game.sea.position.y, game.boundary.yMax),
			Mathf.Clamp (rb.position.z, game.boundary.zMin, game.boundary.zMax)
		);

		//move the held object with you
		if(holding)
		{
			heldObject.position = transform.FindChild("MountPoint").transform.position - (heldObject.FindChild("MountPoint").transform.position - heldObject.position);
			heldObject.eulerAngles = heldObject.gameObject.GetComponent<Pickups>().startingRotation + transform.eulerAngles;
		}

		//we can only drop an object we are holding
		if (input.drop > 0 && holding)
		{
			holding = false;
			//give the pickup gravity
			heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
		}


		speed = rb.velocity;
		//gets the flight animations
		CheckAnimator(speed);
	}

	protected void CheckAnimator(Vector3 speed)
	{

		if (flying)
		{ 
			//increase speed of animations to match the speed of the seagull
			animator.speed = Mathf.Max((speed.magnitude) / 5, 1f);
		}
		else
		{
			animator.speed = Mathf.Max(speed.magnitude/5, 1f);
		}
		//will transition from wing flaps to gliding and walking to standing
		animator.SetFloat("Speed", Mathf.Max(input.moveForward, 0f));

	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("pickup"))
        {
			if (!holding)
			{
				//Check to see if the object we collided with is in our own nest
				if (!GameObject.Find("Nest" + playerNumber).GetComponent<NEST>().isInNest(other.gameObject))
				{
					//other.gameObject.SetActive(false);
					heldObject = other.transform;
					heldObject.gameObject.GetComponent<Pickups>().heldByPlayer = playerNumber;
					holding = true;
				}

				for (int i = 1; i <= game.numberOfNests; i++)
				{
					//if you stole this item, let the nest know its gone
					if (playerNumber != i && GameObject.Find("Nest" + i).GetComponent<NEST>().isInNest(other.gameObject))
					{
						GameObject.Find("Nest" + i).GetComponent<NEST>().removeFromNest(other.gameObject);
					}
				}
				if(game.filledPickups.Contains(other.gameObject.transform))
				{
					game.filledPickups.Remove(other.gameObject.transform);
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
		else if(other.CompareTag("Island"))
		{
			animator.SetTrigger("Walking");
		}
		print(other.tag);
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