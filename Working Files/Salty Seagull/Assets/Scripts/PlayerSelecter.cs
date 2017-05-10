using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelecter : MonoBehaviour
{ 

	const int NUMBER_OF_SEAGULLS = 4;//can update when more seagull skins come

	public int playerNumber;
	
	public int currentSeagull;//the seagull selected

	public bool chosen;//has the player chosen a seagull?

	private class InputValues
	{
		public float turnLR;
		public float select;
	}
	private InputValues input;
	// Use this for initialization
	void Start ()
	{
		input = new InputValues();
		chosen = false;

		//constantly calls an update function, every 0.25 seconds. Lets us get delays on the controller input.
		InvokeRepeating("BestUpdate", 0f, 0.25f);
	}

	void Update()
	{
		//set the pic to be the right seagull
		transform.FindChild("Image").GetComponentInChildren<Image>().material = Resources.Load("Materials/Seagull"+currentSeagull, typeof(Material)) as Material;

		//hit a, you chose
		if(input.select > 0)
		{
			chosen = true;
		}
		//hit b, you unchose
		else if(input.select < 0)
		{
			if (!chosen)
			{
				Scene myScene = SceneManager.GetSceneByName("MainMenu");
				if (!myScene.isLoaded)
				{
					SceneManager.LoadScene("MainMenu");
				}

				if (myScene.isLoaded && myScene.IsValid())
				{
					SceneManager.SetActiveScene(myScene);
				}
			}
			chosen = false;
		}

		//let the player see their choice
		if(chosen)
		{
			transform.FindChild("Text").GetComponent<Text>().text = "DONE!";
			transform.FindChild("Button").SetAsLastSibling();
		}
		else
		{
			transform.FindChild("Text").GetComponent<Text>().text = "Choosing";
			transform.FindChild("Button").SetAsFirstSibling();
		}
		

		//get the controls
		GetControls();
	}

	// Update is called once per frame
	public void BestUpdate()
	{
		//dont move if you chose
		if (!chosen)
		{
			//swipe right
			if (input.turnLR > 0)
			{
				currentSeagull++;
				if (currentSeagull > NUMBER_OF_SEAGULLS)
				{
					currentSeagull -= NUMBER_OF_SEAGULLS;
				}
			}
			//swipe left
			else if (input.turnLR < 0)
			{
				currentSeagull--;
				if (currentSeagull <= 0)
				{
					currentSeagull += NUMBER_OF_SEAGULLS;
				}
			}
		}
	}
	protected virtual void GetControls()
	{
		input.turnLR = Input.GetAxis("P" + playerNumber + " Sideways");
		input.select = Input.GetAxis("P" + playerNumber + " Submit");
	}
}
