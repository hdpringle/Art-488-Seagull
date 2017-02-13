using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;
	public Text scoreText;
	public Text winText;

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
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

		rb.AddForce(movement * speed);
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