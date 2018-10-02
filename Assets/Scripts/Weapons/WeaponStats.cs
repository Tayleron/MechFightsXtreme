using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour {

	public Weapon weapon;
	// Use this for initialization
	void Start () {
		Debug.Log("I am a: " + weapon.weaponName + " and I do: " + weapon.damage 
		+ " damage.");		
	}
}
