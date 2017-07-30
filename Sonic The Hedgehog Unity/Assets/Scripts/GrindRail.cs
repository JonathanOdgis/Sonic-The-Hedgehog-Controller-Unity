using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindRail : MonoBehaviour {
	public bool rail_active;
	EditorPath paths;
	// Use this for initialization
	void Start () {
		paths = GetComponent<EditorPath> ();
	}
	
	// Update is called once per frame
	void Update () {
		//TODO: Instead of trigger, use a raycast to determine if a distance from the paths each frame and then define active
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == SonicController.Instance.gameObject) {
			rail_active = true;
			SonicController.Instance.PathToFollow = paths;
			SonicController.Instance.is_grinding = true;
			//Assign the current waypoint depending on which part of the rail sonic is closest to
			var current_point = 0;
			foreach (Transform path in paths.transform) {
				if (Vector3.Distance (SonicController.Instance.transform.position, path.transform.position) < 3f) {
					SonicController.Instance.currentWayPointID = current_point;
					break;
				} else {
					current_point++;
				}

			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == SonicController.Instance.gameObject) {
			rail_active = false;
			SonicController.Instance.PathToFollow = null;
			SonicController.Instance.is_grinding = false;
			SonicController.Instance.currentWayPointID = 0;
		}
	}

}
