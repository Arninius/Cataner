using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public Button continue_button;
	public Button load_game_button;

	//New Game
	public Button[] NG_buttons;
	public GameObject[] NG_panels;
	int NG_selected_menu;

	//GameData
	public InputField game_name;
	public InputField x_size, z_size, y_size;
	public InputField seed, frequency;
	public InputField octaves;
	public InputField wave_height;

	//Options
	public InputField submesh_size;
	public InputField wave_size;
	public InputField water_submesh_size;
	public InputField water_tiles_per_submesh;
	public InputField camera_move_speed;
	public InputField camera_zoom_speed;
	public InputField camera_rotate_speed;

	//Load Game
	public GameObject LG_Button_Prefab;
	public Transform LG_Panel;

	void Start()
	{
		if (!PlayerPrefs.HasKey ("submesh_size")) PlayerPrefs.SetInt ("submesh_size", 200);
		submesh_size.text = PlayerPrefs.GetInt ("submesh_size").ToString();

		if (!PlayerPrefs.HasKey ("wave_size")) PlayerPrefs.SetInt ("wave_size", 10);
		wave_size.text = PlayerPrefs.GetInt ("wave_size").ToString();

		if (!PlayerPrefs.HasKey ("water_submesh_size")) PlayerPrefs.SetInt ("water_submesh_size", 200);
		water_submesh_size.text = PlayerPrefs.GetInt ("water_submesh_size").ToString();

		if (!PlayerPrefs.HasKey ("water_tiles_per_submesh")) PlayerPrefs.SetInt ("water_tiles_per_submesh", 200);
		water_tiles_per_submesh.text = PlayerPrefs.GetInt ("water_tiles_per_submesh").ToString();

		if (!PlayerPrefs.HasKey ("camera_move_speed")) PlayerPrefs.SetInt ("camera_move_speed", 400);
		camera_move_speed.text = PlayerPrefs.GetInt ("camera_move_speed").ToString();

		if (!PlayerPrefs.HasKey ("camera_zoom_speed")) PlayerPrefs.SetInt ("camera_zoom_speed", 40);
		camera_zoom_speed.text = PlayerPrefs.GetInt ("camera_zoom_speed").ToString();

		if (!PlayerPrefs.HasKey ("camera_rotate_speed")) PlayerPrefs.SetInt ("camera_rotate_speed", 4);
		camera_rotate_speed.text = PlayerPrefs.GetInt ("camera_rotate_speed").ToString();

		NG_buttons [NG_selected_menu].image.color = Color.grey;

		int gameAmount = SaveLoad.getAmount ();
		if (gameAmount > 0) {
			for (int i = 0; i < gameAmount; i++) {
				string name = SaveLoad.getName (i);
				GameObject b = Instantiate (LG_Button_Prefab, LG_Panel);
				b.transform.GetChild (0).GetChild (0).GetComponent<Text>().text = name;
				b.transform.GetChild (0).GetChild (1).GetComponent<Text>().text = SaveLoad.getDate(i);
				b.transform.GetChild (1).GetChild (0).GetComponent<Button>().onClick.AddListener (() => loadGame (name));
				b.transform.GetChild (1).GetChild (1).GetComponent<Button>().onClick.AddListener (() => deleteGame (name, b));
			}
		} else {
			continue_button.interactable = false;
			load_game_button.interactable = false;
		}
	}

	public void generateSeed() {
		seed.text = System.Environment.TickCount.ToString();
	}

	public void NGselectMenu(int i) {
		NG_buttons [NG_selected_menu].image.color = Color.white;
		NG_panels [NG_selected_menu].SetActive (false);
		NG_selected_menu = i;
		NG_buttons [NG_selected_menu].image.color = Color.grey;
		NG_panels [NG_selected_menu].SetActive (true);
	}

	public void newGame() {
		GameData gd = new GameData (
			Mathf.Abs(int.Parse   (x_size.text)),
			Mathf.Abs(int.Parse   (z_size.text)),
			Mathf.Abs(int.Parse   (y_size.text)),
					  int.Parse   (seed.text),
			Mathf.Abs(float.Parse (frequency.text)),
			Mathf.Abs(int.Parse   (octaves.text)),
			Mathf.Abs(float.Parse (wave_height.text))
        );
		GameData.current = gd;
		SaveLoad.newPath (game_name.text);
		SaveLoad.saveGame ();
		SceneManager.LoadScene ("Game");
	}

	public void latestGame() {
		SaveLoad.setPathLatest ();
		SaveLoad.loadGame ();
		SceneManager.LoadScene ("Game");
	}
	
	public void loadGame(string name) {
		SaveLoad.setPath (name);
		SaveLoad.loadGame ();
		SceneManager.LoadScene ("Game");
	}

	public void deleteGame(string name, GameObject b) {
		SaveLoad.deleteGame (name);
		Destroy (b);
		if (SaveLoad.getAmount() == 0) {
			continue_button.interactable = false;
			load_game_button.interactable = false;
		}
	}

	public void changeSettings () {
		int setting;

		setting = int.Parse (submesh_size.text);
		if (setting > 0 && setting <= 254) PlayerPrefs.SetInt ("submesh_size", setting);
		else submesh_size.text = PlayerPrefs.GetInt ("submesh_size").ToString();

		setting = int.Parse (wave_size.text);
		if (setting > 0) PlayerPrefs.SetInt ("wave_size", setting);
		else wave_size.text = PlayerPrefs.GetInt ("wave_size").ToString();

		setting = int.Parse (water_submesh_size.text);
		if (setting > 0) PlayerPrefs.SetInt ("water_submesh_size", setting);
		else water_submesh_size.text = PlayerPrefs.GetInt ("water_submesh_size").ToString();

		setting = int.Parse (water_tiles_per_submesh.text);
		if (setting > 0 && setting <= 254) PlayerPrefs.SetInt ("water_tiles_per_submesh", setting);
		else water_tiles_per_submesh.text = PlayerPrefs.GetInt ("water_tiles_per_submesh").ToString();

		setting = int.Parse (camera_move_speed.text);
		if (setting >= 0) PlayerPrefs.SetInt ("camera_move_speed", setting);
		else camera_move_speed.text = PlayerPrefs.GetInt ("camera_move_speed").ToString();

		setting = int.Parse (camera_zoom_speed.text);
		if (setting >= 0) PlayerPrefs.SetInt ("camera_zoom_speed", setting);
		else camera_zoom_speed.text = PlayerPrefs.GetInt ("camera_zoom_speed").ToString();

		setting = int.Parse (camera_rotate_speed.text);
		if (setting >= 0) PlayerPrefs.SetInt ("camera_rotate_speed", setting);
		else camera_rotate_speed.text = PlayerPrefs.GetInt ("camera_rotate_speed").ToString();

		PlayerPrefs.Save ();
	}

	public void quitGame () {
		Application.Quit ();
	}
}