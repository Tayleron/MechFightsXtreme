using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MechStats : MonoBehaviour {

	public Mech mech;
	public GameObject currentHex;
	public Renderer rend;

	public Weapon weapon;

	
	public float modelHeight;
	private string OGName;

	private int x;
	private int y;
	
	// Use this for initialization
	void Start () {
		//get original name of the mech
		OGName = mech.name;
		//create unique instance of mech (clone)
    mech = Instantiate(mech);
		//rename the clone to the orginal name
		mech.name = OGName;
		
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

  //this will be changed depending on the weapon used and each weapon will 
  //probably have some sort of damage get'er method within itself
  private int dmgToTake = 4;
	private int newDmg;
  //req to randomize hit locations
  private float randHit;
  //req to set hit location
  private string hitLoc;

  private int missDice = 0;
  private int misses = 0;

  //figure out what location was randomly hit
  //maybe I could conbine this with the damaging if method? Far future thing.
  public void DetermineHitLoc()
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

  //method to take damage (with hit Locations)
  public void takeDamage()
  {    
    for (int i = 0; i < missDice + 1; i++)
      {
        DetermineHitLoc();
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
        newDmg = dmgToTake - mech.armorHead;
        if (newDmg < 0)
        {
          newDmg = 0;
        }
        mech.hpHead -= newDmg;
      }
      else if (hitLoc == "Torso")
      {
        newDmg = dmgToTake - mech.armorTorso;
        if (newDmg < 0)
        {
          newDmg = 0;
        }
        mech.hpTorso -= newDmg;
      }
      else if (hitLoc == "Arm")
      {
        if (mech.hpArm > 0)
        {
          newDmg = dmgToTake - mech.armorArm;
          if (newDmg < 0)
          {
            newDmg = 0;
          }
          mech.hpArm -= newDmg;
        }
        else
        {
          hitLoc = "Torso";
          newDmg = dmgToTake - mech.armorTorso;
          if (newDmg < 0)
          {
            newDmg = 0;
          }
          mech.hpTorso -= newDmg;
        }
      }
      else if (hitLoc == "Leg")
      {
        if (mech.hpLeg > 0)
        {
          newDmg = dmgToTake - mech.armorLeg;
          if (newDmg < 0)
          {
            newDmg = 0;
          }
          mech.hpLeg -= newDmg;
        }
        else
        {
          hitLoc = "Torso";
          newDmg = dmgToTake - mech.armorTorso;
          if (newDmg < 0)
          {
            newDmg = 0;
          }
          mech.hpTorso -= newDmg;
        }
      }
    }
		
    Debug.Log(mech.name + " " + hitLoc + " damaged.");

    //check to see if the mech has been disarmed
    if (mech.hpArm <= 0)
    {
      Debug.Log(mech.name + " arm destroyed. TP cost to fire weapons increased.");
      //TODO: Arm damaged logic
      //double TP cost to fire weapons
      //reduce number of weapons able to be fired to 1
    }
    //check to see if the mech is legged
    if (mech.hpLeg <= 0)
    {
      Debug.Log(mech.name + " leg destroyed. Movement speed reduced.");
      //TODO: leg damaged logic
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
	}

	void Update() {
		//Allows the Mech to always know what Hex it's located at.
		//Is this important? does it need to be constant? can it be called only when needed?
    setCurrentHex();
    

  }


}
