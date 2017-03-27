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
	public Vector3 startingRotation;
    // Use this for initialization
    void Start()
    {
        gravityActive = false;
		offset = transform.FindChild("MountPoint").transform.position;
		startingRotation = transform.eulerAngles;
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
		if (!other.CompareTag("pickup"))
		{
			gravityActive = false;
		}
		if (other.CompareTag("nest"))
		{
			//only give points to the player for dropping in their own nest
			if (other.gameObject.GetComponent<NEST>().nestId == heldByPlayer)
			{
				gravityActive = true;
				GameObject.Find(playerName).GetComponent<PlayerController>().setHolding (false);
				other.gameObject.GetComponent<NEST>().addObject(this.gameObject);
			}
		}
	}
}
