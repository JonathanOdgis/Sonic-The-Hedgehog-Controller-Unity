using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class HedgehogTime : MonoBehaviour {
	bool is_hedgehog_time;
	// Use this for initialization
	void Start () {
		
	}

	void OnDrawGizmos() {
		Color cube_color = new Color(0, 0, 200, .2f);

		Gizmos.color = cube_color;
		Gizmos.DrawCube (transform.position, this.transform.localScale);
	}

	// Update is called once per frame
	void Update () {
		if (is_hedgehog_time) {
			if (CrossPlatformInputManager.GetAxis ("Horizontal") != 0 && CrossPlatformInputManager.GetAxis ("Horizontal") != 0) {
				SonicController.Instance.speed = 20f;
				SonicController.Instance.anim.SetFloat ("movement", 100);
				SonicController.Instance.anim.speed = .4f;
			}
				SonicController.Instance.spring_speed = 50f;
			
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == SonicController.Instance.gameObject) {
			is_hedgehog_time = true;
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == SonicController.Instance.gameObject) {
			is_hedgehog_time = false;
			SonicController.Instance.anim.speed = 1f;
			SonicController.Instance.speed = SonicController.Instance.max_speed/2;
			SonicController.Instance.spring_speed = 200f;
		}
	}
}
