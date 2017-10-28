using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaver : MonoBehaviour {

	GameData data = GameData.current;

	void Start() {
		InvokeRepeating ("saveGame", 300f, 300f);
	}

	void Update () {
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown ("f")) {
			saveGame ();
			Debug.Log ("saved");
		}
	}

	void saveGame () {
		data.cam_pos_x = Camera.main.transform.position.x;
		data.cam_pos_z = Camera.main.transform.position.z;
		data.cam_pos_y = Camera.main.transform.position.y;
	}
}
