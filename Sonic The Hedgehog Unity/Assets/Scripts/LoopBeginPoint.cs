using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBeginPoint : MonoBehaviour {
	public EditorPath loop_path;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<SonicController> () && !SonicController.Instance.is_looping) {
			SonicController.Instance.is_looping = true;
			SonicController.Instance.PathToFollow = loop_path;
		}
	}

}
