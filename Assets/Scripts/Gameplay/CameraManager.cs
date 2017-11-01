using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	GameData data = GameData.current;
	const int zoomSpeed = 400;
	const int rotationSpeed = 400;
	const int moveSpeed = 32;

	void Start() {
		transform.position = new Vector3 (data.cam_pos_x, data.cam_pos_y, data.cam_pos_z);
	}

	void FixedUpdate()
	{
		if (Input.GetKey (KeyCode.LeftAlt)) {
			float force_x, force_y = 0, force_z;
			force_x = Input.GetAxis ("Horizontal") * moveSpeed;
			float scroll = Input.GetAxis ("Mouse ScrollWheel");
			force_z = Input.GetAxis ("Vertical") * moveSpeed;
			if (Input.mousePosition.x == 0)
				force_x = -moveSpeed;
			else if (Input.mousePosition.x == Screen.width - 1)
				force_x = moveSpeed;
			if (Input.mousePosition.y == 0)
				force_z = -moveSpeed;
			else if (Input.mousePosition.y == Screen.height - 1)
				force_z = moveSpeed;
			force_y = scroll * zoomSpeed;
		} else {
			if (Input.GetKey (KeyCode.LeftAlt))
				GetComponent<Rigidbody> ().AddRelativeTorque (-force_z, force_x, force_y, ForceMode.VelocityChange);
			else
				GetComponent<Rigidbody> ().AddRelativeForce (force_x, force_z, force_y, ForceMode.VelocityChange);
		}
	}
}
