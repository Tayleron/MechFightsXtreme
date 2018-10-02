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
        hex.transform.position = calcWorldCoord(gridPos);
        hex.transform.parent = hexGridGO.transform;
      }
    }
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

  //GUI buttons to allow for changes
  void OnGUI() {
    if (GUI.Button(new Rect(10,10,100,30), "Save")){
      control.SaveMap();
    }
    if (GUI.Button(new Rect(10,50,100,30), "Load")) {
      control.LoadMap();
    }
  }

	// Use this for initialization
	void Start () 
	{
		setSizes();
		createGrid();
	}

  //Adding the save map function
  public void SaveMap () {
    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = File.Create(Application.persistentDataPath + "/map.dat");
    
    MapData data = new MapData();
    data.Hex = Hex;

    bf.Serialize(file, data);
    file.Close();
  }

  //adding the load map function  
  public void LoadMap () {
    if(File.Exists(Application.persistentDataPath + "map.dat")) 
    {
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Open(Application.persistentDataPath + "/map.dat", FileMode.Open);
      
      MapData data = (MapData)bf.Deserialize(file);
      file.Close();

      Hex = data.Hex;
    }

  }  
}

[Serializable]
class MapData
{
  public GameObject Hex;
}
