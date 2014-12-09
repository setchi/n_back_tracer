using UnityEngine;
using System.Collections;

public class MainSceneUIEventListener : MonoBehaviour {

	void OnClickBackTitleButton() {
		Destroy (GameObject.Find("StorageObject"));
		Application.LoadLevel ("Title");
	}
}
