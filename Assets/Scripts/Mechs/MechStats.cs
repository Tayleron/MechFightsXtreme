using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechStats : MonoBehaviour {

	public Mech mech;
	// Use this for initialization
	void Start () {

		Debug.Log("My name is: " + mech.mechName +
		" and my movement speed is: " + mech.movementSpeedInHexes + ". I have: "
		+ mech.hpTorso + " Torso Hp remaining.");
	}
}
