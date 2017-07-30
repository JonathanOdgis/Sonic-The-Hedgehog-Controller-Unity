using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTerrainHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(RemoveGrass(GetComponent<Terrain>()));
	}
	
	// Update is called once per frame
	void Update () {

	}

	IEnumerator RemoveGrass(Terrain t)
	{
		while (true) {
			yield return new WaitForSeconds (10f);
			// Get all of layer zero.
			var map = t.terrainData.GetDetailLayer (0, 0,
				         t.terrainData.detailWidth, t.terrainData.detailHeight, 0);

			// For each pixel in the detail map...
			for (var y = 0; y < t.terrainData.detailHeight; y++) {
				for (var x = 0; x < t.terrainData.detailWidth; x++) {
					//Debug.Log (x + "," + y);
					if (SonicController.Instance.transform.position.x == x) {
						Debug.Log ("Sonic Hit");
						map [x, y] = 0;
					}
				}
			}

			// Assign the modified map back.
			t.terrainData.SetDetailLayer (0, 0, 0, map);
		}
	}
}
