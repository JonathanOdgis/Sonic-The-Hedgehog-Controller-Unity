using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageUIBehavior : MonoBehaviour {
	bool is_moving;
	// Use this for initialization
	void Start () {
		StartCoroutine (UIAnimation ());
	}
	
	// Update is called once per frame
	void Update () {
		if (is_moving) {
			this.transform.Translate (-transform.right * 1000 * Time.deltaTime);
		}
	}

	IEnumerator UIAnimation() {
		yield return new WaitForSeconds (1f);
		is_moving = true;
		yield return new WaitForSeconds (4f);
		Destroy (this.gameObject);
	}

}
