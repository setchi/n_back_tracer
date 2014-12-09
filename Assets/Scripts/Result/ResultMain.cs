using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultMain : MonoBehaviour {
	Storage storage;

	void Awake() {
		GameObject storageObject = GameObject.Find ("StorageObject");
		storage = storageObject ? storageObject.GetComponent<Storage>() : null;

		if (storage && storage.Has ("Score")) {
			GameObject.Find ("Score").GetComponent<Text>().text = "Score: " + storage.Get("Score").ToString();
		}
	}
}
