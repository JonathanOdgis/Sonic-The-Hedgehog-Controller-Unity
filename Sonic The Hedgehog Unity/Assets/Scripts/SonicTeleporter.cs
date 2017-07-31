using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicTeleporter : MonoBehaviour {
	public Transform new_pos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == SonicController.Instance.gameObject) {
			SonicController.Instance.transform.position = new_pos.position;
		}
	}
}
