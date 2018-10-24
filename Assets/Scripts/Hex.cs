using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {
	private Renderer HexMatRend;
  private MeshFilter HexMeshFilter;
	public Material[] HexMaterials;
  public Mesh[] HexMeshes;
	public GameObject ramp;
  public GridManager grid;

  public int x;
  public int y;

  public float HexModelYHeight;

	//Elevation determines the height of the hex 
	//Use Elevation to determine cliffs and the color/graphics hexes should be assigned
	//isWater determines if the hex is a water hex or a land hex	
	public int Elevation = 1;
  private int ElevationLast;
	public bool isWater = false;
	public bool isRamp = false;
 
	// Use this for initialization
	void Start () {
    HexMatRend = GetComponent<Renderer>();
    HexMeshFilter = GetComponent<MeshFilter>();
    HexMatRend.enabled = true;
    HexMatRend.sharedMaterial = HexMaterials[0];
	}

	private void ElevationCheck() {
    HexModelYHeight = HexMatRend.bounds.size.y;
    if (Elevation == 0)
    {
      HexMatRend.sharedMaterial = HexMaterials[0];
      HexMeshFilter.sharedMesh = HexMeshes[0];
      transform.localPosition = new Vector3(transform.localPosition.x, HexModelYHeight, transform.localPosition.z);
      ElevationLast = 0;
    }
    else if (Elevation == 1)
    {
      HexMatRend.sharedMaterial = HexMaterials[1];
      HexMeshFilter.sharedMesh = HexMeshes[1];
      transform.localPosition = new Vector3(transform.localPosition.x, HexModelYHeight, transform.localPosition.z);
      ElevationLast = 1;
    }
    else //meaning Elevation must be 2
    {
      HexMatRend.sharedMaterial = HexMaterials[2];
      HexMeshFilter.sharedMesh = HexMeshes[2];
      transform.localPosition = new Vector3(transform.localPosition.x, HexModelYHeight, transform.localPosition.z);
      ElevationLast = 2;
    }
	}
	private void WaterCheck() {
    if (isWater)
    {
      HexMatRend.sharedMaterial = HexMaterials[3];
      HexMeshFilter.sharedMesh = HexMeshes[3];
      transform.localPosition = new Vector3(transform.localPosition.x, 0.05f, transform.localPosition.z);
    } else {ElevationCheck();}
	}
	private void RampCheck() {
    if (isRamp && isWater == false && Elevation != 2)
    {
      ramp.GetComponent<Renderer>().enabled = true;
    }
    else
    {
      ramp.GetComponent<Renderer>().enabled = false;
    }
	}

	void Update()
	{	//Elevation
    if (Elevation != ElevationLast) {
      ElevationCheck();
    }
		//Check for Water
		WaterCheck();
		//Check for Ramp
		RampCheck();
	}
}

