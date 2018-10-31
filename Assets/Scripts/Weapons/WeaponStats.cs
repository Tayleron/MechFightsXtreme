using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour {

	public Weapon weapon;
	public MechStats mech;
	public MouseManager MM;


	// Use this for initialization
	void Start () {
		Debug.Log("I am a: " + weapon.weaponName + " and I do: " + weapon.damage 
		+ " damage.");		
	}

	public void atkMech(MechStats mech)
	{
		//call the choosen mech
	
		//call the damage method and pass it the weapon's damage stat
		mech.takeDamage(weapon.damage);
	}
	
}
