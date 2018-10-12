using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public float scrollSpeed = 12f;
	// Update is called once per frame
	void Update () {
		Vector3 translate = new Vector3 (
			Input.GetAxis("Horizontal"),
			0,
			Input.GetAxis("Vertical")
		);
		this.transform.Translate(translate * scrollSpeed * Time.deltaTime, Space.World);
	}
}
