using UnityEngine;
using System.Collections;

public class MainSceneUI : MonoBehaviour {
	public FadeManager fadeManager;

	public void OnClickReturnButton() {
		fadeManager.FadeOut(0.3f, EaseType.easeInQuart, () => {
			Destroy (GameObject.Find("StorageObject"));
			Application.LoadLevel ("Title");
		});
	}
}
