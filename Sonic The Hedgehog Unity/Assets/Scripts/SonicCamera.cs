using UnityEngine;
using System.Collections;

public class SonicCamera : MonoBehaviour {

	public GameObject player;       //Public variable to store a reference to the player game object
	private Vector3 offset;         //Private variable to store the offset distance between the player and camera

	// Use this for initialization
	void Start () 
	{
		//Calculate and store the offset value by getting the distance between the player's position and camera's position.
		offset = transform.position - player.transform.position;
	}

	// LateUpdate is called after Update each frame
	void Update () 
	{
		//transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation,Time.deltaTime * .1f);
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position + offset, 50 * Time.deltaTime);
		//transform.rotation = Quaternion.LookRotation(player.transform.forward) * 10;
		//transform.eulerAngles = Vector3.a
		//transform.eulerAngles = Vector3.RotateTowards(this.transform.eulerAngles, new Vector3(this.transform.eulerAngles.x, player.transform.position.y, this.transform.eulerAngles.z), 1000, 10 * Time.deltaTime); //+ player.transform.position);
		//this.transform.LookAt (target.transform);
	}

}

