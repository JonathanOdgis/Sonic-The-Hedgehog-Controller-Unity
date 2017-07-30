using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrollCamTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos() {
		Color cube_color = new Color(0, 100, 0, .2f);

		Gizmos.color = cube_color;
		Gizmos.DrawCube (transform.position, this.transform.localScale);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == SonicController.Instance.gameObject) {
			SonicCam.Instance.dynamic_cam = false;
			SonicCam.Instance.side_scroll_mode = true;
			SonicController.Instance.side_scroll_z = this.transform.position.z;
			SonicController.Instance.is_side_scrolling = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == SonicController.Instance.gameObject) {
			SonicCam.Instance.dynamic_cam = true;
			SonicCam.Instance.side_scroll_mode = false;
			SonicController.Instance.is_side_scrolling = false;
		}
	}
}
