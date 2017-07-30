using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailsController : MonoBehaviour {
	public GameObject tails_model;
	Animator anim;
	Rigidbody rgb;
	public float max_speed = 45f;
	float speed;
	bool is_grounded;
	// Use this for initialization
	void Start () {
		anim = GetComponentInChildren<Animator> ();
		rgb = GetComponent<Rigidbody> ();
	}

	void LateUpdate() {
		anim.SetFloat ("speed", speed);
		anim.SetBool ("grounded", is_grounded);
	}
	
	// Update is called once per frame
	void Update () {

		is_grounded = false;

		//Grounded check
		RaycastHit hit_ground;

		if (Physics.Raycast(transform.position, -transform.up, out hit_ground, 2f, LayerMask.GetMask("Default"))) {
			if (hit_ground.collider != gameObject) {
				Debug.DrawLine (transform.position, hit_ground.point, Color.cyan);
				is_grounded = true;
			}
		}

		if (SonicController.Instance.rgb.velocity.y > 0) {
			rgb.velocity = Vector3.up * 60 * Time.deltaTime;
		}

		//this is the direction in the world space we want to move:

		transform.LookAt (SonicController.Instance.transform.position);
			//transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
			//this.transform.eulerAngles = new Vector3 (angle, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
			//tails_model.transform.eulerAngles = new Vector3 (slope, sonic_model.transform.eulerAngles.y);
		//} 
		if (Vector3.Distance (this.transform.position, SonicController.Instance.transform.position) > 5) {
			if (speed < max_speed)
				speed += 1f;
			transform.Translate (Vector3.forward * speed * Time.deltaTime);
		} else {
			if (speed > 0)
				speed -= 10f;
			else
				speed = 0f;
		}
		if (Vector3.Distance (this.transform.position, SonicController.Instance.transform.position) > 50) {
			this.transform.position = new Vector3 (SonicController.Instance.transform.position.x - 10, SonicController.Instance.transform.position.y, SonicController.Instance.transform.position.z - 10); 
		}
	}
}
