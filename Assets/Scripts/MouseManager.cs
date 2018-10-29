using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {

	MechStats selectedMech;
  Hex selectedHex;

	//req for camera drag controls
	bool isDraggingCamera = false;
	Vector3 LastMousePosition;
  Vector3 cameraTargetOffset;

	
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
        takeDamage(ourHitObject);
      }
    }
		
		//add scroll wheel zoom
		float scrollAmount = -Input.GetAxis("Mouse ScrollWheel");
  	float cameraMinHeight = 10f;
  	float cameraMaxHeight = 60f;
		
    Vector3 dir = Camera.main.transform.position - hitPos;
    Vector3 p = Camera.main.transform.position;

		if (scrollAmount > 0 || p.y > cameraMinHeight)
		{
      cameraTargetOffset += dir * scrollAmount;
		}
		
		Vector3 lastCameraPosition = Camera.main.transform.position;
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.transform.position + cameraTargetOffset, Time.deltaTime * 5f);
		cameraTargetOffset -= Camera.main.transform.position - lastCameraPosition;
		
		p = Camera.main.transform.position;
    //set limits to the zoom in/out
    if (p.y < cameraMinHeight) {
				p.y = cameraMinHeight;
		}
		if (p.y > cameraMaxHeight) {
				p.y = cameraMaxHeight;
		}
		//set camera position to new zoomed position
		Camera.main.transform.position = p;
		
	}
	
	//Click on Hex
  void MouseOver_Hex(GameObject ourHitObject) 
	{
		if (Input.GetButtonDown("Fire1")) 
		{
			//select the Hex you click on
			selectedHex = ourHitObject.GetComponentInChildren<Hex>();
      // Debug.Log(selectedHex.name);
			//move the selected mech
			if (selectedMech != null && selectedMech.mech.movementRemaining >= selectedMech.mech.movementCostPerHex)
			{
				//change the parent of the selected mech to the newly clicked hex
				selectedMech.transform.parent = selectedHex.transform;
				//move the mech to the new hex via the mech's own method
				selectedMech.moveUnit();
				//deselect the mech
				selectedMech = null;
			}
    }
  }
	//Click on Mech
	void MouseOver_Mech(GameObject ourHitObject) 
	{
		if (Input.GetButtonDown("Fire1")) 
		{
			//select the mech you click on
			selectedMech = ourHitObject.GetComponent<MechStats>();
			Debug.Log(selectedMech.mech.name + " Selected. " + selectedMech.mech.hpTorso + " Torso HP remaining." );
		}
	}
	//this will be changed depending on the weapon used and each weapon will 
	//probably have some sort of damage get'er method within itself
	public int dmgToTake = 2;
	//req to randomize hit locations
	public float randHit;
	//req to set hit location
  string hitLoc;
	//figure out what location was randomly hit
	//maybe I could conbine this with the damaging if method? Far future thing.
  public void DetermineHitLoc(float randHit)
  {
    if (randHit > 0.9f && selectedMech.mech.hpHead > 0) {
      hitLoc = "Head";
    }	else if (randHit > 0.6f && selectedMech.mech.hpTorso > 0) {
      hitLoc = "Torso";
    } else if (randHit > 0.4f && selectedMech.mech.hpArm > 0)	{
      hitLoc = "Arm";
    } else if (randHit > 0.2f && selectedMech.mech.hpLeg > 0)	{
      hitLoc = "Leg";
    } else {
      hitLoc = "miss";
    }
  }
  
	//simple damage test to hurt the mech when "fire2" (right click) is pressed
	void takeDamage(GameObject ourHitObject)
	{
		if (Input.GetButtonDown("Fire2") && selectedMech != null)
		{
      //location to damage
      randHit = Random.value;
			DetermineHitLoc(randHit);
      //deal the damage to the proper location
			if (hitLoc == "Head") {
        selectedMech.mech.hpHead -= dmgToTake;
      } else if (hitLoc == "Torso") {
        selectedMech.mech.hpTorso -= dmgToTake;
      } else if (hitLoc == "Arm") {
        if (selectedMech.mech.hpArm > 0) {
          selectedMech.mech.hpArm -= dmgToTake;
        } else {
          hitLoc = "Torso";
          selectedMech.mech.hpTorso -= dmgToTake;
        }
      } else if (hitLoc == "Leg") {
        if (selectedMech.mech.hpLeg > 0) {
          selectedMech.mech.hpLeg -= dmgToTake;
        } else {
          hitLoc = "Torso";
          selectedMech.mech.hpTorso -= dmgToTake;
        }
      } else if (hitLoc == "miss") {
        hitLoc = "not";
      }

      Debug.Log(selectedMech.mech.name + " " + hitLoc + " damaged.");
			Debug.Log(selectedMech.mech.hpHead + " head HP remaining.");
			Debug.Log(selectedMech.mech.hpTorso + " torso HP remaining.");
			Debug.Log(selectedMech.mech.hpArm + " arm HP remaining.");
			Debug.Log(selectedMech.mech.hpLeg + " leg HP remaining.");

			//check to see if the mech has been disarmed
      if (selectedMech.mech.hpArm <= 0)
			{
				Debug.Log(selectedMech.mech.name + " arm destroyed. TP cost to fire weapons increased.");
				//TODO: increase TP cost logic
			}

			//check to see if the mech is legged
			if (selectedMech.mech.hpLeg <= 0)
			{
				Debug.Log(selectedMech.mech.name + " leg destroyed. Movement speed reduced.");
				//TODO: reduce movement logic
			}

      //check to see if mech is destroyed
      if (selectedMech.mech.hpTorso <= 0 || selectedMech.mech.hpHead <= 0)
      { //TODO: mech destruction logic
        Debug.Log(selectedMech.mech.name + " destroyed. " + "Health restored to max.");
				//restore to max HP
				selectedMech.mech.hpTorso = selectedMech.mech.hpTorsoMax;
				selectedMech.mech.hpHead = selectedMech.mech.hpHeadMax;
				selectedMech.mech.hpArm = selectedMech.mech.hpArmMax;
				selectedMech.mech.hpLeg = selectedMech.mech.hpLegMax;
				//deselect mech
				selectedMech = null;
			}	
		}
	}	
}

		

