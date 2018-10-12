using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mech", menuName = "New Mech")]
public class Mech : ScriptableObject {
  
  public string mechName;
  
  public int hpArm;
  public int hpArmMax;

  public int hpLeg;
  public int hpLegMax;

  public int hpTorso;
  public int hpTorsoMax;

  public int hpHead;
  public int hpHeadMax;
  
  public int tpMax;
  public int tpCurrent;

  public int movementSpeedInHexes;
  public int movementCostPerHex;
  
  public int armorHead;
  public int armorTorso;
  public int armorLeg;
  public int armorArm;

  public int shieldPoints;
  public int weight;
}
