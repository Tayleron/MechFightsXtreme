using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLoc : MonoBehaviour {
  
	public string hitLoc;

  List<string> hitLocs = new List<string>();


  private void Start() {

    hitLocs.Add("hpArm");
    hitLocs.Add("hpLeg");
    hitLocs.Add("hpTorso");
    hitLocs.Add("hpHead");

  }	
}
