using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	GameData data = GameData.current;

	int zoom_speed;
	int move_speed;
	int rotation_speed;

	void Start() {
		zoom_speed = PlayerPrefs.GetInt ("camera_move_speed");
		move_speed = PlayerPrefs.GetInt("camera_zoom_speed");
		rotation_speed = PlayerPrefs.GetInt("camera_rotate_speed");
		transform.position = new Vector3 (data.cam_pos_x, data.cam_pos_y, data.cam_pos_z);
		transform.eulerAngles = new Vector3 (data.rotation, 0, 0);
	}

	void Update()
	{
		if (Input.GetMouseButton (1)) {
			data.rotation = Mathf.Clamp (data.rotation + Input.GetAxis ("Mouse Y") * rotation_speed, 0, 90);
			transform.eulerAngles = new Vector3 (data.rotation, 0, 0);
		}
	}

	void FixedUpdate()
	{
		float force_x, force_y, force_z;
		if (Input.mousePosition.x == 0) {
			force_x = -move_speed;
		} else if (Input.mousePosition.x == Screen.width - 1) {
			force_x = move_speed;
		} else {
			force_x = Input.GetAxis ("Horizontal") * move_speed;
		}
		if (transform.position.x <= 0)
			force_x = Mathf.Max (0, force_x);
		else if (transform.position.x > data.x_size)
			force_x = Mathf.Min (0, force_x);

		if (Input.mousePosition.y == 0) {
			force_z = -move_speed;
		} else if (Input.mousePosition.y == Screen.height - 1) {
			force_z = move_speed;
		} else {
			force_z = Input.GetAxis ("Vertical") * move_speed;
		}
		if (transform.position.z <= 0)
			force_z = Mathf.Max (0, force_z);
		else if (transform.position.z > data.z_size)
			force_z = Mathf.Min (0, force_z);

		force_y = Input.GetAxis ("Mouse ScrollWheel") * zoom_speed;
		if (transform.position.y >= data.y_size * 2 + 200)
			force_y = Mathf.Min (0, force_y);
				
		GetComponent<Rigidbody> ().AddForce (force_x, 0, force_z, ForceMode.VelocityChange);
		GetComponent<Rigidbody> ().AddForce (0, force_y, 0, ForceMode.VelocityChange);
	}
}
