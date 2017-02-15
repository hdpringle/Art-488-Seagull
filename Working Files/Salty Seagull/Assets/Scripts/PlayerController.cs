using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, yMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 10.0f, rotationLR = 2.0f, rotationUD = 2.0f;
	public Text scoreText, winText;

	public Boundary boundary;
	public Transform sea;

	private Rigidbody rb;
	private int count;
	private float yaw = 0.0f, pitch = 0.0f;

	void Start () {
		rb = GetComponent<Rigidbody>();
		count = 0;
		updateScore ();
		winText.text = "";
	}

	// Called for physics
	void FixedUpdate()
	{
		//float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		yaw += rotationLR * Input.GetAxis("Horizontal");
		pitch -= rotationUD * Input.GetAxis("Jump") - rotationUD * Input.GetAxis("Fire2");

		Vector3 movement = new Vector3 (0.0f, 0.0f, moveVertical);

		rb.AddRelativeForce (movement * moveSpeed);
		transform.eulerAngles = new Vector3 (pitch, yaw, 0);

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
			other.gameObject.SetActive(false);
			count++;
			updateScore ();
		}
	}

	void updateScore()
	{
		scoreText.text = "Score: " + count.ToString ();
		if (count >= 12)
		{
			winText.text = "YOU WIN!!";
		}
	}
}