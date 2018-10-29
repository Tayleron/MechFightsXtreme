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
        MouseOver_tempTakeDmg();
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
	//temp method to test damage script
	void MouseOver_tempTakeDmg()
	{
		if(Input.GetButtonDown("Fire2") && selectedMech != null)
		{
			selectedMech.takeDamage();
			if (selectedMech.mech.hpTorso <= 0 || selectedMech.mech.hpHead <= 0)
			{
				selectedMech = null;
			}
		}
	}	

	//temp GUI element to show mech as selected
  void OnGUI()
  {
    if (selectedMech != null)
		{
			GUI.Label(new Rect(10, 20, 100, 40), "Mech Selected: " + selectedMech.mech.name);
			GUI.Label(new Rect(10, 55, 100, 25), selectedMech.mech.hpHead + " head HP");			
			GUI.Label(new Rect(10, 70, 100, 25), selectedMech.mech.hpTorso + " Torso HP");			
			GUI.Label(new Rect(10, 85, 100, 25), selectedMech.mech.hpArm + " Arm HP");
			GUI.Label(new Rect(10, 100, 100, 25), selectedMech.mech.hpLeg + " Leg HP");
			if(GUI.Button(new Rect(10, 130, 100, 25), "Reset Health")) 
			{
				selectedMech.repairAll();
			}
    }
		if (selectedHex != null)
		{
    	GUI.Label(new Rect(10, 0, 100, 20), selectedHex.name + " selected");
  	}
  }
}

		

