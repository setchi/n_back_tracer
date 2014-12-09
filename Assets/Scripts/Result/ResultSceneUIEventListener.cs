using UnityEngine;
using System.Collections;

public class ResultSceneUIEventListener : MonoBehaviour {

	void OnClickGoBackButton() {
		Destroy (GameObject.Find ("StorageObject"));
		Application.LoadLevel ("Title");
	}
}
