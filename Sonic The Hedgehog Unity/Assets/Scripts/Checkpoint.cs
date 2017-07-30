using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
	string checkpoint_id;
	AudioSource audioSrc;
	Animator anim;
	public AudioClip audioClip;
	bool checkpoint_hit;

	// Use this for initialization
	void Start () {
		audioSrc = GetComponent<AudioSource> ();
		anim = GetComponentInChildren<Animator> ();
		if (PlayerPrefs.GetInt ("checkpoint_" + checkpoint_id) == 1) {
			checkpoint_hit = true;
		} else {
			checkpoint_hit = false;
		}
	}

	void LateUpdate()
	{
		anim.SetBool ("is_hit", checkpoint_hit);
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SonicController> ()) {
			if (!checkpoint_hit) {
				checkpoint_hit = true;
				audioSrc.PlayOneShot (audioClip);
				StageManager.Instance.set_current_position(this.transform.position);
			}
		}
	}
}
