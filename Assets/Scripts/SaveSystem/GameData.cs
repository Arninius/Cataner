using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameData
{
	public static GameData current;

	//World
	public float[,] elevations;
	public float[,] water_times;
	public WorldTile[,] world_tiles;
	public int mesh_subdivision_size;
	public int x_size, z_size;
	public int water_height;
	public float wave_height;

	//Camera
	public float cam_pos_x;
	public float cam_pos_z;
	public float cam_pos_y;

	public GameData(
		int x_size,
		int z_size,
		int y_size,
		int seed,
		float noise_interval,
		int mesh_subdivision_size,
		int noise_octaves,
		int water_height,
		float wave_height)
	{
		noise_interval /= 4;

		this.mesh_subdivision_size = mesh_subdivision_size;
		this.x_size = x_size; this.z_size = z_size;
		this.water_height = water_height;
		this.wave_height = wave_height;

		elevations = new float[x_size + 1, z_size + 1];
		world_tiles = new WorldTile[x_size, z_size];
		water_times = new float[x_size + 1, z_size + 1];

		Random.InitState (seed);

		int x, z;
		int sub_seed = newSeed();

		for (int i = 1; i <= noise_octaves; i++) {
			sub_seed = newSeed();
			for (x = 0; x <= x_size; x++) {
				for (z = 0; z <= z_size; z++) {
					elevations [x, z] += 1f/i * (float)SimplexNoise.noise (i * x * noise_interval, sub_seed, i * z * noise_interval);
				}
			}
		}

		int x_center = x_size/2, z_center = z_size/2;
		float max_dist = Mathf.Max(x_center, x_size - x_center, z_center, z_size - z_center);

		for (x = 0; x <= x_size; x++) {
			for (z = 0; z <= z_size; z++) {
				elevations [x, z] /= 2 - 1f / noise_octaves; //auf [-1, 1] begrenzen
				elevations [x, z] += 1; elevations [x, z] /= 2; //auf [0, 1] umrechnen
				float d = dist (x, z, x_center, z_center) / max_dist;
				elevations [x, z] *= y_size;
				elevations [x, z] += water_height - (water_height) * Mathf.Pow(d, 2f);
			}
		}

		for (x = 0; x < x_size; x++) {
			for (z = 0; z < z_size; z++) {
				WorldTile wd = new WorldTile ();
				float[] edges = {
					elevations [x, z],
					elevations [x + 1, z],
					elevations [x, z + 1],
					elevations [x + 1, z + 1]
				};
				//if (Mathf.Max (edges) < water_height + wave_height)
				//	wd.setColor (Color.blue);
				if (Mathf.Min (edges) < water_height + wave_height)
					wd.setColor (Color.yellow);
				else {
					float adjusted_average = ((edges[0] + edges[1] + edges[2] + edges[3]) / 4f - water_height) / y_size;
				
					if (adjusted_average > 0.7f)
						wd.setColor (Color.grey);

					else
						wd.setColor (0, 1-adjusted_average, 0);
				}
				world_tiles [x, z] = wd;
			}
		}
			
		for (x = 0; x <= x_size; x++) {
			for (z = 0; z <= z_size; z++) {
				water_times [x, z] = dist(x, z, x_center, z_center) % (2 * Mathf.PI);
			}
		}

		cam_pos_x = x_size / 2f;
		cam_pos_z = z_size / 2f;
		cam_pos_y = 320;
	}

	int newSeed() {
		return Random.Range (int.MinValue, int.MaxValue);
	}

	float dist(int x1, int z1, int x2, int z2) {
		return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (z1 - z2) * (z1 - z2));
	}
}
