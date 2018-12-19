using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {

	[SerializeField] private GameObject GOFlagStick;
	private Transform transFlagCloth;
	private Renderer rendFlagCloth;

	public Material flagMat;
  public GameObject currentHex;
  public float modelHeight;

	public string teamColor;

  // private int x;
  // private int y;
	

	// Use this for initialization
	void Start () 
	{
    setCurrentHex();

		transFlagCloth = GOFlagStick.transform.Find("flagCloth");
		rendFlagCloth = transFlagCloth.GetComponent<Renderer>();
		rendFlagCloth.material = flagMat;

		modelHeight = GOFlagStick.GetComponent<Renderer>().bounds.size.y;
    transform.localPosition = new Vector3(0, 0, modelHeight / 1.5f);
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
  
	public void setCurrentHex()
  {
    //get current hex and assign x and y values 
    currentHex = transform.parent.gameObject;
		currentHex.GetComponent<Hex>().flag = this;
    // x = currentHex.GetComponent<Hex>().x;
    // y = currentHex.GetComponent<Hex>().y;
  }
}
