using System.Collections;
using UnityEngine;

public class WaterTile : MonoBehaviour {

	GameData data = GameData.current;
	MeshFilter mesh_filter;
	const float speed = 1;
	float[] times;

	void Awake() {
		mesh_filter = GetComponent<MeshFilter>();
	}

	public void construct(Mesh mesh, float[] times)
	{
		this.times = times;
		mesh_filter.mesh = mesh;
	}

	void OnWillRenderObject()
	{
		Mesh mesh = mesh_filter.mesh;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i].y = Mathf.Sin (Time.time * speed + times [i]) * data.wave_height;
		}
		mesh.vertices = vertices;
	}
}