using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotobugController : MonoBehaviour {


	public Camera sonic_cam;
	public GameObject motobug_model;
	public Collider collider;
	AudioSource audio;
	Animator anim;
	Rigidbody rgb;
	public float max_speed = 20.0f;
	public float speed = 0.0f;
	float spin_speed = 0.0f;
	public float max_spin_speed = 20f;
	public float max_drift_speed = 50f;
	public float jump_speed = 8.0F;
	float drift_speed;
	public float gravity = 20.0F;
	float peak_angle;
	Vector3 movement;
	Quaternion rot;
	bool is_grounded;
	bool sliding = false;
	bool homing_attacked;
	bool speed_increase = true;
	float offset_distance_forward;
	float offset_distance_backward;
	float offset_distance;
	public Transform homing_attack_target;
	public AudioClip jump_sound;
	public AudioClip spindash_sound;
	//Different Sonic States
	bool is_idle;
	bool is_standard_movement;
	bool is_boosting;
	bool is_spindashing;
	bool is_homing_attack;
	bool is_grinding;
	bool is_drifting;
	bool is_wall_colliding;
	bool is_riding;
	public GameObject ride_target;
	void Start()
	{
		audio = GetComponent<AudioSource> ();
		anim = GetComponentInChildren<Animator> ();
		rgb = GetComponent<Rigidbody> ();
	}


	void LateUpdate()
	{
		//anim.SetFloat ("movement", speed);
		//anim.SetBool ("is_jumping", !is_grounded);
		//anim.SetBool ("is_spindash", is_spindashing);
		//anim.SetBool ("is_riding", is_riding);
	}
	void FixedUpdate()
	{
		//reading the input:
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		bool jump = Input.GetKeyDown (KeyCode.Space);
		bool is_spin_charging;

		//spindash input
		if (Input.GetKey (KeyCode.LeftShift) && is_grounded) {
			is_spin_charging = true;
		} else if (Input.GetKeyUp (KeyCode.LeftShift) && is_grounded) {
			is_spin_charging = false;
		} else {
			is_spin_charging = false;
		}

		is_grounded = false;

		//Grounded check
		RaycastHit hit_ground;

		if (Physics.Raycast(transform.position, -transform.up, out hit_ground, 3f)) {
			if (hit_ground.collider != gameObject) {
				offset_distance = hit_ground.distance;
				Debug.DrawLine (transform.position, hit_ground.point, Color.cyan);
				if (offset_distance <= 5f) {
					is_grounded = true;
				} 
			}
		}

		float slope = 0;
		//Slope detect
		RaycastHit hit;
		var down_ray = new Ray (transform.position, Vector3.down);

		if (Physics.Raycast (down_ray, out hit, 2f)) {

			slope = Mathf.Atan2(hit.normal.x, hit.normal.y) * Mathf.Rad2Deg;
			Debug.DrawLine (transform.position, hit.point, Color.red);
		} 

		//rgb.useGravity = true;
		float rot_x =  this.transform.rotation.x * Mathf.Rad2Deg;
		//Debug.Log (rot_x);
		if (Mathf.Abs (slope) > 0) { 
			Debug.Log("Hit the slope");
			rgb.velocity = new Vector3 (0, 0, 0);
			//rgb.useGravity = false;
			//speed_increase = false;
			if (speed > 10) {
				speed -= .1f;
			}
			//speed = 60;
			//rgb.isKinematic = true;
		} else {
			rgb.useGravity = true;
			speed_increase = true;
			//rgb.isKinematic = false;

		}

		if (is_grounded && jump) {
			audio.PlayOneShot (jump_sound);
			rgb.velocity = transform.up * (1 + (Mathf.Abs(slope)/100)) * jump_speed;
			// speed = speed / 1.5f;

		}

		if (is_homing_attack && is_grounded) {
			is_standard_movement = true;
			is_homing_attack = false;
		}

		if (moveHorizontal != 0 || moveVertical != 0 && !is_spindashing && !is_homing_attack) {
			is_standard_movement = true;
		} 

		if (is_grounded && !is_spindashing && is_spin_charging) {
			audio.PlayOneShot (spindash_sound);
			is_spindashing = true;
		}

		if (is_grounded && !is_spindashing && is_spin_charging) {
			is_standard_movement = false;
			is_spindashing = true;
			speed = 0f;
		} 

		if (!is_grounded) {
			this.transform.eulerAngles = new Vector3 (0, this.transform.eulerAngles.y);
		}

		if (!is_spin_charging && is_spindashing) {
			is_spindashing = false;
		}

		//Movement Speed
		if (Mathf.Abs (speed) < max_speed && (Mathf.Abs (moveHorizontal) > .5f ||
			Mathf.Abs (moveVertical) > .5f) && speed_increase) {
			speed += .3f;
		} 
		else if ((Mathf.Abs (moveHorizontal) == 0 && Mathf.Abs (moveVertical) == 0) || (is_wall_colliding))
		{
			if (Mathf.Abs (speed) >= .01f)
				speed -= 1f;
			if (speed <= 0) {
				speed = 0;
			}
		}

		if (is_standard_movement) {
			//assuming we only using the single camera:
			var camera = sonic_cam;

			//camera forward and right vectors:
			var forward = camera.transform.forward;
			var right = camera.transform.right;

			//project forward and right vectors on the horizontal plane (y = 0)
			forward.y = 0f;
			right.y = 0f;
			forward.Normalize();
			right.Normalize();

			//this is the direction in the world space we want to move:
			var desiredMoveDirection = forward * moveVertical + right * moveHorizontal;

			if (desiredMoveDirection != Vector3.zero && !is_riding) {
				transform.rotation = Quaternion.RotateTowards (this.transform.rotation, Quaternion.LookRotation (desiredMoveDirection), 60000);
				transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
				//this.transform.eulerAngles = new Vector3 (angle, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
				motobug_model.transform.eulerAngles = new Vector3 (slope, motobug_model.transform.eulerAngles.y);
			} 
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
		}

	}
}