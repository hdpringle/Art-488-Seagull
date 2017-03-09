using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{

    //might use in the future?
    public int pointVal;
    public float spawnTime;

	//I set up a bool here so gravity only effects objects when they are in the air.
    public bool gravityActive;
	public float gravityStrength = 0.075f;
	public int heldByPlayer = 0;
	public Vector3 offset;
    // Use this for initialization
    void Start()
    {
        gravityActive = false;
		pointVal = 1;
		offset = transform.FindChild("MountPoint").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gravityActive)
        {
			transform.Translate(Vector3.down* gravityStrength , Space.World);
		}
    }

	//we dont need gravity once an object has hit a surface
    private void OnTriggerEnter(Collider other)
    {
		string playerName = "Player";
		//Can allow this to work for multiplayer
		if(heldByPlayer != 0)
		{
			playerName = playerName + heldByPlayer;
		}

		gravityActive = false;
		if (other.CompareTag("nest"))
		{
			//only give points to the player for dropping in their own nest
			if (other.gameObject.GetComponent<NEST>().nestId == heldByPlayer)
			{
				gravityActive = true;
				GameObject.Find(playerName).GetComponent<PlayerController>().holding = false;
				GameObject.Find(playerName).GetComponent<PlayerController>().count+=pointVal;
				GameObject.Find(playerName).GetComponent<PlayerController>().updateScore();
			}
		}
	}
}
