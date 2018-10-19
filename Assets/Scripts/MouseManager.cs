﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {

	MechStats selectedMech;
  Hex selectedHex;

	//req for camera drag controls
	bool isDraggingCamera = false;
	Vector3 LastMousePosition;
	//zoom levels
  public float cameraMinHeight = 10f;
  public float cameraMaxHeight = 50f;
	
	void Start() {
			
	}
	// Update is called once per frame
	void Update () {
		//Do nothing if mousing over UI buttons
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}
		//Debug.Log("Mouse Position" + Input.mousePosition);

		//Get a ray from the camera to the board
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		//req for selections
		RaycastHit hitInfo;
		
		//req for drag controls
		float rayLength = (ray.origin.y / ray.direction.y);
		Vector3 hitPos = ray.origin - (ray.direction * rayLength);

    //Camera drag on/off
		if (Input.GetMouseButtonDown(0))
    {
      isDraggingCamera = true;
			LastMousePosition = hitPos;
    } 
		else if (Input.GetMouseButtonUp(0))
		{
			isDraggingCamera = false;	
		}

		//Camera drag
		if(isDraggingCamera)
		{
			Vector3 diff = LastMousePosition - hitPos;
			Camera.main.transform.Translate(diff, Space.World);
		}
    //Mouse overs to provide info
		if (Physics.Raycast(ray, out hitInfo))
    {
      GameObject ourHitObject = hitInfo.collider.transform.gameObject;

			//Mouse over a Hex
      if (ourHitObject.GetComponent<Hex>() != null)
      {
        MouseOver_Hex(ourHitObject);
      }
			//Mouse over a mech
      else if (ourHitObject.GetComponent<MechStats>() != null)
      {
        MouseOver_Mech(ourHitObject);
      }
    }

		//add scroll wheel zoom
		float scrollAmount = -Input.GetAxis("Mouse ScrollWheel");
		if(Mathf.Abs(scrollAmount) > 0.01f) {
			Vector3 dir = Camera.main.transform.position - hitPos;
			Camera.main.transform.Translate(dir * scrollAmount, Space.World);
      Vector3 p = Camera.main.transform.position;

			//set limits to the zoom in/out
			if (p.y < cameraMinHeight)
			{
				p.y = cameraMinHeight;
			}
			if (p.y > cameraMaxHeight)
			{
				p.y = cameraMaxHeight;
			}
			Camera.main.transform.position = p;

			//TODO: Add a method to prevent scrolling at a certain point (high and low points)
			//so that it doesn't keep trying to scroll at the max and min points. 
		}
	}
	
	//Click on Hex
  void MouseOver_Hex(GameObject ourHitObject) 
	{

		if (Input.GetMouseButtonDown(0)) 
		{
			selectedHex = ourHitObject.GetComponentInChildren<Hex>();
      Debug.Log(selectedHex.name);
    }
  }
	//Click on Mech
	void MouseOver_Mech(GameObject ourHitObject) {
		if(Input.GetMouseButtonDown(0)) {
			selectedMech = ourHitObject.GetComponent<MechStats>();
			Debug.Log(selectedMech.name);
		}
	}
}

		

