using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onGUI : MonoBehaviour {

	public MouseManager mm;
	
  //simple statement to inform of readiness
  private string rdyOrNot;

  //temp GUI element to show selected mech stats + buttons
  void OnGUI()
  {
    if (mm.selectedMech != null)
    {
      GUI.Label(new Rect(10, 20, 100, 40), "Mech Selected: " + mm.selectedMech.mech.name);
      GUI.Label(new Rect(10, 55, 100, 25), mm.selectedMech.mech.hpHead + " Head HP");
      GUI.Label(new Rect(10, 70, 100, 25), mm.selectedMech.mech.hpTorso + " Torso HP");
      GUI.Label(new Rect(10, 85, 100, 25), mm.selectedMech.mech.hpArm + " Arm HP");
      GUI.Label(new Rect(10, 100, 100, 25), mm.selectedMech.mech.hpLeg + " Leg HP");
      GUI.Label(new Rect(10, 115, 100, 25), mm.selectedMech.mech.shieldPoints + " Shields");
      GUI.Label(new Rect(10, 130, 100, 25), mm.selectedMech.mech.tpCurrent + "/" + mm.selectedMech.mech.tpMax + " TP");

      //if the equipped Weapon is ready or not
      if (mm.selectedMech.equippedWeapon.rdyToFire)
      {
        rdyOrNot = ": Ready";
      }
      else
      {
        rdyOrNot = ": Not Ready";
      }

      GUI.Label(new Rect(10, 145, 170, 40), "Equipped Weapon:          " + mm.selectedMech.equippedWeapon.name + rdyOrNot + " "
                                            + mm.selectedMech.equippedWeapon.shotsRemaining + "/"
                                            + mm.selectedMech.equippedWeapon.weapon.rateOfFire);

      if (mm.selectedMech.wep00)
      {
        if (GUI.Button(new Rect(10, 190, 30, 25), "00"))
        {
          mm.selectedMech.equipWeapon("00");
        }
      }
      if (mm.selectedMech.wep01)
      {
        if (GUI.Button(new Rect(40, 190, 30, 25), "01"))
        {
          mm.selectedMech.equipWeapon("01");
        }
      }
      if (mm.selectedMech.wep02)
      {
        if (GUI.Button(new Rect(70, 190, 30, 25), "02"))
        {
          mm.selectedMech.equipWeapon("02");
        }
      }
      if (mm.selectedMech.wep03)
      {
        if (GUI.Button(new Rect(100, 190, 30, 25), "03"))
        {
          mm.selectedMech.equipWeapon("03");
        }
      }
      if (mm.selectedMech.wep04)
      {
        if (GUI.Button(new Rect(130, 190, 30, 25), "04"))
        {
          mm.selectedMech.equipWeapon("04");
        }
      }
      if (mm.selectedMech.wep05)
      {
        if (GUI.Button(new Rect(10, 220, 30, 25), "05"))
        {
          mm.selectedMech.equipWeapon("05");
        }
      }
      if (mm.selectedMech.wep06)
      {
        if (GUI.Button(new Rect(40, 220, 30, 25), "06"))
        {
          mm.selectedMech.equipWeapon("06");
        }
      }
      if (mm.selectedMech.wep07)
      {
        if (GUI.Button(new Rect(70, 220, 30, 25), "07"))
        {
          mm.selectedMech.equipWeapon("07");
        }
      }
      if (mm.selectedMech.wep08)
      {
        if (GUI.Button(new Rect(100, 220, 30, 25), "08"))
        {
          mm.selectedMech.equipWeapon("08");
        }
      }
      if (mm.selectedMech.wep09)
      {
        if (GUI.Button(new Rect(130, 220, 30, 25), "09"))
        {
          mm.selectedMech.equipWeapon("09");
        }
      }
      if (GUI.Button(new Rect(10, 250, 100, 25), "Reset Health"))
      {
        mm.selectedMech.repairAll();
      }
      if (GUI.Button(new Rect(10, 280, 100, 25), "Reset Shield"))
      {
        mm.selectedMech.repairShield();
      }
      if (GUI.Button(new Rect(10, 310, 100, 25), "Restore TP"))
      {
        mm.selectedMech.restoreTP();
      }
      if (GUI.Button(new Rect(10, 340, 100, 25), "Attack"))
      {
        mm.attack();
      }
      if (GUI.Button(new Rect(10, 370, 100, 25), "Reload"))
      {
        mm.selectedMech.reloadWeapons();
      }
      GUI.Label(new Rect(120, 300, 100, 100), "Weapons: " + mm.selectedMech.listOfWeapons);
    }
    if (mm.selectedHex != null)
    {
      GUI.Label(new Rect(10, 0, 100, 20), mm.selectedHex.name + " selected");
    }
  }
}
