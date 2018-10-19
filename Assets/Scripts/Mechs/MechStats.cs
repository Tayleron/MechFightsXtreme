using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechStats : MonoBehaviour {

	public Mech mech;
	public GameObject currentHex;

	private int x;
	private int y;
	
	// Use this for initialization
	void Start () {
		
		setCurrentHex();
	
    Debug.Log("My name is: " + mech.mechName +
		" and my movement speed is: " + mech.movementSpeedInHexes + ". I have: "
		+ mech.hpTorso + " Torso Hp remaining." + " And I am located at: " + x + ", " + y);
	}

	void setCurrentHex() 
	{
    currentHex = transform.parent.gameObject;
    x = currentHex.GetComponent<Hex>().x;
    y = currentHex.GetComponent<Hex>().y;
  }

	void Update() {
			
	}


}
