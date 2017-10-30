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
	public InputField world_seed, noise_interval;
	public InputField mesh_subdivision_size, noise_octaves;
	public InputField water_height, wave_height;

	//Load Game
	public GameObject LG_Button_Prefab;
	public Transform LG_Panel;

	void Start()
	{
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
		NG_buttons [NG_selected_menu].image.color = Color.grey;
	}

	public void generateSeed() {
		world_seed.text = System.Environment.TickCount.ToString();
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
            int.Parse (x_size.text),
            int.Parse (z_size.text),
            int.Parse (y_size.text),
            int.Parse (world_seed.text),
            float.Parse (noise_interval.text),
            int.Parse (mesh_subdivision_size.text),
            int.Parse (noise_octaves.text),
            int.Parse (water_height.text),
            float.Parse (wave_height.text)
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
}