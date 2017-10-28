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
		int saveAmount = SaveLoad.saveAmount ();
		if (saveAmount > 0) {
			for (int i = 0; i < saveAmount; i++) {
				int index = i;
				GameObject b = Instantiate (LG_Button_Prefab, LG_Panel);
				b.transform.GetChild (0).GetChild (0).GetComponent<Text>().text = SaveLoad.getName(index);
				b.transform.GetChild (0).GetChild (1).GetComponent<Text>().text = SaveLoad.getDate(index);
				b.transform.GetChild (1).GetChild (0).GetComponent<Button>().onClick.AddListener (() => loadSave (index));
				b.transform.GetChild (1).GetChild (1).GetComponent<Button>().onClick.AddListener (() => deleteSave (index));
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

	public void NewGame() {
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
		SaveLoad.saveNew (game_name.text);
		SceneManager.LoadScene ("Game");
	}

	public void Continue() {
		SaveLoad.continueGame ();
		SceneManager.LoadScene ("Game");
	}

	public void loadSave(int index) {
		Debug.Log ("Load " + index);
	}

	public void deleteSave(int index) {
		Debug.Log ("Delete " + index);
	}
}
