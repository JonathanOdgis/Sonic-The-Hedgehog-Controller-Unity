using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoPlaneController : MonoBehaviour {
	public GameObject tornado_cam;
	public GameObject tornado_model;
	public GameObject sonic_model;
	public bool auto_pilot_mode;
	public bool controlled_mode;
	public float speed = 10f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		if (controlled_mode) {
			if (Input.GetKeyDown(KeyCode.Space))
			{
				controlled_mode = false;
				sonic_model.SetActive (false);
				tornado_cam.SetActive (false);
				SonicController.Instance.gameObject.SetActive (true);
				SonicController.Instance.sonic_cam.enabled = true;
				SonicController.Instance.transform.position = new Vector3(this.transform.position.x, this.transform.position.y -10, this.transform.position.z);
				SonicController.Instance.gameObject.SetActive (true);
				auto_pilot_mode = true;
			}

			//assuming we only using the single camera:
			var camera = tornado_cam;

			//camera forward and right vectors:
			var forward = camera.transform.forward;
			var right = camera.transform.right;
			var up = camera.transform.up;

			//project forward and right vectors on the horizontal plane (y = 0)
			forward.y = 0f;
			forward.y = 0f;
			right.y = 0f;
			forward.Normalize();
			right.Normalize();

			var desiredMoveDirectionUp = transform.up * moveVertical * 5000;
			var desiredMoveDirection = forward + right * moveHorizontal * 5000;

			//TODO: Up and down

			//this is the direction in the world space we want to move:


			if (desiredMoveDirection != Vector3.zero) {
				//transform.Rotate(moveVertical, this.transform.rotation.y, this.transform.rotation.z);
				transform.rotation = Quaternion.RotateTowards (this.transform.rotation, Quaternion.LookRotation (desiredMoveDirection, desiredMoveDirectionUp), 100 * Time.deltaTime);
				//this.transform.eulerAngles = new Vector3 (angle, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
			} 
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
			transform.Translate (transform.up * moveVertical * speed * Time.deltaTime);
		}
		if (auto_pilot_mode) {
			transform.Translate(Vector3.forward * 50 * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<SonicController> ()) {
			auto_pilot_mode = false;
			controlled_mode = true;
			other.GetComponent<SonicController> ().gameObject.SetActive (false);
			sonic_model.SetActive (true);
			SonicController.Instance.sonic_cam.enabled = false;
			tornado_cam.SetActive (true);
		}
	}
}
