using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MechStats : MonoBehaviour {

	public Mech mech;
	public GameObject currentHex;
	public Renderer rend;

	
	public float modelHeight;
	private string OGName;

	private int x;
	private int y;
	
	// Use this for initialization
	void Start () {
		//get original name of the mech
		OGName = mech.name;
		//create unique instance of mech (clone)
    mech = Instantiate(mech);
		//rename the clone to the orginal name
		mech.name = OGName;
		
		//get the full height of the model
		modelHeight = mech.MechPrefab.GetComponent<Renderer>().bounds.size.y;

    Debug.Log("My name is: " + mech.mechName +
		" and I have: " + mech.movementRemaining + " movement remaining. I have: "
		+ mech.hpTorso + " Torso Hp remaining." + " And I am located at: " + x + ", " + y);
	}

	void setCurrentHex() 
	{ 
		//get current hex and assign x and y values 
    currentHex = transform.parent.gameObject;
    x = currentHex.GetComponent<Hex>().x;
    y = currentHex.GetComponent<Hex>().y;
  }

	public void moveUnit()
	{
		//set the position of the model to stand on the top of its parent hex
    transform.localPosition = new Vector3(0, 0, modelHeight / 1.5f);
	}

	

	void Update() {

    setCurrentHex();

  }


}
