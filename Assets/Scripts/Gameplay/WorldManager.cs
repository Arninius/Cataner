using System.Collections;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public MeshManager sub_mesh_prefab;
	public WaterTile water_tile_prefab;
    MeshManager[,] sub_meshes;
	GameData data = GameData.current;

    void Start()
	{
		int cols = Mathf.CeilToInt(data.x_size / (float)data.mesh_subdivision_size);
		int rows = Mathf.CeilToInt(data.z_size / (float)data.mesh_subdivision_size);
        sub_meshes = new MeshManager[cols, rows];

		int x, z;

        //Create Submeshes
        for (x = 0; x < cols; x++) {
            for (z = 0; z < rows; z++) {

                //Calculate Position and Size of Submesh
				IntVector2 sub_pos = new IntVector2(x * data.mesh_subdivision_size, z * data.mesh_subdivision_size);
				IntVector2 sub_size = new IntVector2(Mathf.Min(data.mesh_subdivision_size, data.x_size - sub_pos.x), Mathf.Min(data.mesh_subdivision_size, data.z_size - sub_pos.z));
                IntVector2 sub_vsize = sub_size + 1;

				//Generate Mesh Data
				int num_verts = sub_vsize.x * sub_vsize.z;
				Vector3[] normals = new Vector3[num_verts];
				int[] triangles = new int[sub_size.x * sub_size.z * 6];
				Texture2D texture = new Texture2D(sub_size.x, sub_size.z);
				Vector3[] world_vertices = new Vector3[num_verts];
				Vector3[] water_vertices = new Vector3[num_verts];
				float[] water_times = new float[num_verts];
				Vector2[] uv = new Vector2[num_verts];

                int sub_x, sub_z;

                //Vertices, Normals, UVs
                for (sub_x = 0; sub_x < sub_vsize.x; sub_x++) {
                    for (sub_z = 0; sub_z < sub_vsize.z; sub_z++) {
                        int index = sub_z * sub_vsize.x + sub_x;
						water_times [index] = data.water_times[sub_pos.x + sub_x, sub_pos.z + sub_z];
						world_vertices[index] = new Vector3(sub_x, data.elevations[sub_pos.x + sub_x, sub_pos.z + sub_z], sub_z);
						water_vertices[index] = new Vector3(sub_x, 0, sub_z);
                        normals[index] = Vector3.up;
                        uv[index] = new Vector2((float)sub_x / sub_size.x, 1f - (float)sub_z / sub_size.z);
                    }
                }

                //Triangles and Texture
                for (sub_x = 0; sub_x < sub_size.x; sub_x++) {
                    for (sub_z = 0; sub_z < sub_size.z; sub_z++) {
						int index = sub_z * sub_vsize.x + sub_x;
                        int triOffset = (sub_z * sub_size.x + sub_x) * 6;
						triangles[triOffset] = index;
						triangles[triOffset + 1] = index + sub_vsize.x;
						triangles[triOffset + 2] = index + sub_vsize.x + 1;
						triangles[triOffset + 3] = index;
						triangles[triOffset + 4] = index + sub_vsize.x + 1;
						triangles[triOffset + 5] = index + 1;
						texture.SetPixel(sub_x, sub_size.z - sub_z - 1, data.world_tiles[sub_pos.x + sub_x, sub_pos.z + sub_z].getColor());
                    }
                }
				texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Point;
                texture.Apply();

                //Create world mesh
                Mesh world_mesh = new Mesh();
				world_mesh.vertices = world_vertices;
				world_mesh.triangles = triangles;
				world_mesh.normals = normals;
				world_mesh.uv = uv;

				//Instantiate Submesh
				MeshManager m = Instantiate(sub_mesh_prefab, new Vector3(transform.position.x + sub_pos.x, 0, transform.position.z + sub_pos.z), transform.rotation, transform);
				m.construct(world_mesh, texture);
				sub_meshes[x, z] = m;

				//Create water mesh
				Mesh water_mesh = new Mesh();
				water_mesh.vertices = water_vertices;
				water_mesh.triangles = triangles;
				water_mesh.normals = normals;

				//Instantiate water_tile
				WaterTile w = Instantiate(water_tile_prefab, new Vector3(transform.position.x + sub_pos.x, data.water_height, transform.position.z + sub_pos.z), transform.rotation, transform);
				w.construct (water_mesh, water_times);
            }
        }
    }

    public void tintTexture(int pos_x, int pos_z, Color col)
    {
		int x = pos_x / data.mesh_subdivision_size;
		int z = pos_z / data.mesh_subdivision_size;
		sub_meshes[x, z].setTexture(pos_x % data.mesh_subdivision_size, pos_z % data.mesh_subdivision_size, (data.world_tiles[pos_x, pos_z].getColor() + col) / 2);
    }

    public void redoTexture(int pos_x, int pos_z)
    {
		int x = pos_x / data.mesh_subdivision_size;
		int z = pos_z / data.mesh_subdivision_size;
		sub_meshes[x, z].setTexture(pos_x % data.mesh_subdivision_size, pos_z % data.mesh_subdivision_size, data.world_tiles[pos_x, pos_z].getColor());
    }
}