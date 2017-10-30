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

	//DETAILS
	public static int saveAmount() {
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
		
	public static void newGame(string name) {
		string file_path = Path.Combine (path, name + ".gd");
		for(int i = 0; File.Exists (file_path); i++) {
			file_path = Path.Combine (path, string.Format ("{0}({1}).gd", name, i));
		}
		current_path = file_path;
	}

	public static void latestGame() {
		string[] paths = Directory.GetFiles (path);
		string file_path = paths [0];
		for (int i = 1; i < paths.Length; i++) {
			if (Directory.GetCreationTime (paths [i]).CompareTo (Directory.GetCreationTime(file_path)) > 0) {
				file_path = paths[i];
			}
		}
		fs = File.Open (file_path, FileMode.Open);
		GameData.current = (GameData)bf.Deserialize (fs);
		fs.Close ();
		current_path = file_path;
	}

	public static void loadGame(string name) {
		current_path = Path.Combine (path, name + ".gd");
		fs = File.Open (current_path, FileMode.Open);
		GameData.current = (GameData)bf.Deserialize(fs);
		fs.Close();
	}

	public static void saveGame() {
		fs = File.Create (current_path);
		bf.Serialize (fs, GameData.current);
		fs.Close ();
	}

	public static void deleteGame(string name) {
		File.Delete (Path.Combine(path, name + ".gd"));
	}
}