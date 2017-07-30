using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicWater : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (SonicController.Instance.speed < 70) {
			GetComponent<BoxCollider> ().enabled = false;
		} else {
			GetComponent<BoxCollider> ().enabled = true;
		}
	}


}
