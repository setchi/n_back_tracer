using UnityEngine;
using LitJson;
using System.IO;
using System.Collections;

public class LocalStorage {
	static string storagePath = Application.persistentDataPath + "/storage";

	public static void Write<T>(T data)where T:class {
		
		if (!Directory.Exists(storagePath)) {
			Directory.CreateDirectory(storagePath);
		}
		
		string path = storagePath + "/data";
		string json = JsonMapper.ToJson(data);
		
		File.WriteAllText(path, json, System.Text.Encoding.UTF8);
	}

	public static T Read<T>()where T:class {
		string path = storagePath + "/data";
		
		if (!Directory.Exists(storagePath)) {
			return null;
		}

		string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
		return JsonMapper.ToObject<T>(json);
	}
}
