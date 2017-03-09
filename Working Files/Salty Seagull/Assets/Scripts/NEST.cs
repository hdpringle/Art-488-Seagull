using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEST : MonoBehaviour {
	public ArrayList itemsInNest;
	public int nestId;
	// Use this for initialization
	void Start ()
	{
		itemsInNest = new ArrayList();
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
		itemsInNest.Add(obj);
	}
	public void removeFromNest(GameObject obj)
	{
		itemsInNest.Remove(obj);
	}

}
