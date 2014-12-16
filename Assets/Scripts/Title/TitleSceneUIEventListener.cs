using UnityEngine;
using System.Collections;

public class TitleSceneUIEventListener : MonoBehaviour {
	Storage storage;
	
	// Use this for initialization
	void Awake () {
		storage = GameObject.Find ("StorageObject").GetComponent<Storage>();
	}
	
	void TransitionIfReady() {
		if (storage.Has("Length") && storage.Has("BackNum")) {
			Application.LoadLevel ("Main");
		}
	}
	
	void SetBackNum(int n) {
		storage.Set ("BackNum", n);
		TransitionIfReady ();
	}
	
	void SetLength(int length) {
		storage.Set("Length", length);
		TransitionIfReady ();
	}
	
	void OnClickN1Button() { SetBackNum (1); }
	void OnClickN2Button() { SetBackNum (2); }
	void OnClickN3Button() { SetBackNum (3); }
	
	void OnClickL4Button() { SetLength (4); }
	void OnClickL5Button() { SetLength (5); }
	void OnClickL6Button() { SetLength (6); }
}
