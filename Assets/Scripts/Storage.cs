using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Storage : SingletonGameObject<Storage> {
	Dictionary<string, string> dictionary = new Dictionary<string, string>();
	
	void Awake() {
		DontDestroyOnLoad(this);
	}
	
	public static string Get(string key) {
		if (Contains(key)) {
			return Instance.dictionary[key];
		}
		return null;
	}
	
	public static void Set(string key, string value) {
		if (Contains(key)) {
			Instance.dictionary[key] = value;
			return;
		}
		
		Instance.dictionary.Add(key, value);
	}
	
	public static bool Contains(string key) {
		return Instance.dictionary.ContainsKey(key);
	}
}
