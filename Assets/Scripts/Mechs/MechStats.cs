using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class MechStats : MonoBehaviour {

  public GridManager GM;

	public Mech mech;
	public GameObject currentHex;
	public Renderer rend;

	public Weapon weapon;
	public WeaponStats equippedWeaponStats;
  public GameObject currentWeaponGO;
  public Transform mechTrans;
	
	
	public float modelHeight;
  public float wepModelHeight;
	private string OGName;

	private int x;
	private int y;

	
  void Start()
  {
    //testing Pathing
    seeker = GetComponent<Seeker>();

    //get original name of the mech
    OGName = mech.name;
		//create unique instance of mech (clone)
    mech = Instantiate(mech);
		//rename the clone to the orginal name
		mech.name = OGName;
    mechTrans = GetComponent<Transform>();


		//get the full height of the model
		modelHeight = mech.MechPrefab.GetComponent<Renderer>().bounds.size.y;
    moveUnit(Vector3.zero, Vector3.zero);
    setCurrentHex();

    Debug.Log("My name is: " + mech.mechName +
		" and I have: " + mech.movementRemaining + " movement remaining. I have: "
		+ mech.hpTorso + " Torso Hp remaining." + " And I am located at: " + x + ", " + y);


    //testing weapon loading
    loadWeapon("Autocannon");
    loadWeapon("Laser");
    loadWeapon("Flamer");
    loadWeapon("Railgun");
    setWepBools();

    //equip a default weapon
    if (WeaponsList.ContainsKey("00"))
    {
      equipWeapon("00");
    }
    //build the list of weapons to display in the GUI
    listWeapons();

    // log to return my dictionary
    // foreach (var key in WeaponsList)
    // {
    //     Debug.Log(key);
    // }

  }

  //References to WeaponPrefabs
  public GameObject Autocannon;
  public GameObject EnergyBlade;
  public GameObject Flamer;
  public GameObject GaussRifle;
  public GameObject Laser;
  public GameObject Railgun;
  public GameObject Rockets;
  //req for determineWeapon()
  public GameObject weaponPrefabToSpawn;

  //used to choose which weapon will be spawned
  void determineWeapon(string weaponToAdd)
  {
    if (weaponToAdd == "Autocannon")
    {
      weaponPrefabToSpawn = Autocannon;
    }
    else if (weaponToAdd == "EnergyBlade")
    {
      weaponPrefabToSpawn = EnergyBlade;
    }
    else if (weaponToAdd == "Flamer")
    {
      weaponPrefabToSpawn = Flamer;
    }
    else if (weaponToAdd == "GaussRifle")
    {
      weaponPrefabToSpawn = GaussRifle;
    }
    else if (weaponToAdd == "Laser")
    {
      weaponPrefabToSpawn = Laser;
    }
    else if (weaponToAdd == "Railgun")
    {
      weaponPrefabToSpawn = Railgun;
    }
    else if (weaponToAdd == "Rockets")
    {
      weaponPrefabToSpawn = Rockets;
    }
  }

  //Dictionary holding all weapons held by a mech
  //access with wepID to get a weapon GameObject
  private Dictionary<string, GameObject> WeaponsList = new Dictionary<string, GameObject>();
  public bool wep00 = false;
  public bool wep01 = false;
  public bool wep02 = false;
  public bool wep03 = false;
  public bool wep04 = false;
  public bool wep05 = false;
  public bool wep06 = false;
  public bool wep07 = false;
  public bool wep08 = false;
  public bool wep09 = false;

  private void setWepBools()
  {
    if (WeaponsList.ContainsKey("00"))
    {
      wep00 = true;
    }
    if (WeaponsList.ContainsKey("01"))
    {
      wep01 = true;
    }
    if (WeaponsList.ContainsKey("02"))
    {
      wep02 = true;
    }
    if (WeaponsList.ContainsKey("03"))
    {
      wep03 = true;
    }
    if (WeaponsList.ContainsKey("04"))
    {
      wep04 = true;
    }
    if (WeaponsList.ContainsKey("05"))
    {
      wep05 = true;
    }
    if (WeaponsList.ContainsKey("06"))
    {
      wep06 = true;
    }
    if (WeaponsList.ContainsKey("07"))
    {
      wep07 = true;
    }
    if (WeaponsList.ContainsKey("08"))
    {
      wep08 = true;
    }
    if (WeaponsList.ContainsKey("09"))
    {
      wep09 = true;
    }
  }


  //simple ID system value
  int id = 0;
  //req to set weapon IDs
  string wepID;

  //add weapon to list containing all the weapons available to this mech
  void loadWeapon(string weaponToAdd)
  {
    //determine the type of weapon
    determineWeapon(weaponToAdd);
    OGName = weaponPrefabToSpawn.name;
    currentWeaponGO = Instantiate(weaponPrefabToSpawn) as GameObject;
    currentWeaponGO.name = OGName;
    currentWeaponGO.transform.SetParent(mechTrans);
    wepModelHeight = currentWeaponGO.GetComponent<Renderer>().bounds.size.y;
    currentWeaponGO.transform.position = new Vector3(mechTrans.position.x, (modelHeight / 2) + (wepModelHeight / 2), mechTrans.position.z);
    wepID = string.Format("{0}", id.ToString("00"));
    WeaponsList.Add(wepID, currentWeaponGO);
    id++;
  }

  public void equipWeapon(string wepID)
  {
    currentWeaponGO.GetComponent<Renderer>().enabled = false;
    //set weapon from dictionary to be the equipped weapon, allowing it to shoot
    currentWeaponGO = WeaponsList[wepID];
    equippedWeaponStats = currentWeaponGO.GetComponent<WeaponStats>();
    currentWeaponGO.GetComponent<Renderer>().enabled = true;
    Debug.Log(equippedWeaponStats.name + " equipped");
  }

  //req for listing weapons in onGUI element
  public string listOfWeapons;

  public void listWeapons()
  {
    foreach (KeyValuePair<string, GameObject> item in WeaponsList)
    {
      listOfWeapons += string.Format(item.Key.ToString() +"-"+ item.Value.name.ToString() + " ");
    }
  }

  //method to repair all damage from a mech
  public void repairAll()
  {
    Debug.Log(mech.name + " Health restored to max.");
    mech.hpTorso = mech.hpTorsoMax;
    mech.hpHead = mech.hpHeadMax;
    mech.hpArm = mech.hpArmMax;
    mech.hpLeg = mech.hpLegMax;
    mech.shieldPoints = mech.shieldPointsMax;
  }
  //method use to reset shields
  public void repairShield()
  {
    mech.shieldPoints = mech.shieldPointsMax;
  }
  //method to restore TP
  public void restoreTP()
  {
    mech.tpCurrent = mech.tpMax;
  }
  //method to reload weapons
  public void reloadWeapons()
  {
    foreach (GameObject value in WeaponsList.Values)
    {
      value.GetComponent<WeaponStats>().rdyToFire = true;
      value.GetComponent<WeaponStats>().shotsRemaining = value.GetComponent<WeaponStats>().weapon.rateOfFire;
    }
  }

  void setCurrentHex() 
	{ 
		//get current hex and assign x and y values 
    currentHex = transform.parent.gameObject;
    x = currentHex.GetComponent<Hex>().x;
    y = currentHex.GetComponent<Hex>().y;
  }


  //Testing Pathing
  Seeker seeker;

	public void moveUnit(Vector3 start, Vector3 end)
	{
    //Testing Pathing
    seeker.StartPath(start, end, onPathComplete);
	}

  //Testing Pathing
  public void onPathComplete (Path p)
  {
    foreach (GraphNode node in p.path)
    {
      Debug.Log("Waypoint: " + (Vector3)node.position);
    }
    //set the position of the model to stand on the top of its parent hex
    transform.localPosition = new Vector3(0, 0, modelHeight / 1.5f);
  }

  
  //req to randomize hit locations
  private float randHit;
  //req to set hit location
  private string hitLoc;

  //figure out what location was randomly hit
  //maybe I could conbine this with the damaging if method? Far future thing.
  public void determineHitLoc()
  {
    randHit = Random.value;
    if (randHit > 0.9f)
    {
      hitLoc = "Head";
    }
    else if (randHit > 0.6f)
    {
      hitLoc = "Torso";
    }
    else if (randHit > 0.4f)
    {
      hitLoc = "Arm";
    }
    else if (randHit > 0.2f)
    {
      hitLoc = "Leg";
    }
    else
    {
      hitLoc = "miss";
    }
  }

  //this will be changed depending on the weapon used and each weapon will 
  //probably have some sort of damage get'er method within itself
  private int dmg;
	private int shld;
  private int armor;
  private int bonusDmg;
  private int shieldDmg;
	
	public void newDmg(int dmgToTake, int armor, int shield, string weaponName)
	{	
		//Gauss Rifle deals double damage to shields and normal damage after that.
    //this If statement also encapsulates all other weapon's damage vs shields
		//tested: this logic seems to work
    if(shield > 0)
    {
      if(weaponName == "Gauss Rifle")
      {
        shieldDmg = dmgToTake;
        for (int i = 0; i < dmgToTake; i++)
        {
          shield -= 2;
          shieldDmg -= 1;
          if (shield <= 0)
          {
            break;
          }
        }
        dmgToTake = shieldDmg;
      } else
      {
        shieldDmg = dmgToTake;
        for (int i = 0; i < dmgToTake; i++)
        {
          shield -= 1;
          shieldDmg -= 1;
          if (shield <= 0)
          {
            break;
          }
        }
        //post shield damage stats, how much damage to deal to the actual mech
        // and how many points of shield are left
        dmgToTake = shieldDmg;
      }
      shld = shield;
      if(shld < 0)
      {
        shld = 0;
      }
    }

		//autocannons reduce their dmg by double the armor value
		//tested: works as expected
		if(weaponName == "Autocannon")
		{
			armor = armor*2;
		}

		//flamers ignore armor and instead deal double the armor value as bonus dmg
		//tested: works as expected (might be too powerful)
		if(weaponName == "Flamer")
		{
			bonusDmg = armor;
			armor = 0;
		}

		//calc dmg to hp
		dmg = dmgToTake - armor + bonusDmg;
    
    //minimum of 0 dmg
		if (dmg < 0)
    {
      dmg = 0;
    }

		//lasers always do 5 dmg
		if (weaponName == "Laser" && dmg < 5)
		{
			dmg = 5;
		}
	}

  public void spendTP(int weaponTP)
  {
    if (noArm)
    { //double TP Cost if no arms remaining
      mech.tpCurrent -= weaponTP*2;
    } 
    else if (oneArm)
    { //+50% TP cost if one arm remaining
      mech.tpCurrent -= weaponTP + Mathf.RoundToInt(weaponTP/2);
    }
    else
    { //normal TP cost if arms are normal
      mech.tpCurrent -= weaponTP;
    }
  }

  //req for one arm/no arm implimentation
  public bool oneArm = false;
  public bool noArm = false;
  //req for legged mech logic
  public bool oneLeg = false;

  //req for miss dice
  private int missDice = 0;
  private int misses = 0;
  
  //method to take damage (with hit Locations)
	//provide the damage to take as an argument
	//which should come from the weapon's damage stat
  public void takeDamage(int dmgToTake, string weaponName)
  {
		if (weaponName == "Rockets")
		{
			missDice -= 1;
			if(missDice < 0) {
				missDice = 0;
			}
		}
    for (int i = 0; i < missDice + 1; i++)
      {
        determineHitLoc();
        // Debug.Log(hitLoc);
        if (hitLoc == "miss")
        {
          misses++;
        }
        // Debug.Log(misses);
      }

    //deal the damage to the proper location
    if (misses != 0)
    {
      hitLoc = "not";
      misses = 0;
    }    
		else
    {
      if (hitLoc == "Head")
      {
				newDmg(dmgToTake, mech.armorHead, mech.shieldPoints, weaponName);
        mech.shieldPoints = shld;
        mech.hpHead -= dmg;
      }
      else if (hitLoc == "Torso")
      {
        newDmg(dmgToTake, mech.armorTorso, mech.shieldPoints, weaponName);
        mech.shieldPoints = shld;
        mech.hpTorso -= dmg;
      }
      else if (hitLoc == "Arm")
      {
        if (mech.hpArm > 0)
        {
          newDmg(dmgToTake, mech.armorArm, mech.shieldPoints, weaponName);
          mech.shieldPoints = shld;
          mech.hpArm -= dmg;
        }
        else
        {
          hitLoc = "Torso";
          newDmg(dmgToTake, mech.armorTorso, mech.shieldPoints, weaponName);
          mech.shieldPoints = shld;
          mech.hpTorso -= dmg;
        }
      }
      else if (hitLoc == "Leg")
      {
        if (mech.hpLeg > 0)
        {
          newDmg(dmgToTake, mech.armorLeg, mech.shieldPoints, weaponName);
          mech.shieldPoints = shld;
          mech.hpLeg -= dmg;
        }
        else
        {
          hitLoc = "Torso";
          newDmg(dmgToTake, mech.armorTorso, mech.shieldPoints, weaponName);
          mech.shieldPoints = shld;
          mech.hpTorso -= dmg;
        }
      }
    }
		
    Debug.Log(mech.name + " " + hitLoc + " damaged.");

    //check to see if the mech has been disarmed
    if (mech.hpArm <= 0)
    {
      Debug.Log(mech.name + " arm destroyed. TP cost to fire weapons increased.");
      //first time losing an arm
      oneArm = true;
      //set health for second arm
      mech.hpArm = mech.hpArmMax;
      //if arm health drops again lose second arm
      if (oneArm && mech.hpArm <= 0)
      {
        noArm = true;
      }
      //TODO: reduce number of weapons able to be fired to 2 and then 1
    }
    //check to see if the mech is legged
    if (mech.hpLeg <= 0)
    {
      Debug.Log(mech.name + " leg destroyed. Movement speed reduced.");
      //first leg destroyed
      oneLeg = true;
      //set health for second leg
      mech.hpLeg = mech.hpLegMax;
      //if leg health drops again lose second leg and die
      if (oneLeg && mech.hpLeg <= 0)
      {
        Debug.Log(mech.name + " final leg destroyed. Mech destroyed.");
        Destroy(gameObject);
      }
      //increase TP per move + 1
      //halve max movement distance
    }
    //check to see if mech is destroyed
    if (mech.hpTorso <= 0 || mech.hpHead <= 0)
    {
			Debug.Log(mech.name + " destroyed.");
			//Destroy the mech/gameObject
			Destroy(gameObject);
    }  
  }
	
	

	void Update() {
		//Allows the Mech to always know what Hex it's located at.
		//Is this important? does it need to be constant? can it be called only when needed?
    setCurrentHex();
  }


}
