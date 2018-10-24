using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechStats : MonoBehaviour {

	public Mech mech;
	public GameObject currentHex;
	public Renderer rend;

	public float modelHeight;

	private int x;
	private int y;
	private float yHeight;
	
	// Use this for initialization
	void Start () {		
		modelHeight = mech.MechPrefab.GetComponent<Renderer>().bounds.size.y;
    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + (modelHeight/ 1.5f));
	
    Debug.Log("My name is: " + mech.mechName +
		" and my movement speed is: " + mech.movementSpeedInHexes + ". I have: "
		+ mech.hpTorso + " Torso Hp remaining." + " And I am located at: " + x + ", " + y);
	}

	void setCurrentHex() 
	{ 
		//get current hex and assign x and y values 
    currentHex = transform.parent.gameObject;
    x = currentHex.GetComponent<Hex>().x;
    y = currentHex.GetComponent<Hex>().y;
		yHeight = currentHex.GetComponent<Hex>().HexModelYHeight;
  }

	void Update() {
    setCurrentHex();
	}


}
