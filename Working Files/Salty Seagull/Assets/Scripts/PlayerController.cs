using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float moveSpeed, rotationLR = 2.0f, rotationUD = 2.0f;
	private float yaw = 0.0f, pitch = 0.0f;
	public Text scoreText, winText;

	private Rigidbody rb;
	private int count;

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
		pitch -= rotationUD * Input.GetAxis("Mouse Y");

		Vector3 movement = new Vector3 (0.0f, 0.0f, moveVertical);

		rb.AddRelativeForce (movement * moveSpeed);
		transform.eulerAngles = new Vector3 (pitch, yaw, 0);
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