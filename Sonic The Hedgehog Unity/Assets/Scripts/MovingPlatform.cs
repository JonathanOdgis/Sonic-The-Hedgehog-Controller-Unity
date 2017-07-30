using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	private GameObject target=null;
	private Vector3 offset;
	void Start(){
		target = null;
	}
	void OnTriggerStay(Collider col){
		target = col.gameObject;
		offset = target.transform.position - transform.position;
	}
	void OnTriggerExit(Collider col){
		target = null;
	}
	void LateUpdate(){
		if (target != null) {
			target.transform.position = transform.position+offset;
		}
	}
}
