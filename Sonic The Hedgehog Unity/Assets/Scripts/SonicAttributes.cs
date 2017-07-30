using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicAttributes : MonoBehaviour {
	public static SonicAttributes Instance;

	int rings;
	string min;
	string sec;

	// Use this for initialization
	void Start () {
		Instance = this;
		SonicAttributes.Instance.get_time ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!SonicController.Instance.is_victory) {
			var timer = Time.time;

			var minutes = Mathf.Floor (timer / 60);
			var seconds = Mathf.RoundToInt (timer % 60);

			//Handle Minutes
			if (minutes < 10) {
				min = "0" + minutes.ToString ();
			} else {
				min = minutes.ToString ();
			}

			//Handle Seconds
			if (seconds < 10) {
				sec = "0" + Mathf.RoundToInt (seconds).ToString ();
			} else {
				sec = seconds.ToString ();
			}
		}
	}

	public int get_rings()
	{
		return rings;
	}

	public void add_rings(int ring_add)
	{
		rings += ring_add;
	}

	public void subtract_rings(int ring_diff)
	{
		rings -= ring_diff;
	}

	public string get_time()
	{
		return min + ":" + sec;
	}
}
