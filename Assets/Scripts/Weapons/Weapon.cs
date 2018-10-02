using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "New Weapon")]
public class Weapon : ScriptableObject {

	public string weaponName;

	public int damage;
	public int maxRange;
	public int minRange;
	public int tpCost;
	public int weight;

	public GameObject weaponPrefab;
}
