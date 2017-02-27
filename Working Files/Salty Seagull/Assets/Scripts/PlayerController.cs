﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Steering steering;

    public Transform sea;

    private Rigidbody rb;
    public int count;
    private float yaw, pitch;
    public bool holding = false;
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
    }

    // Called for physics
    void FixedUpdate()
    {
        moveVertical = Input.GetAxis("Vertical");

        switch (steering)
        {
            case Steering.KEYS:
                turnUD = (Input.GetAxis("Fire3") - Input.GetAxis("Fire2"));
                turnLR = Input.GetAxis("Horizontal");
				if (Input.GetKeyDown(KeyCode.Space) && holding)
				{
					holding = false;
					heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
				}
				break;

            case Steering.MOUSE:
                turnUD = Input.GetAxis("Mouse Y");
                turnLR = Input.GetAxis("Mouse X");
                limits.tilt = 0;
				if (Input.GetKeyDown(KeyCode.Space) && holding)
				{
					holding = false;
					heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
				}
				break;

            case Steering.JOYSTICK:
                turnUD = -Input.GetAxis("Mouse Y");
                turnLR = Input.GetAxis("Mouse X");
                //press bumper to drop the object
                if (Input.GetKeyDown(("joystick 1 button " + 5.ToString())) && holding)
                {
                    holding = false;
                    heldObject.gameObject.GetComponent<Pickups>().gravityActive = true;
                }
                break;
        }

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
}