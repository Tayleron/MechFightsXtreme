using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pathfinding;

public class MouseManager : MonoBehaviour {

	public MechStats selectedMech;
	public MechStats selectedMechLast;
  Hex selectedHex;
	public WeaponStats selectedWeapon;

	//req for camera drag controls
	bool isDraggingCamera = false;
	Vector3 LastMousePosition;
  Vector3 cameraTargetOffset;

	//Testing Pathing
	Seeker seeker;

	
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
				MouseOver_Attack();
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
	
	public Vector3 start;
	public Vector3 end;
	
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
				//set the starting node for pathfinding
				start = selectedMech.transform.localPosition;
				//change the parent of the selected mech to the newly clicked hex
				selectedMech.transform.parent = selectedHex.transform;
				//set the end point for pathfinding
				end = selectedMech.transform.localPosition;
				//call the moveUnit method for the selectedMech and pass it the pathfinding coords
				selectedMech.moveUnit(start, end);
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

	public bool CR_running;
	//method to perform an attack
	void attack()
	{
		selectedWeapon = selectedMech.equippedWeapon;
		if (selectedWeapon.rdyToFire)
		{
			//TODO: figure out how to set up rate of fire again. Something Changed and it's gone wrong
			//It looks like all the coroutines are being executed at once, instead of one at a time
			//it's causing the SelectedMech to be set to null
			for (int i = 0; i < selectedWeapon.weapon.rateOfFire; i++)
			{
        StartCoroutine(selection());
      }
		}
		else
		{
			Debug.Log(selectedWeapon + " not ready to fire.");
		}
	}
	
	//method to attack after a simple right click instead of using the GUI Attack button
	void MouseOver_Attack()
	{
		if(Input.GetButtonDown("Fire2") && selectedMech != null)
		{
			attack();
		}
	}

	//req for selection() to function
	private bool cancel = false;
	private int TpTest;

  IEnumerator selection()
  {
		CR_running = true;
		selectedMechLast = selectedMech;
		selectedMech = null;
    Debug.Log("Select Mech To Attack");
		yield return new WaitUntil(() => selectedMech != null || Input.GetKey(KeyCode.Escape) || cancel);
		//get the TP cost that will need to be spent based on the mech's Arm bools
		if (selectedMechLast.noArm)
		{
			TpTest = selectedWeapon.weapon.tpCost * 2;
		}
		else if (selectedMechLast.oneArm)
		{
			TpTest = selectedWeapon.weapon.tpCost + Mathf.RoundToInt(selectedWeapon.weapon.tpCost/2);
		}
		else
		{
			TpTest = selectedWeapon.weapon.tpCost;
		}
		
		//cancel if escape
		if (Input.GetKey(KeyCode.Escape))
		{
			selectedMech = selectedMechLast;
			Debug.Log("Attack Canceled");
			CR_running = false;
			yield break;
		}
		//cancel if TpTest is too low
		else if (selectedMechLast.mech.tpCurrent < TpTest)
		{
			selectedMech = selectedMechLast;
			cancel = true;
			cancel = false;
			Debug.Log("TP too low");
      CR_running = false;
      yield break;
		}
		//if a new mech is selected, attack it
		else if (selectedMech != null && selectedMech != selectedMechLast)
		{
			//spend the TP required to attack with the selected weapon
			selectedMechLast.spendTP(selectedWeapon.weapon.tpCost);
			//call the weapon's method of attack, passing it the attacked mech
			selectedWeapon.atkMech(selectedMech);
			//reselect  original mech
			selectedMech = selectedMechLast;
      CR_running = false;
      yield break;
		}
  }

	//temp GUI element to show selected mech stats + buttons
  void OnGUI()
  {
    if (selectedMech != null)
		{
			GUI.Label(new Rect(10, 20, 100, 40), "Mech Selected: " + selectedMech.mech.name);
			GUI.Label(new Rect(10, 55, 100, 25), selectedMech.mech.hpHead + " Head HP");			
			GUI.Label(new Rect(10, 70, 100, 25), selectedMech.mech.hpTorso + " Torso HP");			
			GUI.Label(new Rect(10, 85, 100, 25), selectedMech.mech.hpArm + " Arm HP");
			GUI.Label(new Rect(10, 100, 100, 25), selectedMech.mech.hpLeg + " Leg HP");
			GUI.Label(new Rect(10, 115, 100, 25), selectedMech.mech.shieldPoints + " Shields");
			GUI.Label(new Rect(10, 130, 100, 25), selectedMech.mech.tpCurrent + "/" + selectedMech.mech.tpMax + " TP");
			if(GUI.Button(new Rect(10, 250, 100, 25), "Reset Health")) 
			{
				selectedMech.repairAll();
			}
			if(GUI.Button(new Rect(10, 280, 100, 25), "Reset Shield")) 
			{
				selectedMech.repairShield();
			}
			if(GUI.Button(new Rect(10, 310, 100, 25), "Restore TP")) 
			{
        selectedMech.restoreTP();
			}
			if(GUI.Button(new Rect(10, 340, 100, 25), "Attack")) 
			{
        attack();
			}
    }
		if (selectedHex != null)
		{
    	GUI.Label(new Rect(10, 0, 100, 20), selectedHex.name + " selected");
  	}
  }
}

		

