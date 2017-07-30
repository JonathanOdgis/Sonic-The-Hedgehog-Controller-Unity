using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotobugEnemy : MonoBehaviour {
	Rigidbody rgb;
	public Color rayColor = Color.white;
	public List<Transform> path_objs = new List<Transform>();
	Transform[] theArray;
	public Transform path_points_container;

	//current index in the pathtofollow
	public int currentWayPointID;
	private float reachDistance = 1.0f;
	public float speed = 5f;
	public float rot_speed = 100f;
	public GameObject enemy_effect;

	// Use this for initialization
	void Start () {
		rgb = GetComponent<Rigidbody> ();

	}

	void OnDrawGizmos()
	{
		Gizmos.color = rayColor;
		theArray = path_points_container.GetComponentsInChildren<Transform> ();
		path_objs.Clear ();
		foreach (Transform path_obj in theArray) {
				path_objs.Add (path_obj);
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

	void Update()
	{
		Physics.IgnoreCollision (this.GetComponent<Collider> (), SonicController.Instance.collider);
		if (currentWayPointID != path_objs.Count && currentWayPointID >= 0) {
			float distance = Vector3.Distance (path_objs [currentWayPointID].position, transform.position);
			transform.position = Vector3.MoveTowards (transform.position, path_objs [currentWayPointID].position, speed * 5 * Time.deltaTime);

			if (distance <= reachDistance) {
				currentWayPointID++;
				if (currentWayPointID < path_objs.Count - 1)
					currentWayPointID++;
				else
					currentWayPointID = 0;
			}
		}

		var rotation = Quaternion.LookRotation (path_objs [currentWayPointID].position-transform.position);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rot_speed * Time.deltaTime);

	}

	IEnumerator DestroyState() {
		rgb.AddForce (Vector3.up * 10000);
		yield return new WaitForSeconds (1f);
		GameObject clone = Instantiate (enemy_effect.gameObject, this.transform.position, this.transform.rotation) as GameObject;
		yield return new WaitForSeconds (.2f);
		Destroy (this.gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<SonicController> ()) {
			if (SonicController.Instance.is_spindashing || !SonicController.Instance.is_grounded) {
				StartCoroutine (DestroyState ());
			}
		}
	}
}
