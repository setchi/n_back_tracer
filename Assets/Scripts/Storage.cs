using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Storage : MonoBehaviour {
	Dictionary<string, int> storage = new Dictionary<string, int>();

	void Awake() {
		DontDestroyOnLoad (this);
	}

	public void Set(string key, int value) {
		if (Has (key)) {
			storage[key] = value;
			return;
		}
		storage.Add (key, value);
	}

	public int Get(string key) {
		return storage [key];
	}
	
	public bool Has(string key) {
		return storage.ContainsKey(key);
	}
}
