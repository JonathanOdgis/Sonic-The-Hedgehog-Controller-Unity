using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EditorPath : MonoBehaviour {

	public Color rayColor = Color.white;
	public List<Transform> path_objs = new List<Transform>();
	Transform[] theArray;



	// Use this for initialization
	void Start () {
			
	}

	void OnDrawGizmos()
	{
		Gizmos.color = rayColor;
		theArray = GetComponentsInChildren<Transform> ();
		path_objs.Clear ();
		foreach (Transform path_obj in theArray) {
			if (path_obj != this.transform && path_obj.transform.parent == this.transform) {
				path_objs.Add (path_obj);
			}
		}

		for (int i = 0; i < path_objs.Count; i++) {
			Vector3 pos = path_objs [i].position;
			if (i > 0) {
				Vector3 previous = path_objs [i - 1].position;
				Gizmos.DrawLine (previous, pos);
				Gizmos.DrawWireSphere (pos, 0.3f);
			}
		}
	}


	// Update is called once per frame
	void Update () {
	
	}
}
