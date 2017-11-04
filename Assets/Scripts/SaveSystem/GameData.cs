using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameData
{
	public static GameData current;

	//World
	public float[,] elevations;
	public WorldTile[,] world_tiles;
	public float wave_height;

	//Sorry
	public float x_center, z_center;
	public int x_size, y_size, z_size;

	//Camera
	public float cam_pos_x;
	public float cam_pos_z;
	public float cam_pos_y;
	public float rotation;

	public GameData(
		int x_size,
		int y_size,
		int z_size,
		int seed,
		float frequency,
		int octaves,
		float wave_height)
	{
		this.x_size = x_size;
		this.z_size = z_size;
		this.y_size = y_size;
		this.wave_height = wave_height;

		elevations = new float[x_size + 1, z_size + 1];
		world_tiles = new WorldTile[x_size, z_size];

		Random.InitState (seed);

		int x, z;
		int sub_seed = newSeed();
		float dividend = 0;

		for (int i = 1; i <= octaves; i++) {
			float interval = i / frequency;
			float weight = 1f / i;
			dividend += weight;
			sub_seed = newSeed();
			for (x = 0; x <= x_size; x++) {
				for (z = 0; z <= z_size; z++) {
					elevations [x, z] += weight * (float)SimplexNoise.noise (x * interval, sub_seed, z * interval);
				}
			}
		}

		x_center = x_size / 2f; z_center = z_size / 2f;
		float max_dist = Mathf.Min(x_center, x_size - x_center, z_center, z_size - z_center);

		for (x = 0; x <= x_size; x++) {
			for (z = 0; z <= z_size; z++) {
				float d = Mathf.Pow(distToCenter (x, z) / max_dist, 2f);
				elevations [x, z] /= dividend / y_size;
				elevations [x, z] += (1 - 2 * d) * y_size;
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
				if (Mathf.Min (edges) < wave_height)
					wd.setColor (Color.yellow);
				else {
					float height = (edges [0] + edges [1] + edges [2] + edges [3]) / 4f / y_size;

					if (height > 1.5f)
						wd.setColor (Color.grey);

					else
						wd.setColor (0, 1.5f - height, 0);
				}
				world_tiles [x, z] = wd;
			}
		}

		cam_pos_x = x_center;
		cam_pos_z = z_center;
		cam_pos_y = y_size * 2;
	}

	int newSeed() {
		return Random.Range (int.MinValue, int.MaxValue);
	}

	public float distToCenter(float x1, float z1) {
		return Mathf.Sqrt((x1 - x_center) * (x1 - x_center) + (z1 - z_center) * (z1 - z_center));
	}
}
