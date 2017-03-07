using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, yMax, zMin, zMax;
}

[System.Serializable]
public class SeagullLimits
{
	public float upAngle, downAngle, accelSpeed, glideDecel, antiDrift, rotationLR, rotationUD, maxSpeed, tilt;
}

[System.Serializable]
public enum Steering
{
	KEYS, MOUSE, JOYSTICK
}

public class InputValues
{
	public float moveVertical, turnUD, turnLR;
}

public class PlayerController : MonoBehaviour
{

    public Text scoreText, winText, warnText, timer;
    public int requiredScore = 10;

    public Boundary boundary;
    public SeagullLimits limits;

    public Transform sea;
	public GameController gameController;

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
    }

    // Called for physics
    void FixedUpdate()
    {
		if (!gameController.GameStarted () || gameController.GameEnded () || gameController.isPaused ())
		{
			return;
		}

		input.moveVertical = Input.GetAxis("Vertical");

		if ((Input.GetKeyDown(("joystick 1 button " + 5.ToString()))) || Input.GetKeyDown(KeyCode.Space) && holding)
		{
			holding = false;
			heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
		}

		input.turnUD = FABS ((Input.GetAxis ("Fire3") - Input.GetAxis ("Fire2")), Input.GetAxis ("Mouse Y"));
		input.turnLR = FABS (Input.GetAxis("Horizontal"), Input.GetAxis("Mouse X"));

		yaw += limits.rotationLR * input.turnLR;
		pitch -= limits.rotationUD * input.turnUD;

        pitch = Mathf.Clamp(pitch, -limits.upAngle, limits.downAngle);

        Vector3 movement = -transform.InverseTransformDirection(rb.velocity) * limits.antiDrift;
		if (input.moveVertical > 0 || (input.moveVertical < 0 && movement.z < 0))
        {
			movement.z = input.moveVertical * limits.accelSpeed;
        }
        else
        {
            movement.z = movement.z / limits.antiDrift * limits.glideDecel;
        }

        rb.AddRelativeForce(movement);

        // rb.MoveRotation (rb.rotation * Quaternion.Euler( new Vector3 (pitch, yaw, 0)));
		rb.rotation = Quaternion.Euler(pitch, yaw, input.turnLR * -limits.tilt);
        // rb.rotation = Quaternion.Euler (pitch, yaw, 0);

        rb.position = new Vector3
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            Mathf.Clamp(rb.position.y, sea.position.y, boundary.yMax),
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );


        //move the held object with you
        if(holding)
        {
			/*8
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