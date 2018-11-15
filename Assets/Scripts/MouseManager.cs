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
		if (EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) 
		{
			Debug.Log("check");
			return;
		} else
		{	
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


  //method to attack after a simple right click instead of using the GUI Attack button
  void MouseOver_Attack()
  {
    if (Input.GetButtonDown("Fire2") && selectedMech != null)
    {
      attack();
    }
  }

	//used to set how many shots you get based on the weapon's rate of fire
	private int shots;
	//req to test if the weapon has enough TP to fire
  private int TpTest;

  //method to perform an attack
  void attack()
	{
		selectedWeapon = selectedMech.equippedWeapon;
		shots = selectedWeapon.shotsRemaining;
		Debug.Log(shots);
		if (selectedWeapon.rdyToFire && shots > 0)
		{
			//test if TP is high enough to use the weapon
      if (selectedMech.noArm)
      {
        TpTest = selectedWeapon.weapon.tpCost * 2;
      }
      else if (selectedMech.oneArm)
      {
        TpTest = selectedWeapon.weapon.tpCost + Mathf.RoundToInt(selectedWeapon.weapon.tpCost / 2);
      }
      else
      {
        TpTest = selectedWeapon.weapon.tpCost;
      }

			//if the test fails, log, if it passes, go into coroutine
			if (selectedMech.mech.tpCurrent < TpTest)
			{
				Debug.Log("TP too low");
			}
			else
			{
			StartCoroutine(selection());
			}
		}
		else
		{
			Debug.Log(selectedWeapon + " not ready to fire.");
		}
	}

  IEnumerator selection()
  {
		selectedMechLast = selectedMech;
		selectedMech = null;
    Debug.Log("Select Mech To Attack");
		yield return new WaitUntil(() => selectedMech != null || Input.GetKey(KeyCode.Escape));
		
		//cancel if escape
		if (Input.GetKey(KeyCode.Escape))
		{
			selectedMech = selectedMechLast;
			Debug.Log("Attack Canceled");
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
      shots--;
			selectedWeapon.shotsRemaining = shots;
			Debug.Log(shots);
			if (shots > 0)
			{
        yield return StartCoroutine(selection());
      }
			else
			{
				selectedWeapon.rdyToFire = false;
				yield break;
			}
		}
  }
	//simple statement to inform of readiness
	private string rdyOrNot;

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
			
			//if the equipped Weapon is ready or not
			if (selectedMech.equippedWeapon.rdyToFire)
			{
				rdyOrNot = ": Ready";
			}
			else
			{
				rdyOrNot = ": Not Ready";
			}

			GUI.Label(new Rect(10, 145, 170, 40), "Equipped Weapon:          " + selectedMech.equippedWeapon.name + rdyOrNot + " "
																						+ selectedMech.equippedWeapon.shotsRemaining + "/" 
																						+ selectedMech.equippedWeapon.weapon.rateOfFire);
			
			if (selectedMech.wep00)
			{
				if(GUI.Button(new Rect(10, 190, 30, 25), "00")) 
				{
					selectedMech.equipWeapon("00");
				}
			}
			if (selectedMech.wep01)
			{
				if(GUI.Button(new Rect(40, 190, 30, 25), "01")) 
				{
					selectedMech.equipWeapon("01");
				}
			}
			if (selectedMech.wep02)
			{
				if(GUI.Button(new Rect(70, 190, 30, 25), "02")) 
				{
					selectedMech.equipWeapon("02");
				}
			}
			if (selectedMech.wep03)
			{
				if(GUI.Button(new Rect(100, 190, 30, 25), "03")) 
				{
					selectedMech.equipWeapon("03");
				}	
			}
			if (selectedMech.wep04)
			{
				if(GUI.Button(new Rect(130, 190, 30, 25), "04")) 
				{
					selectedMech.equipWeapon("04");
				}
			}
			if (selectedMech.wep05)
			{
				if(GUI.Button(new Rect(10, 220, 30, 25), "05")) 
				{
					selectedMech.equipWeapon("05");
				}
			}
			if(selectedMech.wep06)
			{	
				if(GUI.Button(new Rect(40, 220, 30, 25), "06")) 
				{
					selectedMech.equipWeapon("06");
				}
			}	
			if (selectedMech.wep07)
			{	if(GUI.Button(new Rect(70, 220, 30, 25), "07")) 
				{
					selectedMech.equipWeapon("07");
				}
			}
			if (selectedMech.wep08)
			{
				if(GUI.Button(new Rect(100, 220, 30, 25), "08")) 
				{
					selectedMech.equipWeapon("08");
				}
			}
			if (selectedMech.wep09)
			{
				if (GUI.Button(new Rect(130, 220, 30, 25), "09"))
				{
          selectedMech.equipWeapon("09");
        }
			}
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
			if(GUI.Button(new Rect(10, 370, 100, 25), "Reload")) 
			{
        selectedMech.reloadWeapons();
			}
      GUI.Label(new Rect(120, 300, 100, 100), "Weapons: " + selectedMech.listOfWeapons);
    }
		if (selectedHex != null)
		{
    	GUI.Label(new Rect(10, 0, 100, 20), selectedHex.name + " selected");
  	}
  }
}

		

