using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicCam : MonoBehaviour {
	public static SonicCam Instance;
	Camera cam;

	public bool standard_mode = true;
	public bool dynamic_cam = true;
	public bool side_scroll_mode = false;
	public bool free_fall_mode = false;
	public bool is_victory_mode = false;
	public bool upper_bound_mode = false;
	public bool locked_mode;

	public GameObject locked_mode_target;
	public float min_connection_distance = 3f;
	public float max_connection_distance = 30f;
	public float y_offset = 10;
	float cam_fov;
	bool is_started;

	public float cam_to_player_offset = 15f;
	public float standard_cam_offset = 15f;
	public float upper_cam_offset = 25f;
	public Vector3 side_scroll_cam_offset = new Vector3(0f, 7.5f, 0f);

	public float cam_height = 5f;
	public float standard_cam_height = 5f;
	public float upper_cam_height = 10f;

	public float sonic_cam_speed = 10f;

	Vector3 player_offset;

	void Start() {
		Instance = this;
		cam = GetComponent<Camera> ();
		cam_fov = cam.fieldOfView;
		//StartCoroutine(startup_state ());
		player_offset = this.transform.position - SonicController.Instance.transform.position;
		player_offset.y = cam_height;
		this.transform.position = SonicController.Instance.transform.position + player_offset.normalized * 20;
	}

	void FixedUpdate() {
		Vector3 player_offset = this.transform.position - SonicController.Instance.transform.position;
	
		//Cam Height 
		if (upper_bound_mode) {
			cam_height = upper_cam_height;
			cam_to_player_offset = upper_cam_offset;
		} else {
			cam_height = standard_cam_height;
			cam_to_player_offset = standard_cam_offset;
		}
			
		if (dynamic_cam) {
			var neededRotation = Quaternion.LookRotation (SonicController.Instance.gameObject.transform.position - this.transform.position);
			player_offset.y = cam_height;
			this.transform.position = SonicController.Instance.transform.position + player_offset.normalized * cam_to_player_offset;
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, neededRotation, sonic_cam_speed * Time.deltaTime);
		}
		if (side_scroll_mode) {
			var neededRotation = Quaternion.LookRotation (SonicController.Instance.gameObject.transform.position - this.transform.position);
			this.transform.position = SonicController.Instance.transform.position + side_scroll_cam_offset;
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, neededRotation, sonic_cam_speed * Time.deltaTime);
		}
		if (locked_mode) {
			var neededRotation = Quaternion.LookRotation (SonicController.Instance.gameObject.transform.position - this.transform.position);
			this.transform.position = locked_mode_target.transform.position;
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, neededRotation, sonic_cam_speed * Time.deltaTime);
		}
	}

	IEnumerator startup_state()
	{
		//Stops the camera from getting stuck behind walls if the veritcality is too much at the start of the game load
		yield return new WaitForSeconds (.3f);
		is_started = true;
	}
}