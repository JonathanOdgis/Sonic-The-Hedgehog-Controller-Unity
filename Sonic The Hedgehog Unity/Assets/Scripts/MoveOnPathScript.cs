using UnityEngine;
using System.Collections;

public class MoveOnPathScript : MonoBehaviour {
	public static MoveOnPathScript Instance;
	//Path that the object will currently follow
	public EditorPath PathToFollow;

	//current index in the pathtofollow
	public int currentWayPointID;

	//speed to follow the path
	public float speed;

	public bool is_moving_forwards;
	public bool is_moving_backwards;

	bool is_railed;

	//If the player is there and it's near the point and it won't be as hard and more curvy in moving with the points (0) is hard (1) is a bit more curved
	private float reachDistance = 1.0f;
	public float rotationSpeed = 5.0f;
	public string pathName;
	Vector3 last_position;
	Vector3 current_position;
	Quaternion rotation = new Quaternion();

	// Use this for initialization
	void Start () {
		Instance = this; 
		assign_part_of_path ();


		//PathToFollow = GameObject.Find (pathName).GetComponent<EditorPath> ();
		last_position = transform.position;
	}

	void FixedUpdate()
	{
		if (Input.GetKeyDown (KeyCode.J)) {
			is_moving_forwards = true;
			is_moving_backwards = false;
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			is_moving_forwards = false;
			is_moving_backwards = true;
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			is_moving_forwards = false;
			is_moving_backwards = false;
		}

		if (is_moving_backwards) {
			//record the previous way point from the current
			rotation = Quaternion.LookRotation (transform.position - PathToFollow.path_objs [PathToFollow.path_objs.Count-1].position);
		} 
		if (is_moving_forwards) {
			rotation = Quaternion.LookRotation (transform.position - PathToFollow.path_objs [currentWayPointID].position);
		} 

		transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
	
	}


	// Update is called once per frame
	void Update () {

		if (currentWayPointID != PathToFollow.path_objs.Count && currentWayPointID >= 0) {
			float distance = Vector3.Distance (PathToFollow.path_objs [currentWayPointID].position, transform.position);
			transform.position = Vector3.MoveTowards (transform.position, PathToFollow.path_objs [currentWayPointID].position, speed * 5 * Time.deltaTime);

			if (distance <= reachDistance && is_moving_forwards) {
				currentWayPointID++;
				if (currentWayPointID < PathToFollow.path_objs.Count -1)
					currentWayPointID++;
			}
			if (distance <= reachDistance && is_moving_backwards) {
				if (currentWayPointID > 0)
					currentWayPointID--;
			}
		} 

	}

	void assign_part_of_path()
	{
		foreach (Transform path in PathToFollow.path_objs)
		{
			float dist = Vector3.Distance (path.position, transform.position);
			Debug.Log (dist + " vs " + reachDistance);
			if (dist < 3) {
				Debug.Log ("FOUND");
				currentWayPointID = PathToFollow.path_objs.IndexOf (path);
				break;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Rail") && !is_railed) {

			if (other.gameObject.transform.parent.GetComponent<EditorPath> ()) {
				is_railed = true;
				GetComponent<Rigidbody> ().isKinematic = true;
				PathToFollow = other.gameObject.transform.parent.GetComponent<EditorPath> ();
				assign_part_of_path ();
			}
		}
	}

}
