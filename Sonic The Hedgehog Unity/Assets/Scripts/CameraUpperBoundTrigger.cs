using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUpperBoundTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos() {
		Color cube_color = new Color(0, 0, 100, .2f);

		Gizmos.color = cube_color;
		Gizmos.DrawCube (transform.position, this.transform.localScale);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<SonicController> ()) {
			SonicCam.Instance.upper_bound_mode = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.GetComponent<SonicController> ()) {
			SonicCam.Instance.upper_bound_mode = false;
		}
	}
}
