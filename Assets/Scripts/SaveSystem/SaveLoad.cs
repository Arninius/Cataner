using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {
	public static string path = Path.Combine(Application.persistentDataPath, "saves");
	static BinaryFormatter bf = new BinaryFormatter();
	static FileStream fs;

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

	public static void saveNew(string name) {
		string file_path = Path.Combine (path, name + ".gd");
		for(int i = 0; File.Exists (file_path); i++) {
			file_path = Path.Combine (path, string.Format ("{0}({1}).gd", name, i));
		}
		GameData.current.path = file_path;
		fs = File.Create (file_path);
		bf.Serialize (fs, GameData.current);
		fs.Close ();
	}

	public static void saveCurrent() {
		fs = File.Create (GameData.current.path);
		bf.Serialize (fs, GameData.current);
		fs.Close ();
	}

	public static void continueGame() {
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
	}

	public static GameData loadGame(string file_path) {
		fs = File.Open (file_path, FileMode.Open);
		GameData gd = (GameData)bf.Deserialize(fs);
		fs.Close();
		return gd;
	}
}