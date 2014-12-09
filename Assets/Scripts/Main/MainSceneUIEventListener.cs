using UnityEngine;
using System.Collections;

public class MainSceneUIEventListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void BackTitleScene() {
		Destroy (GameObject.Find("StorageObject"));
		Application.LoadLevel ("Title");
	}
}
