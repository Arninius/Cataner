using System.Collections;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public WorldMesh world_mesh_prefab;
	public WaterMesh water_mesh_prefab;
	GameData data = GameData.current;
	int submesh_size;
    WorldMesh[,] sub_meshes;

    void Start()
	{
		submesh_size = PlayerPrefs.GetInt("submesh_size");
		int water_submesh_size = PlayerPrefs.GetInt ("water_submesh_size");
		int water_tiles_per_submesh = PlayerPrefs.GetInt ("water_tiles_per_submesh");
		int wave_size = PlayerPrefs.GetInt ("wave_size");

		int cols = Mathf.CeilToInt(data.x_size / (float)submesh_size);
		int rows = Mathf.CeilToInt(data.z_size / (float)submesh_size);
        sub_meshes = new WorldMesh[cols, rows];

		int x, z;

        //Create Submeshes
		for (x = 0; x < cols; x++) {
			for (z = 0; z < rows; z++) {

				//Calculate Position and Size of Submesh
				IntVector2 sub_pos = new IntVector2 (x * submesh_size, z * submesh_size);
				IntVector2 sub_size = new IntVector2 (Mathf.Min (submesh_size, data.x_size - sub_pos.x), Mathf.Min (submesh_size, data.z_size - sub_pos.z));
				IntVector2 sub_vsize = sub_size + 1;

				//Generate Mesh Data
				int num_verts = sub_vsize.x * sub_vsize.z;
				Vector3[] normals = new Vector3[num_verts];
				int[] triangles = new int[sub_size.x * sub_size.z * 6];
				Texture2D texture = new Texture2D (sub_size.x, sub_size.z);
				Vector3[] vertices = new Vector3[num_verts];
				Vector2[] uv = new Vector2[num_verts];

				int sub_x, sub_z;

				//Vertices, Normals, UVs
				for (sub_x = 0; sub_x < sub_vsize.x; sub_x++) {
					for (sub_z = 0; sub_z < sub_vsize.z; sub_z++) {
						int index = sub_z * sub_vsize.x + sub_x;
						vertices [index] = new Vector3 (sub_x, data.elevations [sub_pos.x + sub_x, sub_pos.z + sub_z], sub_z);
						normals [index] = Vector3.up;
						uv [index] = new Vector2 ((float)sub_x / sub_size.x, 1f - (float)sub_z / sub_size.z);
					}
				}

				//Triangles and Texture
				for (sub_x = 0; sub_x < sub_size.x; sub_x++) {
					for (sub_z = 0; sub_z < sub_size.z; sub_z++) {
						int index = sub_z * sub_vsize.x + sub_x;
						int triOffset = (sub_z * sub_size.x + sub_x) * 6;
						triangles [triOffset + 0] = index;
						triangles [triOffset + 1] = index + sub_vsize.x;
						triangles [triOffset + 2] = index + sub_vsize.x + 1;
						triangles [triOffset + 3] = index;
						triangles [triOffset + 4] = index + sub_vsize.x + 1;
						triangles [triOffset + 5] = index + 1;
						texture.SetPixel (sub_x, sub_size.z - sub_z - 1, data.world_tiles [sub_pos.x + sub_x, sub_pos.z + sub_z].getColor ());
					}
				}
				texture.wrapMode = TextureWrapMode.Clamp;
				texture.filterMode = FilterMode.Point;
				texture.Apply ();

				//Create world mesh
				Mesh world_mesh = new Mesh ();
				world_mesh.vertices = vertices;
				world_mesh.triangles = triangles;
				world_mesh.normals = normals;
				world_mesh.uv = uv;

				//Instantiate Submesh
				WorldMesh m = Instantiate (world_mesh_prefab, new Vector3 (transform.position.x + sub_pos.x, 0, transform.position.z + sub_pos.z), transform.rotation, transform);
				m.construct (world_mesh, texture);
				sub_meshes [x, z] = m;
			}
		}

		cols = Mathf.CeilToInt(data.x_size / (float)water_submesh_size);
		rows = Mathf.CeilToInt(data.z_size / (float)water_submesh_size);
		float tile_size = (float)water_submesh_size / water_tiles_per_submesh;

		for (x = 0; x < cols; x++) {
			for (z = 0; z < rows; z++) {

				//Calculate Position and Size of Submesh
				IntVector2 sub_pos = new IntVector2(x * water_submesh_size, z * water_submesh_size);
				IntVector2 sub_size = new IntVector2(
					Mathf.Min(water_tiles_per_submesh, Mathf.CeilToInt((data.x_size - sub_pos.x) / tile_size)),
					Mathf.Min(water_tiles_per_submesh, Mathf.CeilToInt((data.z_size - sub_pos.z) / tile_size)));
				IntVector2 sub_vsize = sub_size + 1;

				//Generate Mesh Data
				int num_verts = sub_vsize.x * sub_vsize.z;
				Vector3[] normals = new Vector3[num_verts];
				int[] triangles = new int[sub_size.x * sub_size.z * 6];
				Vector3[] vertices = new Vector3[num_verts];
				float[] times = new float[num_verts];

				int sub_x, sub_z;

				//Vertices, Normals, times
				for (sub_x = 0; sub_x < sub_vsize.x; sub_x++) {
					for (sub_z = 0; sub_z < sub_vsize.z; sub_z++) {
						int index = sub_z * sub_vsize.x + sub_x;
						times [index] = (data.distToCenter(sub_pos.x + sub_x * tile_size, sub_pos.z + sub_z * tile_size) * 2 * Mathf.PI / wave_size) % (2 * Mathf.PI);
						vertices[index] = new Vector3(sub_x * tile_size, 0, sub_z * tile_size);
						normals[index] = Vector3.up;
					}
				}

				//Triangles
				for (sub_x = 0; sub_x < sub_size.x; sub_x++) {
					for (sub_z = 0; sub_z < sub_size.z; sub_z++) {
						int index = sub_z * sub_vsize.x + sub_x;
						int triOffset = (sub_z * sub_size.x + sub_x) * 6;
						triangles[triOffset + 0] = index;
						triangles[triOffset + 1] = index + sub_vsize.x;
						triangles[triOffset + 2] = index + sub_vsize.x + 1;
						triangles[triOffset + 3] = index;
						triangles[triOffset + 4] = index + sub_vsize.x + 1;
						triangles[triOffset + 5] = index + 1;
					}
				}

				//Create water mesh
				Mesh water_mesh = new Mesh();
				water_mesh.vertices = vertices;
				water_mesh.triangles = triangles;
				water_mesh.normals = normals;

				//Instantate Submesh
				WaterMesh w = Instantiate(water_mesh_prefab, new Vector3(transform.position.x + sub_pos.x, 0, transform.position.z + sub_pos.z), transform.rotation, transform);
				w.construct (water_mesh, times);
			}
		}
    }

    public void tintTexture(int pos_x, int pos_z, Color col)
    {
		int x = pos_x / submesh_size;
		int z = pos_z / submesh_size;
		sub_meshes[x, z].setTexture(pos_x % submesh_size, pos_z % submesh_size, (data.world_tiles[pos_x, pos_z].getColor() + col) / 2);
    }

    public void redoTexture(int pos_x, int pos_z)
    {
		int x = pos_x / submesh_size;
		int z = pos_z / submesh_size;
		sub_meshes[x, z].setTexture(pos_x % submesh_size, pos_z % submesh_size, data.world_tiles[pos_x, pos_z].getColor());
    }
}