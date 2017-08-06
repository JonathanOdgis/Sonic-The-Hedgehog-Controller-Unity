using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableTrigger : MonoBehaviour {
	public GameObject[] objects_to_disable;
	public GameObject[] objects_to_enable;
	bool event_occured;

	// Use this for initialization
	void Start () {
		foreach (GameObject obj in objects_to_enable) {
			obj.SetActive (false);
		}
		foreach (GameObject obj in objects_to_disable) {
			obj.SetActive (true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == SonicController.Instance.gameObject && !event_occured) {
			event_occured = true;
			foreach (GameObject obj in objects_to_enable) {
				obj.SetActive (true);
			}
			foreach (GameObject obj in objects_to_disable) {
				obj.SetActive (false);
			}
		}
	}
}
