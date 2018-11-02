using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour {

	public Weapon weapon;
	public MechStats mech;


	// Use this for initialization
	void Start () {
		// Debug.Log("I am a: " + weapon.weaponName + " and I do: " + weapon.damage 
		// + " damage.");		
	}
	
	//include the mech to be attacked as an argument
	public void atkMech(MechStats mech)
	{
		//call the damage method and pass it the weapon's damage stat
		mech.takeDamage(weapon.damage, weapon.weaponName);
	}
	
}
