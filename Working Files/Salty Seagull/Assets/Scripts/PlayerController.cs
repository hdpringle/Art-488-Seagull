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
	public float moveForward, moveSideways, turnUD, turnLR, drop, flyWalk;
}

public class PlayerController : MonoBehaviour
{
    public Text scoreText, winText;
	public GameController game;
	public int playerNumber;

    private Rigidbody rb;
	private CharacterController cc;
	private Animator animator;
    private float yaw, pitch, worldy, sinr, cosr;
    private bool holding, flying;
	protected InputValues input;
	private Vector3 speed;
    Transform heldObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
		cc = GetComponent<CharacterController> ();
		animator = GetComponent<Animator> ();
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
			input.moveSideways = Input.GetAxis ("P2 Sideways");
			input.turnUD = Input.GetAxis ("P2 Vertical");
			input.turnLR = Input.GetAxis ("P2 Horizontal");
			input.drop = Input.GetAxis ("P2 Drop");
			input.flyWalk = Input.GetAxis ("P2 Crouch");
			break;
		default:
			input.moveForward = Input.GetAxis ("Forward");
			input.moveSideways = Input.GetAxis ("Sideways");
			input.turnUD = Input.GetAxis ("Vertical");
			input.turnLR = Input.GetAxis ("Horizontal");
			input.drop = Input.GetAxis ("Drop");
			input.flyWalk = Input.GetAxis ("Crouch");
			break;
		}
	}

	protected void DoMotion()
	{
		yaw += game.seagullLimits.rotationLR * input.turnLR;

		if (input.flyWalk > 0)
		{
			if (flying)
			{
				pitch = 0;
				cc.enabled = true;
			}
			else
			{
				cc.enabled = false;
			}
			flying = !flying;
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

			cc.Move (new Vector3(
				(cosr * input.moveSideways + sinr * input.moveForward) * game.seagullLimits.walkSpeed,
				game.seagullLimits.walkGravity * Time.deltaTime,
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

		if (input.drop > 0 && holding)
		{
			holding = false;
			heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
		}

		speed = rb.velocity;
		//gets the flight animations
		CheckAnimator(speed);
	}
	
	protected void CheckAnimator(Vector3 speed)
	{
		if (speed.magnitude/5 > 1)
		{
			//increase speed of animations to match the speed of the seagull
			animator.speed = Mathf.Max((speed.magnitude) / 5, 0.75f);
		}
		else
		{
			animator.speed = 1;
		}
		//will transition from wing flaps to gliding
		animator.SetFloat("Speed", Mathf.Max(speed.magnitude, 0.75f));

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