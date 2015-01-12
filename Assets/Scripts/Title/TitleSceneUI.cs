using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TitleSceneUI : MonoBehaviour {
	public FadeManager fadeManager;
	Storage storage;
	List<GameObject> BackNumButtons;
	List<GameObject> LengthButtons;
	
	// Use this for initialization
	void Awake () {
		fadeManager.FadeIn(0.4f, EaseType.easeInQuart);

		storage = GameObject.Find ("StorageObject").GetComponent<Storage>();

		BackNumButtons = Enumerable.Range(1, 3).Select(i => GameObject.Find("BackNum" + i.ToString())).ToList();
		LengthButtons = Enumerable.Range(4, 3).Select(i => GameObject.Find("Chain" + i.ToString())).ToList();

		Debug.Log (BackNumButtons.Count.ToString());
	}
	
	void TransitionIfReady() {
		if (storage.Has("Chain") && storage.Has("BackNum")) {
			fadeManager.FadeOut(0.4f, EaseType.easeOutQuart, () => Application.LoadLevel ("Main"));
		}
	}
	
	void SetBackNum(int n) {
		storage.Set ("BackNum", n);
		EmitButtonAnimate(BackNumButtons, n - 1);
	}
	
	void SetChain(int length) {
		storage.Set("Chain", length);
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

	public void OnClickNumButton(string n) {
		SetBackNum(int.Parse(n));
	}

	public void OnClickChainButton(string chain) {
		SetChain(int.Parse(chain));
	}
}
