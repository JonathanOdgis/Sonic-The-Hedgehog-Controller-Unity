using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicController : MonoBehaviour {

	public static SonicController Instance;

	//Components
	public Camera sonic_cam;
	public GameObject sonic_model;
	public Collider collider;
	AudioSource audio;
	public Animator anim;
	public Rigidbody rgb;

	//Physics and Movement variables
	public float max_speed = 20.0f;
	public float speed = 0.0f;
	public float acceleration = .5f;
	float spin_speed = 0.0f;
	public float turn_speed = 200f;
	public float side_scroll_turn_speed = 10000f;
	public float side_scroll_z;
	public float max_spin_speed = 20f;
	public float max_drift_speed = 50f;
	public float jump_speed = 8.0F;
	public float free_fall_speed = 30f;
	public float spring_speed = 200;
	float drift_speed;
	public float gravity = 20.0F;
	float peak_angle;
	Vector3 movement;
	Quaternion rot;
	public bool is_grounded;
	bool sliding = false;
	bool homing_attacked;
	bool speed_increase = true;
	float offset_distance_forward;
	float offset_distance_backward;
	float offset_distance;
	float time_since_last_spring_collision;
	float time_left_in_spring_sequence;
	float time_since_started_homing;
	public float max_homing_attack_time = 1f;
	public float slope;
	public Vector3 starting_pos;
	public Transform homing_attack_target;
	Vector3 original_scale;

	//Sound Effects
	public AudioClip jump_sound;
	public AudioClip spindash_sound;
	public AudioClip homing_attack_sound;
	public AudioClip running_sound;

	//Particle Systems
	public ParticleSystem running_effect;
	public ParticleSystem homing_attack_effect;

	//Target Game Objects for Homing Attacks, Grinding, Spring Collisions
	public GameObject ride_target;
	public GameObject spring_target;
	public GameObject sonic_dropped_ring;

	//Path Following Variables
	public EditorPath PathToFollow;

	//current index in the pathtofollow
	public int currentWayPointID;

	//If the player is there and it's near the point and it won't be as hard and more curvy in moving with the points (0) is hard (1) is a bit more curved
	private float reachDistance = 1.0f;
	public float rotationSpeed = 5.0f;
	public string pathName;
	Vector3 last_position;
	Vector3 current_position;
	Quaternion rotation = new Quaternion();

	public Dictionary<string, bool> sonic_states = new Dictionary<string, bool>();

	//Different Sonic States
	bool is_idle;
	bool is_standard_movement;
	bool is_boosting;
	bool is_stunned;
	bool is_dead;
	public bool is_spindashing;
	public bool is_homing_attack;
	public bool is_homing_attack_success;
	public bool is_grinding;
	bool is_drifting;
	bool is_wall_colliding;
	bool is_riding;
	bool is_springing;
	bool breaking;
	bool going_fast;
	bool horizontal_held_down;
	bool vertical_held_down;
	public bool is_looping;
	public bool is_free_falling;
	public bool is_victory;
	public bool controls_enabled;
	public bool is_side_scrolling;
	bool allow_vertical = true;

	void Start()
	{
		Instance = this;
		this.transform.parent = null;
		audio = GetComponent<AudioSource> ();
		anim = GetComponentInChildren<Animator> ();
		rgb = GetComponent<Rigidbody> ();
		original_scale = new Vector3 (0.07f, 0.07f, 0.07f);
		//Add all the states to a list for easy disable of all but one.
		sonic_states.Add ("is_idle", is_idle);
		sonic_states.Add ("is_standard_movement", is_standard_movement);
		sonic_states.Add ("is_boosting", is_boosting);
		sonic_states.Add ("is_spindashing", is_spindashing);
		sonic_states.Add ("is_homing_attack", is_homing_attack);
		sonic_states.Add ("is_grinding", is_grinding);
		sonic_states.Add ("is_drifting", is_drifting);
		sonic_states.Add ("is_wall_colliding", is_wall_colliding);
		sonic_states.Add ("is_riding", is_riding);
		sonic_states.Add ("is_springing", is_springing);

		if (starting_pos != Vector3.zero) {
			this.transform.position = starting_pos;
		}
	}


	void LateUpdate()
	{
		anim.SetFloat ("movement", speed);
		anim.SetBool ("is_jumping", !is_grounded && !is_looping);
		anim.SetBool ("is_spindash", is_spindashing);
		anim.SetBool ("is_riding", is_riding);
		anim.SetBool ("is_grinding", is_grinding);
		anim.SetBool ("is_springing", is_springing);
		anim.SetBool ("is_free_falling", is_free_falling && !is_grounded);
		anim.SetBool ("is_victory", is_victory && is_grounded);
		anim.SetBool ("is_hit", is_stunned);
		anim.SetBool ("is_dead", is_dead);

	}
	void FixedUpdate()
	{
		//Reading the Input
		float moveHorizontal = 0;
		float moveVertical = 0;
		bool jump = false;
		bool free_fall = false;
		bool is_spin_charging = false;

		if (controls_enabled) {
			if (Input.GetAxis ("Horizontal") != 0) {
				horizontal_held_down = true;
			} else {
				horizontal_held_down = false;
			}


			if (Input.GetAxis ("Vertical") != 0) {
				vertical_held_down = true;
				if (!is_side_scrolling)
					allow_vertical = true;
			} else {
				vertical_held_down = false;
			}

			if (!vertical_held_down && is_side_scrolling && allow_vertical) {
				allow_vertical = false;
			}
	

			moveHorizontal = Input.GetAxis ("Horizontal");

			if (!is_side_scrolling)
				moveVertical = Input.GetAxis ("Vertical");
			if (is_side_scrolling && allow_vertical)
				moveHorizontal = Input.GetAxis ("Vertical");

			jump = Input.GetKeyDown (KeyCode.Space);
			free_fall = Input.GetKey (KeyCode.E) && !is_grounded;

			//Spindash Input
			if (Input.GetKey (KeyCode.LeftShift) && is_grounded) {
				is_spin_charging = true;
			} else if (Input.GetKeyUp (KeyCode.LeftShift) && is_grounded) {
				is_spin_charging = false;
			} else {
				is_spin_charging = false;
			}
		}
		is_grounded = false;

		//Grounded check
		RaycastHit hit_ground;

		if (Physics.Raycast(transform.position, -transform.up, out hit_ground, 1f, LayerMask.GetMask("Default"))) { //1 for the distance orignally
			if (hit_ground.collider != gameObject) {
				offset_distance = hit_ground.distance;
				Debug.DrawLine (transform.position, hit_ground.point, Color.cyan);
				if (offset_distance <= 5f) {
					is_grounded = true;
				} 
			}
		}

		is_wall_colliding = false;

		//Grounded check
		RaycastHit hit_wall;

		if (Physics.Raycast(transform.position, transform.forward, out hit_wall, 2f)) {
			Debug.DrawLine (transform.position, hit_wall.point, Color.green);
			if (hit_wall.collider != gameObject) {
				Debug.DrawLine (transform.position, hit_wall.point, Color.green);
				is_wall_colliding = true;
			}
		}

		//float slope = 0;
		slope = 0;
		//Slope detect
		RaycastHit hit;
		var down_ray = new Ray (transform.position, -transform.up); //this.transform.down was Vector3.down

		float ray_dist = 1.1f;
		if (is_looping) {
			ray_dist = 7f;
		}

		if (Physics.Raycast (down_ray, out hit, ray_dist)) {
			slope = Mathf.Atan2(hit.normal.x, hit.normal.y) * Mathf.Rad2Deg;
			Debug.DrawLine (transform.position, hit.point, Color.red);
		} 

		if (Mathf.Abs (slope) > 0 && speed > 0) { 
			if (Mathf.Abs (slope) > 40) {
				rgb.useGravity = false;
				rgb.velocity = new Vector3 (0, 0, 0);
			} else {
				rgb.useGravity = true;
			}
		} else {
			rgb.useGravity = true;
			speed_increase = true;
		}

		if (is_grounded) {
			is_homing_attack = false;
			is_homing_attack_success = false;
		}

		if (is_grounded && jump) {
			audio.PlayOneShot (jump_sound);
			rgb.velocity = transform.up * jump_speed;
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
			if (!is_looping)
				this.transform.eulerAngles = new Vector3 (0, this.transform.eulerAngles.y);
			moving_platform_target = null;
		}

		if (!is_spin_charging && is_spindashing) {
			is_spindashing = false;
		}

		//Movement Speed
		if (Mathf.Abs (speed) < max_speed && (Mathf.Abs (moveHorizontal) > .5f ||
			Mathf.Abs (moveVertical) > .5f) && speed_increase) {
			speed += acceleration;
		} 
		else if ((Mathf.Abs (moveHorizontal) == 0 && Mathf.Abs (moveVertical) == 0))
		{
			if (Mathf.Abs (speed) >= .01f)
				speed -= 10f;
			if (speed <= 0) {
				speed = 0;
			}
		}

		if (is_wall_colliding && speed > max_speed /2) {
			speed -= 2f;
			if (speed <= 0) {
				speed = 0;
			}
		}

		if (free_fall && !is_looping && !is_springing && !is_homing_attack) {
			is_free_falling = true;
		} else {
			is_free_falling = false;

		}
			
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

		var desiredMoveDirection = forward * moveVertical + right * moveHorizontal;
		//this is the direction in the world space we want to move:

		if (is_standard_movement && !is_grinding && !is_looping && !is_free_falling) {
			if (desiredMoveDirection != Vector3.zero && !is_riding) {
				if (!is_side_scrolling)
					transform.rotation = Quaternion.RotateTowards (this.transform.rotation, Quaternion.LookRotation (desiredMoveDirection), (turn_speed * speed) * Time.deltaTime);
				else {
					transform.rotation = Quaternion.RotateTowards (this.transform.rotation, Quaternion.LookRotation (desiredMoveDirection), side_scroll_turn_speed);
				}
			} 
			transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
			sonic_model.transform.eulerAngles = new Vector3 (slope, sonic_model.transform.eulerAngles.y);
			if(is_grounded)
				transform.Translate(Vector3.forward * speed * Time.deltaTime);
			else
				transform.Translate(Vector3.forward * (speed/1.6f) * Time.deltaTime);
			if (is_side_scrolling)
				this.transform.position = new Vector3(transform.position.x, transform.position.y, side_scroll_z);
		}

		if (is_looping) {
			if (jump) {
				is_looping = false;
				currentWayPointID = 0;
				return;
			}
			speed = max_speed;
			rgb.useGravity = false;
			//Use the editor paths
			if (currentWayPointID != PathToFollow.path_objs.Count && currentWayPointID >= 0) {
				float distance = Vector3.Distance (PathToFollow.path_objs [currentWayPointID].position, transform.position);
				this.transform.position = Vector3.MoveTowards (transform.position, PathToFollow.path_objs [currentWayPointID].position, speed * Time.deltaTime);

				transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
				sonic_model.transform.eulerAngles = new Vector3 (slope, sonic_model.transform.eulerAngles.y);

				if (distance <= reachDistance) {
					currentWayPointID++;
					if (currentWayPointID < PathToFollow.path_objs.Count - 1)
						currentWayPointID++;
				} 
			} else {
				is_looping = false;
				currentWayPointID = 0;
			}
		} 

		if (is_grinding) {
			if (jump) {
				is_grinding = false;
				currentWayPointID = 0;
				return;
			}
			speed = max_speed;
			rgb.useGravity = false;
			//Use the editor paths
			if (currentWayPointID != PathToFollow.path_objs.Count && currentWayPointID >= 0) {
				float distance = Vector3.Distance (PathToFollow.path_objs [currentWayPointID].position, transform.position);
				if (distance <= reachDistance) {
					currentWayPointID++;
					if (currentWayPointID < PathToFollow.path_objs.Count - 1)
						currentWayPointID++;
				} 
				transform.rotation = Quaternion.RotateTowards (this.transform.rotation, Quaternion.LookRotation (desiredMoveDirection), 600000);
				transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
				sonic_model.transform.eulerAngles = new Vector3 (slope, sonic_model.transform.eulerAngles.y);
			
				transform.position = Vector3.MoveTowards (transform.position, PathToFollow.path_objs [currentWayPointID].position, speed * Time.deltaTime);
				//rotation = Quaternion.LookRotation (transform.position - PathToFollow.path_objs [currentWayPointID].position);
			} else {
				is_grinding = false;
				currentWayPointID = 0;
			}
		}

		if (is_free_falling) {
			SonicCam.Instance.y_offset = 2f;
			rgb.velocity = new Vector3 (rgb.velocity.x, -free_fall_speed, rgb.velocity.z);
			if (desiredMoveDirection != Vector3.zero && !is_riding) {
				transform.rotation = Quaternion.RotateTowards (this.transform.rotation, Quaternion.LookRotation (desiredMoveDirection), 600000);
				transform.rotation = Quaternion.FromToRotation (transform.up, hit.normal) * transform.rotation;
				//this.transform.eulerAngles = new Vector3 (angle, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
				sonic_model.transform.eulerAngles = new Vector3 (slope, sonic_model.transform.eulerAngles.y);
			} 
			transform.Translate (Vector3.forward * speed * Time.deltaTime);
		} else {
			
		}

		if (is_spindashing) {
			if (spin_speed < max_spin_speed) {
				spin_speed += 1f;
			}
			transform.Translate(Vector3.forward * spin_speed * Time.deltaTime);
		}

		if (!is_grounded && jump && !is_homing_attack && homing_attack_target) {
			audio.PlayOneShot (homing_attack_sound);
			is_homing_attack = true;
			set_anti_physics ();
		}

		if (speed > 20 && is_grounded) {
			if (audio.clip != running_sound && !audio.isPlaying) {
				//audio.clip = running_sound;
				//audio.Play ();
			}
			if (!running_effect.isPlaying) {
				running_effect.Play ();
			}
		} else {
			if (audio.clip == running_sound && audio.isPlaying) {
				//audio.clip = null;
				//audio.Stop ();
			}
			if (running_effect.isPlaying) {
				running_effect.Stop ();
			}
		}

		if (is_homing_attack && !is_homing_attack_success) {
			if (!homing_attack_effect.isPlaying) {
				homing_attack_effect.Play ();
			}
			time_since_started_homing += .01f;
			if (homing_attack_target == null)
				rgb.velocity = transform.forward * 1000 * Time.deltaTime;
			else {
				rgb.useGravity = false;
				this.transform.rotation = Quaternion.RotateTowards (this.transform.rotation, Quaternion.LookRotation (homing_attack_target.transform.position, homing_attack_target.transform.position), 60000);
				this.transform.position = Vector3.MoveTowards (this.transform.position, homing_attack_target.transform.position, 300 * Time.deltaTime);
				//Make a time based on the actual distance at that moment (spring_time = d/v)
				if (Vector3.Distance(this.transform.position,homing_attack_target.transform.position) < .1f || (is_grounded && time_since_started_homing > max_homing_attack_time)) {
					is_homing_attack = false;
					time_since_started_homing = 0f;
				}
			}
		} else {
			homing_attack_effect.Stop ();
		}
		
		if (is_springing ) {
			time_since_last_spring_collision += .01f;
			is_homing_attack = false;
			is_standard_movement = false;
			rgb.useGravity = false;
			this.transform.rotation = Quaternion.RotateTowards (this.transform.rotation, spring_target.transform.rotation, 60000);
			this.transform.position = Vector3.MoveTowards (this.transform.position, spring_target.transform.position, spring_speed * Time.deltaTime);

			if ((Vector3.Distance(this.transform.position, spring_target.transform.position) < 5) || (is_grounded && time_since_last_spring_collision > .2f)) { //|| (!is_grounded && time_since_last_spring_collision >= time_left_in_spring_sequence)) {
				is_springing = false;
				time_since_last_spring_collision = 0f;
				time_left_in_spring_sequence = 0f;
			}
		} else {
			is_standard_movement = true;
			if (!is_looping)
				rgb.useGravity = true;
		}

		if (is_riding) {
			is_standard_movement = false;
			rgb.useGravity = false;
			collider.enabled = false;
			this.transform.position = ride_target.transform.position;
		}

	}

	public void set_anti_physics()
	{
		rgb.velocity = Vector3.zero;
		is_standard_movement = false;
	}

	public void initialize_spring_state()
	{
		set_anti_physics ();
		is_springing = true;
	}

	public IEnumerator homing_attack_success_state ()
	{
		Debug.Log ("begin success");
		is_homing_attack_success = true;
		is_homing_attack = false;
		rgb.velocity = Vector3.up * 50f;
		homing_attack_target = null;
		yield return new WaitForSeconds (.1f);
		is_homing_attack_success = false;
		Debug.Log ("end success");
	}

	public IEnumerator hit_state()
	{
		if (SonicAttributes.Instance.get_rings() > 0)
		{
			controls_enabled = false;
			is_stunned = true;
			drop_rings (SonicAttributes.Instance.get_rings()/4);
			SonicAttributes.Instance.subtract_rings (SonicAttributes.Instance.get_rings ());
			rgb.AddForce (-this.transform.forward * 1000);
			rgb.AddForce (this.transform.up * 1000);
			yield return new WaitForSeconds (1f);
			is_stunned = false;
			controls_enabled = true;
		}
		else
		{
			StartCoroutine (dead_state ());
		}
	}

	public IEnumerator dead_state() {
		controls_enabled = false;
		is_stunned = true;
		is_dead = true;
		rgb.AddForce (-this.transform.forward * 1000);
		rgb.AddForce (this.transform.up * 1000);
		yield return new WaitForSeconds (3f);
		StageManager.Instance.reload_level ();
	}


	void drop_rings(int rings)
	{
		for (int i = 0; i <= rings; i++)
		{
			var clone = Instantiate (sonic_dropped_ring.gameObject, this.transform.position, this.transform.rotation);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		//Locked mode camera target
		if (other.gameObject.layer == 10) {
			SonicCam.Instance.standard_mode = false;

			SonicCam.Instance.locked_mode = true;
			SonicCam.Instance.locked_mode_target = other.gameObject.transform.GetChild (0).gameObject;
		}
		//Enemy 
		if (other.gameObject.GetComponent<EnemyAttributes>() && !is_stunned && !is_dead) {
			if (!is_spindashing && !is_homing_attack && is_grounded)
				StartCoroutine(hit_state ());
		}

	}
	GameObject moving_platform_target;
	Vector3 offset;
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 0) {

		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.layer == 0) {
		}
	}

	/*
	void disable_all_sonic_states(string[] exception_states)
	{
		foreach (string state in sonic_states.Keys) {
			foreach (string exception_state in exception_states) {
				if (exception_state == state) {

				} else {
					sonic_states.TryGetValue (state, false);
				}
			}
		}
	}
	*/
}