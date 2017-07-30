using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SonicController> ()) {
			SonicController.Instance.speed = SonicController.Instance.max_speed * 2;
		}
	}
}
