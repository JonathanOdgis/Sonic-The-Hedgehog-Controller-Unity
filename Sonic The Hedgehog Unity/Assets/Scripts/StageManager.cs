using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StageManager : MonoBehaviour {
	public static StageManager Instance;
	Vector3 current_sonic_spawn_position;
	int current_level;
	// Use this for initialization
	void Start () {
		Instance = this;
		if (PlayerPrefs.GetFloat ("current_pos_x") != null && PlayerPrefs.GetFloat ("current_pos_x") != null && PlayerPrefs.GetFloat ("current_pos_x") != null) {
			Debug.Log ("Apply checkpoint");
			//SonicController.Instance.transform.position = new Vector3 (PlayerPrefs.GetFloat ("current_pos_x"), PlayerPrefs.GetFloat ("current_pos_y"), PlayerPrefs.GetFloat ("current_pos_z"));
		}
		current_level = SceneManager.GetActiveScene ().buildIndex;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void set_current_position(Vector3 pos)
	{
		PlayerPrefs.SetFloat ("current_pos_x", pos.x);
		PlayerPrefs.SetFloat ("current_pos_y", pos.y);
		PlayerPrefs.SetFloat ("current_pos_z", pos.z);
		current_sonic_spawn_position = pos;



	}

	public void reload_level()
	{
		SceneManager.LoadScene(current_level);
	}
}
