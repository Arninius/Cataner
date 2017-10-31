﻿using System.Collections;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public MeshManager sub_mesh_prefab;
	public WaterTile water_tile_prefab;
	GameData data = GameData.current;
	IntVector2 size; //MÖGLICHERWEISE IN START VERLEGEN FALLS NICHT GEBRAUCHT, SONST EVTL AUCH PUBLIC MACHEN
	int submesh_size;
    MeshManager[,] sub_meshes;

    void Start()
	{
		submesh_size = PlayerPrefs.GetInt("submesh_size");
		size.x = data.world_tiles.GetLength (0);
		size.z = data.world_tiles.GetLength (1);

		int cols = Mathf.CeilToInt(size.x / (float)submesh_size);
		int rows = Mathf.CeilToInt(size.z / (float)submesh_size);
        sub_meshes = new MeshManager[cols, rows];

		int x, z;

        //Create Submeshes
        for (x = 0; x < cols; x++) {
            for (z = 0; z < rows; z++) {

                //Calculate Position and Size of Submesh
				IntVector2 sub_pos = new IntVector2(x * submesh_size, z * submesh_size);
				IntVector2 sub_size = new IntVector2(Mathf.Min(submesh_size, size.x - sub_pos.x), Mathf.Min(submesh_size, size.z - sub_pos.z));
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
						water_times [index] = data.distToCenter(sub_pos.x + sub_x, sub_pos.z + sub_z) % (2 * Mathf.PI);
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