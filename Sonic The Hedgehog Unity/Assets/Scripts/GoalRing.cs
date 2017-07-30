using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalRing : MonoBehaviour {
	public GameObject goal_ring_model;
	AudioSource audioSrc;
	public AudioClip idle_ring_audio;
	public AudioClip touched_ring_audio;
	bool shrink_ring;
	// Use this for initialization
	void Start () {
		audioSrc = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (shrink_ring) {
			if (goal_ring_model.transform.localScale.x > 0 && goal_ring_model.transform.localScale.y > 0 && goal_ring_model.transform.localScale.z > 0) {
				float x = goal_ring_model.transform.localScale.x - .1f;
				float y = goal_ring_model.transform.localScale.y - .1f;
				float z = goal_ring_model.transform.localScale.z - .1f;
				goal_ring_model.transform.localScale = new Vector3 (x, y, z);
			} else {
				shrink_ring = false;
			}

		}
	}

	IEnumerator destroy_state () {
		shrink_ring = true;
		yield return new WaitForSeconds (1f);
	}

	IEnumerator OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SonicController> () && !SonicController.Instance.is_victory) {
			StartCoroutine(destroy_state ());
			audioSrc.Stop ();
			audioSrc.PlayOneShot (touched_ring_audio);
			SonicController.Instance.controls_enabled = false;
			//SonicCam.Instance.is_victory_mode = true;
			SonicController.Instance.is_victory = true;
			MusicManager.Instance.set_audio (MusicManager.Instance.results_music, false);
			StartCoroutine(destroy_state ());
			yield return new WaitForSeconds (10f);
			SceneManager.LoadScene ("title_screen");

		}
	}
}
