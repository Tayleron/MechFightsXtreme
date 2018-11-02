using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MechStats : MonoBehaviour {

	public Mech mech;
	public GameObject currentHex;
	public Renderer rend;

	public Weapon weapon;
	public WeaponStats equippedWeapon;
	
	
	public float modelHeight;
	private string OGName;

	private int x;
	private int y;
	
  void Start()
  {
		//get original name of the mech
		OGName = mech.name;
		//create unique instance of mech (clone)
    mech = Instantiate(mech);
		//rename the clone to the orginal name
		mech.name = OGName;

		//temp instantiate a weapon to test	
		if (equippedWeapon != null)
    {
      OGName = equippedWeapon.name;
      equippedWeapon = Instantiate(equippedWeapon);
		  equippedWeapon.name = OGName;
    }
		//get the full height of the model
		modelHeight = mech.MechPrefab.GetComponent<Renderer>().bounds.size.y;
    moveUnit();
    setCurrentHex();

    Debug.Log("My name is: " + mech.mechName +
		" and I have: " + mech.movementRemaining + " movement remaining. I have: "
		+ mech.hpTorso + " Torso Hp remaining." + " And I am located at: " + x + ", " + y);
	}

	void setCurrentHex() 
	{ 
		//get current hex and assign x and y values 
    currentHex = transform.parent.gameObject;
    x = currentHex.GetComponent<Hex>().x;
    y = currentHex.GetComponent<Hex>().y;
  }

	public void moveUnit()
	{
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
      shld = shield;
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

		Debug.Log("Damage dealt: " + dmg);
	}

  public void spendTP(int weaponTP)
  {
    if (noArm)
    {
      mech.tpCurrent -= weaponTP*2;
    } 
    else if (oneArm)
    {
      mech.tpCurrent -= weaponTP + Mathf.RoundToInt(weaponTP/2);
    }
    else
    {
      mech.tpCurrent -= weaponTP;
    }
    Debug.Log("new TP " + mech.tpCurrent + "/" + mech.tpMax);
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
  public void repairShield()
  {
    mech.shieldPoints = mech.shieldPointsMax;
  }
  public void restoreTP()
  {
    mech.tpCurrent = mech.tpMax;
  }
	
	// void loadWeapons()
	// {
		
	// }
	// public void equipWeapon()
	// {

	// }

	void Update() {
		//Allows the Mech to always know what Hex it's located at.
		//Is this important? does it need to be constant? can it be called only when needed?
    setCurrentHex();
  }


}
