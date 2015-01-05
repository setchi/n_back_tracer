using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TitleSceneUIEventListener : MonoBehaviour {
	Storage storage;
	List<GameObject> BackNumButtons;
	List<GameObject> LengthButtons;
	
	// Use this for initialization
	void Awake () {
		storage = GameObject.Find ("StorageObject").GetComponent<Storage>();

		BackNumButtons = Enumerable.Range(1, 3).Select(i => GameObject.Find("Button" + i.ToString())).ToList();
		LengthButtons = Enumerable.Range(4, 3).Select(i => GameObject.Find("Length" + i.ToString())).ToList();

		Debug.Log (BackNumButtons.Count.ToString());
	}
	
	void TransitionIfReady() {
		if (storage.Has("Length") && storage.Has("BackNum")) {
			Application.LoadLevel ("Main");
		}
	}
	
	void SetBackNum(int n) {
		storage.Set ("BackNum", n);
		EmitButtonAnimate(BackNumButtons, n - 1);
	}
	
	void SetLength(int length) {
		storage.Set("Length", length);
		EmitButtonAnimate(LengthButtons, length - 4);
	}

	void EmitButtonAnimate(List<GameObject> buttonList, int index) {
		foreach (var go in buttonList) {
			TweenPlayer.Play(gameObject, new Tween(0.2f).ScaleTo(go, Vector3.one, EaseType.easeOutBack));
		}
		TweenPlayer.Play(gameObject, new Tween(0.2f)
		                 .ScaleTo(buttonList[index], Vector3.one * 1.3f, EaseType.easeOutBack)
		                 .Complete(() => TransitionIfReady()));
	}
	
	void OnClickN1Button() { SetBackNum (1); }
	void OnClickN2Button() { SetBackNum (2); }
	void OnClickN3Button() { SetBackNum (3); }
	
	void OnClickL4Button() { SetLength (4); }
	void OnClickL5Button() { SetLength (5); }
	void OnClickL6Button() { SetLength (6); }
}
