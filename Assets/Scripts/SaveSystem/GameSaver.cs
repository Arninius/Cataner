using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaver : MonoBehaviour {

	GameData data = GameData.current;
	float countdown = 10f;

	void Update () {
		countdown -= Time.deltaTime;
		if ((Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown ("f")) || countdown < 0) {
			data.cam_pos_x = Camera.main.transform.position.x;
			data.cam_pos_z = Camera.main.transform.position.z;
			data.cam_pos_y = Camera.main.transform.position.y;
			SaveLoad.saveCurrent ();
			Debug.Log ("saved");
			countdown = 100f;
		}
	}
}
