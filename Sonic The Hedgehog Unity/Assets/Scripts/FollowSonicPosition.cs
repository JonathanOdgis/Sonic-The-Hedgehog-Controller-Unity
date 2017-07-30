using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSonicPosition : MonoBehaviour {
	public Transform target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (target.position.x, this.transform.position.y, target.transform.position.z);
	}
}
