using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAttackAttribute : MonoBehaviour {
	AudioSource audioSrc;
	public AudioClip destroy_audio;
	public float homing_attack_distance = 30f;
	public bool bounce_sonic;
	// Use this for initialization
	void Start () {
		audioSrc = GetComponent<AudioSource> ();	
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (this.gameObject.transform.position, SonicController.Instance.transform.position) < homing_attack_distance && !SonicController.Instance.is_homing_attack) {
			if (SonicController.Instance.homing_attack_target != null) {
				if (Vector3.Distance (SonicController.Instance.transform.position, SonicController.Instance.homing_attack_target.position) < Vector3.Distance (SonicController.Instance.transform.position, this.gameObject.transform.position)) {
					SonicController.Instance.homing_attack_target = this.gameObject.transform;
				}
			} else {
				SonicController.Instance.homing_attack_target = this.gameObject.transform;
			}
		} 
		if (Vector3.Distance (this.gameObject.transform.position, SonicController.Instance.transform.position) > homing_attack_distance && SonicController.Instance.homing_attack_target == this.gameObject.transform) {
			SonicController.Instance.homing_attack_target = null;
		}
	}

	IEnumerator OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SonicController> () && (SonicController.Instance.is_homing_attack || !SonicController.Instance.is_grounded)) {
			if (bounce_sonic) {
				if (!SonicController.Instance.is_homing_attack_success)
					StartCoroutine(SonicController.Instance.homing_attack_success_state ());
			}
			if (GetComponent<EnemyAttributes> ()) {
				audioSrc.PlayOneShot (destroy_audio);
				yield return new WaitForSeconds (.5f);
				Destroy (this.gameObject);
			}

		}
	}
}
