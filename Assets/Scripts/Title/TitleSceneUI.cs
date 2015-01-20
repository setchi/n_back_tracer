using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TitleSceneUI : MonoBehaviour {
	public FadeManager fadeManager;
	public GameObject buttonArea;
	public GameObject textArea;
	public GameObject[] buttonElements;
	public GameObject[] textElements;

	Storage storage;
	List<GameObject> BackNumButtons;
	List<GameObject> LengthButtons;
	List<GameObject> MenuButtons;
	Image[][] screenButtonImages;
	Text[][] screenButtonTexts;
	Text[] screenTexts;
	int currentScreen = 0;
	
	// Use this for initialization
	void Awake () {
		fadeManager.FadeIn(0.4f, EaseType.easeInQuart);

		storage = GameObject.Find ("StorageObject").GetComponent<Storage>();

		BackNumButtons = Enumerable.Range(2, 3).Select(i => GameObject.Find("BackNum" + i.ToString())).ToList();
		LengthButtons = Enumerable.Range(4, 3).Select(i => GameObject.Find("Chain" + i.ToString())).ToList();
		MenuButtons = new List<GameObject> { GameObject.Find("Start"), GameObject.Find("Ranking") };
		
		screenButtonImages = buttonElements.Select(obj => obj.GetComponentsInChildren<Image>()).ToArray();
		screenButtonTexts = buttonElements.Select(obj => obj.GetComponentsInChildren<Text>()).ToArray();
		screenTexts = textElements.Select(obj => obj.GetComponent<Text>()).ToArray();

		Debug.Log (BackNumButtons.Count.ToString());
	}
	
	void TransitionIfReady() {
		if (storage.Has("Chain") && storage.Has("BackNum")) {
			fadeManager.FadeOut(0.5f, EaseType.easeInQuad, () => Application.LoadLevel ("Main"));
		}
	}
	
	void SetBackNum(int n) {
		storage.Set ("BackNum", n);
		EmitButtonAnimate(BackNumButtons, n - 2, () => {
			MoveScreen(3);
			TransitionIfReady();
		});
	}
	
	void SetChain(int length) {
		storage.Set("Chain", length);
		EmitButtonAnimate(LengthButtons, length - 4, () => MoveScreen(2));
	}
	
	void EmitButtonAnimate(List<GameObject> buttonList, int index, Action onComplete = null) {
		foreach (var go in buttonList) {
			TweenPlayer.Play(gameObject, new Tween(0.2f).ScaleTo(go, Vector3.one, EaseType.easeOutBack));
		}
		TweenPlayer.Play(gameObject, new Tween(0.2f)
		                 .ScaleTo(buttonList[index], Vector3.one * 1.3f, EaseType.easeOutBack)
		                 .Complete(onComplete));
	}

	bool isMoving = false;
	void MoveScreen(int screen) {
		if (isMoving) return;
		isMoving = true;

		var buttonAreaTransform = buttonArea.GetComponent<RectTransform>();
		var textAreaTransform = textArea.GetComponent<RectTransform>();
		var startPos = buttonAreaTransform.localPosition;
		var textAreaPos = textAreaTransform.localPosition;
		var endPos = startPos;
		endPos.x = screen * -1200;

		TweenPlayer.Play(gameObject, new Tween(0.5f)
		                 .ValueTo(startPos, endPos, EaseType.easeInOutExpo, pos => {
			buttonAreaTransform.localPosition = pos;
			textAreaPos.x = -pos.x;
			textAreaTransform.localPosition = textAreaPos;

		}).ValueTo(Vector3.zero, Vector3.one, EaseType.linear, pos => {
			var fadeIn = new Color(1, 1, 1, EaseType.easeInCubic(0, 1, pos.x));
			var fadeOut = new Color(1, 1, 1, EaseType.easeOutCubic(1, 0, pos.x));

			for (int i = 0, l = screenButtonImages[currentScreen].Length; i < l; i++) {
				screenButtonImages[currentScreen][i].color = fadeOut;
				screenButtonTexts[currentScreen][i].color = fadeOut;
			}
			screenTexts[currentScreen].color = fadeOut;
			
			if (screen == 3) return;

			for (int i = 0, l = screenButtonImages[screen].Length; i < l; i++) {
				screenButtonImages[screen][i].color = fadeIn;
				screenButtonTexts[screen][i].color = fadeIn;
			}
			screenTexts[screen].color = fadeIn;

		}).Complete(() => {
			isMoving = false;
			currentScreen = screen;
		}));
	}
	
	public void OnClickStartButton() {
		EmitButtonAnimate(MenuButtons, 0, () => MoveScreen(1));
	}
	
	public void OnClickRankingButton() {
		EmitButtonAnimate(MenuButtons, 1, () => fadeManager.FadeOut(0.4f, EaseType.easeOutQuart, () => Application.LoadLevel ("Ranking")));
	}
	
	public void OnClickNumButton(string n) {
		SetBackNum(int.Parse(n));
	}
	
	public void OnClickChainButton(string chain) {
		SetChain(int.Parse(chain));
	}
}
