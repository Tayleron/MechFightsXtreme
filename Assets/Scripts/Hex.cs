﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {
	private Renderer HexMatRend;
	public Material[] HexMaterials;
	public GameObject ramp;
	//Elevation determines the height of the hex 
	//Use Elevation to determine cliffs and the color/graphics hexes should be assigned
	//isWater determines if the hex is a water hex or a land hex	
	public int Elevation = 1;
	public bool isWater = false;
	public bool isRamp = false;
 
	// Use this for initialization
	void Start () {
    HexMatRend = GetComponent<Renderer>();
    HexMatRend.enabled = true;
    HexMatRend.sharedMaterial = HexMaterials[0];
	}
	
	private void ElevationCheck() {
    if (Elevation == 0)
    {
      HexMatRend.sharedMaterial = HexMaterials[0];
      transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 2);
    }
    else if (Elevation == 1)
    {
      HexMatRend.sharedMaterial = HexMaterials[1];
      transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 4);
    }
    else //meaning Elevation must be 2
    {
      HexMatRend.sharedMaterial = HexMaterials[2];
      transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 6);
    }
	}
	private void WaterCheck() {
    if (isWater)
    {
      HexMatRend.sharedMaterial = HexMaterials[3];
      transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
    }
	}
	private void RampCheck() {
    if (isRamp && isWater == false && Elevation != 2)
    {
      ramp.GetComponent<Renderer>().enabled = true;
    }
    else
    {
      ramp.GetComponent<Renderer>().enabled = false;
    }
	}
	void Update()
	{	//Elevation
    ElevationCheck();
		//Check for Water
		WaterCheck();
		//Check for Ramp
		RampCheck();
	}
}

