using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {
	public int ring_value = 1;
	public bool is_dropped_ring;
	bool can_be_picked_up = true;
	bool picked_up;
	AudioSource audio_src;
	public GameObject ring_model;
	public AudioClip ring_pickup_sound;
	// Use this for initialization
	void Start () {
		audio_src = GetComponent<AudioSource> ();
		if (is_dropped_ring) {
			can_be_picked_up = false;
			StartCoroutine (dropped_ring_state ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SonicController> () && can_be_picked_up) {
			picked_up = true;
			audio_src.PlayOneShot (ring_pickup_sound);
			ring_model.SetActive (false);
			SonicAttributes.Instance.add_rings (ring_value);
			yield return new WaitForSeconds (.6f);
			Destroy (this.gameObject);
		}
	}

	IEnumerator dropped_ring_state ()
	{
		GetComponent<Rigidbody> ().AddForce (Vector3.up * 100);
		GetComponent<Rigidbody> ().AddForce (Vector3.right * Random.Range(-100, 100));
		//Physics.IgnoreCollision (SonicController.Instance.gameObject.GetComponent<Collider>(), this.GetComponent<BoxCollider> ());
		yield return new WaitForSeconds (1f);
		can_be_picked_up = true;
		yield return new WaitForSeconds (3f);
		var retries = 0;
		while (!picked_up && retries < 10) {
			ring_model.SetActive (true);
			yield return new WaitForSeconds (.2f);
			ring_model.SetActive (false);
			yield return new WaitForSeconds (.2f);
			ring_model.SetActive (true);
			retries++;
		}
		Destroy (this.gameObject);

	}
}
