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
	public InputField water_height, wave_height;

	//Options
	public InputField submesh_size;
	public InputField camera_move_speed;
	public InputField camera_zoom_speed;
	public InputField camera_rotate_speed;

	//Load Game
	public GameObject LG_Button_Prefab;
	public Transform LG_Panel;

	void Start()
	{
		if (PlayerPrefs.GetInt ("submesh_size") == 0)
			PlayerPrefs.SetInt ("submesh_size", 200);
		submesh_size.text = PlayerPrefs.GetInt ("submesh_size").ToString();
		if (PlayerPrefs.GetInt ("camera_move_speed") == 0)
			PlayerPrefs.SetInt ("camera_move_speed", 400);
		camera_move_speed.text = PlayerPrefs.GetInt ("camera_move_speed").ToString();
		if (PlayerPrefs.GetInt ("camera_zoom_speed") == 0)
			PlayerPrefs.SetInt ("camera_zoom_speed", 40);
		camera_zoom_speed.text = PlayerPrefs.GetInt ("camera_zoom_speed").ToString();
		if (PlayerPrefs.GetInt ("camera_rotate_speed") == 0)
			PlayerPrefs.SetInt ("camera_rotate_speed", 4);
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
            int.Parse (x_size.text),
            int.Parse (z_size.text),
            int.Parse (y_size.text),
            int.Parse (seed.text),
            float.Parse (frequency.text),
            int.Parse (octaves.text),
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

	public void changeSettings () {
		PlayerPrefs.SetInt ("submesh_size", Mathf.Clamp(int.Parse(submesh_size.text), 10, 254));
		PlayerPrefs.SetInt ("camera_move_speed", Mathf.Abs(int.Parse(camera_move_speed.text)));
		PlayerPrefs.SetInt ("camera_zoom_speed", Mathf.Abs(int.Parse(camera_zoom_speed.text)));
		PlayerPrefs.SetInt ("camera_rotate_speed", Mathf.Abs(int.Parse(camera_rotate_speed.text)));
		PlayerPrefs.Save ();
	}

	public void quitGame () {
		Application.Quit ();
	}
}