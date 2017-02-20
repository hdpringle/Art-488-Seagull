using System.Collections;
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
	public float upAngle, downAngle, accelSpeed, decelSpeed, rotationLR, rotationUD, maxSpeed, tilt;
}

public class PlayerController : MonoBehaviour {

	public Text scoreText, winText, warnText, timer;
	public int requiredScore = 10;

	public Boundary boundary;
	public SeagullLimits limits;
	public Transform sea;

	private Rigidbody rb;
	private int count;
	private float yaw = 0.0f, pitch = 0.0f;
	private bool holding = false;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		count = 0;
		updateScore ();
		winText.text = "";
		rb.freezeRotation = true;
	}

	// Called for physics
	void FixedUpdate()
	{
		//float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		float turnUp = Input.GetAxis ("Fire3");
		float turnDown = Input.GetAxis ("Fire2");
		float turnLR = Input.GetAxis ("Horizontal");

		yaw += limits.rotationLR * turnLR;
		pitch -= limits.rotationUD * (turnUp - turnDown);

		pitch = Mathf.Clamp (pitch, -limits.upAngle, limits.downAngle);

		Vector3 movement = -transform.InverseTransformDirection (rb.velocity) * limits.decelSpeed;
		movement.z = moveVertical * limits.accelSpeed;

		rb.AddRelativeForce (movement);
		print (rb.GetRelativePointVelocity (rb.position));

		//rb.MoveRotation (rb.rotation * Quaternion.Euler( new Vector3 (pitch, yaw, 0)));
		rb.rotation = Quaternion.Euler (pitch, yaw, turnLR * -limits.tilt);

		rb.position = new Vector3
		(
			Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
			Mathf.Clamp(rb.position.y, sea.position.y, boundary.yMax),
			Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
		);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("pickup"))
		{
			if (holding)
			{
				// Display warning message
			}
			else
			{
				other.gameObject.SetActive(false);
				holding = true;
			}
		}
		else if (other.gameObject.CompareTag("nest"))
		{
			holding = false;
			count++;
			updateScore ();
		}
	}

	void updateScore()
	{
		scoreText.text = "Score: " + count.ToString ();
		if (count >= requiredScore)
		{
			winText.text = "YOU WIN!!";
		}
	}

	float DragForce()
	{
		return 0;
	}
}