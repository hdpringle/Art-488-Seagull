using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEST : MonoBehaviour {
	public ArrayList itemsInNest;
	public int nestId;
	private int count;
	private AudioSource source1;
	private AudioSource source2;

	// Use this for initialization
	void Start ()
	{
		itemsInNest = new ArrayList();
		source1 = GetComponents<AudioSource>()[0];
		source2 = GetComponents<AudioSource>()[1];

		//transform.FindChild("Beacon").GetComponent<MeshRenderer>().material.color.a = 0.25f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public bool isInNest(GameObject obj)
	{
		if(itemsInNest.Contains(obj))
		{
			return true;
		}
		return false;
	}

	public void addObject(GameObject obj)
	{
		int rand = Random.Range(0,2);
		if (rand > 0)
		{
			source1.PlayOneShot(source1.clip);
		}
		else
		{
			source2.PlayOneShot(source2.clip);
		}
		itemsInNest.Add(obj);
		count += obj.GetComponent<Pickups> ().pointVal;
	}

	public void removeFromNest(GameObject obj)
	{
		itemsInNest.Remove(obj);
		count -= obj.GetComponent<Pickups> ().pointVal;
	}

	public int GetScore()
	{
		return count;
	}
}
