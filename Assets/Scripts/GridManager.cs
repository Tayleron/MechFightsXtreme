using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GridManager : MonoBehaviour 
{
  //Req for persisence and save/load functions
  public static GridManager control;
	//Attach the Prefab Hex in unity
	public GameObject Hex;
  //Allow the setting of the grid size in Unity
	public int gridWidthInHexes = 10;
	public int gridHeightInHexes = 10;
	//Allow setSizes function to store the dimensions of the Prefab
	private float hexWidth;
	private float hexHeight;
  //Set default Elevation of Hexes when not randomizing
  public int setDefaultElevation = 0;
  //Randomizer variables
  public bool RandomOnOff = true;
  //percentage of hexes that will be water when randomizing maps
  public float waterPercent = .25f;
  //on off switch for Labels
  public bool LabelsOnOff = true;
  //Used by dictionary to store Hex Coords as a string
  private string hexCoords;
  //input a string format "x,y" to spawn unit at that location
  public string CoordsToSpawnAt = "2,2";


  //Dictionary of all hexes, added to by createGrid()
  Dictionary<string, GameObject> hexValues = new Dictionary<string, GameObject>();

	//Get the dimensions to allow proper spacing of the tiles
	void setSizes() 
	{
    hexWidth = Hex.GetComponent<Renderer>().bounds.size.x;
    hexHeight = Hex.GetComponent<Renderer>().bounds.size.z;
	}
  //The calcInitPos function calculates the positioning for the tiles into the grid
	//The center of the grid is (0,0,0)
  Vector3 calcInitPos()	
	{
    Vector3 initPos;
    //the initial position will be in the left upper corner
    initPos = new Vector3(-hexWidth * gridWidthInHexes / 2f + hexWidth / 2, 0,
        gridHeightInHexes / 2f * hexHeight - hexHeight / 2);

    return initPos;
  }
  //method used to convert hex grid coordinates to game world coordinates
  public Vector3 calcWorldCoord(Vector2 gridPos)
  {
   // Debug.Log(gridPos.x +" "+ gridPos.y);
    //Position of the first hex tile
    Vector3 initPos = calcInitPos();
    //Every second row is offset by half of the tile width
    float offset = 0;
    if (gridPos.y % 2 != 0)
      offset = hexWidth / 2;

    float x = initPos.x + offset + gridPos.x * hexWidth;
    //Every new line is offset in z direction by 3/4 of the hexagon height
    float z = initPos.z - gridPos.y * hexHeight * 0.75f;
    return new Vector3(x, 0, z);
  }

  //Finally the method which initialises and positions all the tiles
  void createGrid()
  {
    //Game object which is the parent of all the hex tiles
    GameObject hexGridGO = new GameObject("HexGrid");

    for (float y = 0; y < gridHeightInHexes; y++)
    {
      for (float x = 0; x < gridWidthInHexes; x++)
      {
        //GameObject assigned to Hex public variable is cloned
        GameObject hex = (GameObject)Instantiate(Hex);
        //Current position in grid
        Vector2 gridPos = new Vector2(x, y);
        //create a string to store the Coordinates
        hexCoords = string.Format("{0},{1}", x, y);
        //this line spreads the hexes out
        hex.transform.position = calcWorldCoord(gridPos);
        //this line puts all the hexes under the hexgrid GameObject
        hex.transform.parent = hexGridGO.transform;
        //Name the hexes and give them their Coordinates
        hex.name = "Hex_" + x + "_" + y;
        hex.GetComponent<Hex>().x = (int)x;
        hex.GetComponent<Hex>().y = (int)y;
        //Labels for the hex coordinates
        if (LabelsOnOff) {
          hex.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", x, y);
        }
        //Set Hexes to the default height as specified above
        hex.GetComponent<Hex>().Elevation = setDefaultElevation;
        //Toggle for randomizer
        if (RandomOnOff) {
          //Add random Elevation and Random Water
          hex.GetComponent<Hex>().Elevation = UnityEngine.Random.Range(0, 3);
          hex.GetComponent<Hex>().isWater = UnityEngine.Random.value < waterPercent;
        }
        //Add the new hex to the hex dictionary
        hexValues.Add(hexCoords,hex);
      }
    }
  }
  //temp prefab holder to test spawner
  public GameObject mechPrefab;
  public Transform hexTran;

  void spawnUnitAt(GameObject mechPrefab, string location)
  {
    GameObject myHex = hexValues[location];
    hexTran = myHex.transform;
    Instantiate(mechPrefab, myHex.transform.position, Quaternion.identity, myHex.transform);
  }
  

  //Adding persistence
  void Awake () {
    if (control == null) {
      DontDestroyOnLoad(gameObject);
      control = this;
    } else if (control != this) {
      Destroy(gameObject);
    }
  }
	// Use this for initialization
	void Start () 
	{
		setSizes();
		createGrid();
    spawnUnitAt(mechPrefab, CoordsToSpawnAt);

	}

  //GUI buttons to allow for changes
  // void OnGUI() {
  //   if (GUI.Button(new Rect(10,10,100,30), "Save")){
  //     control.SaveMap();
  //   }
  //   if (GUI.Button(new Rect(10,50,100,30), "Load")) {
  //     control.LoadMap();
  //   }
  //   if (GUI.Button(new Rect(10,90,100,30), "Randomize")) {
  //     control.createGrid();
  //   }
  // }
}