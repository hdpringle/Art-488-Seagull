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
    private float moveVertical, turnUD, turnLR;

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

        moveVertical = Input.GetAxis("Vertical");

		if ((Input.GetKeyDown(("joystick 1 button " + 5.ToString()))) || Input.GetKeyDown(KeyCode.Space) && holding)
		{
			holding = false;
			heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
		}

		turnUD = FABS ((Input.GetAxis ("Fire3") - Input.GetAxis ("Fire2")), Input.GetAxis ("Mouse Y"));
		turnLR = FABS (Input.GetAxis("Horizontal"), Input.GetAxis("Mouse X"));

        yaw += limits.rotationLR * turnLR;
        pitch -= limits.rotationUD * turnUD;

        pitch = Mathf.Clamp(pitch, -limits.upAngle, limits.downAngle);

        Vector3 movement = -transform.InverseTransformDirection(rb.velocity) * limits.antiDrift;
        if (moveVertical > 0 || (moveVertical < 0 && movement.z < 0))
        {
            movement.z = moveVertical * limits.accelSpeed;
        }
        else
        {
            movement.z = movement.z / limits.antiDrift * limits.glideDecel;
        }

        rb.AddRelativeForce(movement);

        // rb.MoveRotation (rb.rotation * Quaternion.Euler( new Vector3 (pitch, yaw, 0)));
        rb.rotation = Quaternion.Euler(pitch, yaw, turnLR * -limits.tilt);
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
            heldObject.position = transform.FindChild("MountPoint").transform.position;
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
            count++;
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