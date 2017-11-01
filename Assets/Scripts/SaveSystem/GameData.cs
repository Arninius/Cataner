using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameData
{
	public static GameData current;

	//World
	public float[,] elevations;
	public WorldTile[,] world_tiles;
	public float x_center, z_center;
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
		float frequency,
		int octaves,
		int water_height,
		float wave_height)
	{
		this.water_height = water_height;
		this.wave_height = wave_height;

		elevations = new float[x_size + 1, z_size + 1];
		world_tiles = new WorldTile[x_size, z_size];

		Random.InitState (seed);

		int x, z;
		int sub_seed = newSeed();

		for (int i = 1; i <= octaves; i++) {
			float interval = i / frequency;
			sub_seed = newSeed();
			for (x = 0; x <= x_size; x++) {
				for (z = 0; z <= z_size; z++) {
					elevations [x, z] += 1f/i * (float)SimplexNoise.noise (x * interval, sub_seed, z * interval);
				}
			}
		}

		x_center = x_size / 2f; z_center = z_size / 2f;
		float max_dist = Mathf.Max(x_center, x_size - x_center, z_center, z_size - z_center);

		for (x = 0; x <= x_size; x++) {
			for (z = 0; z <= z_size; z++) {
				elevations [x, z] /= 2 - 1f / octaves; //auf [-1, 1] begrenzen
				elevations [x, z] += 1; elevations [x, z] /= 2; //auf [0, 1] umrechnen
				float d = distToCenter (x, z) / max_dist;
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

		cam_pos_x = x_size / 2f;
		cam_pos_z = z_size / 2f;
		cam_pos_y = 320;
	}

	int newSeed() {
		return Random.Range (int.MinValue, int.MaxValue);
	}

	public float distToCenter(int x1, int z1) {
		return Mathf.Sqrt((x1 - x_center) * (x1 - x_center) + (z1 - z_center) * (z1 - z_center));
	}
}
