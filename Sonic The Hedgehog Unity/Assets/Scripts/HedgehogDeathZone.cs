using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogDeathZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnDrawGizmos() {
		Color cube_color = new Color(100, 0, 0, .2f);

		Gizmos.color = cube_color;
		Gizmos.DrawCube (transform.position, this.transform.localScale);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SonicController> ()) {
			StartCoroutine(SonicController.Instance.dead_state());
		}
	}
}

