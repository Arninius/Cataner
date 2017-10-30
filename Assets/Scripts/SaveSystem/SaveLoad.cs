using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {
	public static string path = Path.Combine(Application.persistentDataPath, "saves");
	public static string current_path;
	static BinaryFormatter bf = new BinaryFormatter();
	static FileStream fs;

	//GET INFORMATION

	public static int getAmount() {
		if (!Directory.Exists (path)) Directory.CreateDirectory (path);
		return Directory.GetFiles (path).Length;
	}

	public static string getName(int index) {
		return Path.GetFileNameWithoutExtension (Directory.GetFiles (path) [index]);
	}

	public static string getDate(int index) {
		System.DateTime dt = Directory.GetCreationTime (Directory.GetFiles (path) [index]);
		return dt.ToString("dd.MM.yyyy - HH:mm");
	}
		
	//SET CURRENT_PATH

	public static void newPath(string name) {
		current_path = Path.Combine (path, name + ".gd");
		for(int i = 0; File.Exists (current_path); i++) {
			current_path = Path.Combine (path, string.Format ("{0}({1}).gd", name, i));
		}
	}

	public static void setPathLatest() {
		string[] paths = Directory.GetFiles (path);
		current_path = paths [0];
		for (int i = 1; i < paths.Length; i++) {
			if (Directory.GetCreationTime (paths [i]).CompareTo (Directory.GetCreationTime(current_path)) > 0) {
				current_path = paths[i];
			}
		}
	}

	public static void setPath(string name) {
		current_path = Path.Combine (path, name + ".gd");
	}

	//USE CURRENT_PATH

	public static void loadGame() {
		fs = File.Open (current_path, FileMode.Open);
		GameData.current = (GameData)bf.Deserialize(fs);
		fs.Close();
	}

	public static void saveGame() {
		fs = File.Create (current_path);
		bf.Serialize (fs, GameData.current);
		fs.Close ();
	}

	//DELETE PATH

	public static void deleteGame(string name) {
		File.Delete (Path.Combine(path, name + ".gd"));
	}
}