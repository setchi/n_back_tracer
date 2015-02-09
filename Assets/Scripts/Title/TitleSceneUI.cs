using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TitleSceneUI : MonoBehaviour {
	public FadeManager fadeManager;
	public GameObject leftArrow;
	public GameObject buttonArea;
	public GameObject textArea;
	public GameObject[] buttonElements;
	public GameObject[] textElements;

	List<GameObject> BackNumButtons;
	List<GameObject> LengthButtons;
	List<GameObject> MenuButtons;
	Image[][] screenButtonImages;
	Text[][] screenButtonTexts;
	Text[] screenTexts;
	int currentScreen = 0;

	void Awake () {
		fadeManager.FadeIn(0.4f, EaseType.easeInQuart);

		BackNumButtons = Enumerable.Range(2, 3).Select(i => GameObject.Find("BackNum" + i.ToString())).ToList();
		LengthButtons = Enumerable.Range(4, 3).Select(i => GameObject.Find("Chain" + i.ToString())).ToList();
		MenuButtons = new List<GameObject> { GameObject.Find("Start"), GameObject.Find("Ranking") };
		
		screenButtonImages = buttonElements.Select(obj => obj.GetComponentsInChildren<Image>()).ToArray();
		screenButtonTexts = buttonElements.Select(obj => obj.GetComponentsInChildren<Text>()).ToArray();
		screenTexts = textElements.Select(obj => obj.GetComponent<Text>()).ToArray();
	}

	void Update() {
		if (Input.GetKey(KeyCode.Escape)) {
			if (currentScreen > 0) {
				ResetAllButtonScale();
				SlideMenu(currentScreen - 1);
			}
		}
	}
	
	void TransSceneIfReady() {
		if (Storage.Contains("Chain") && Storage.Contains("BackNum")) {
			fadeManager.FadeOut(0.5f, EaseType.easeInQuad, () => Application.LoadLevel ("Main"));
		}
	}
	
	void SetBackNum(int n) {
		Storage.Set ("BackNum", n.ToString());
		AnimateButtonScale(BackNumButtons, n - 2, () => {
			SlideMenu(3);
			TransSceneIfReady();
		});
	}
	
	void SetChain(int length) {
		Storage.Set("Chain", length.ToString());
		AnimateButtonScale(LengthButtons, length - 4, () => SlideMenu(2));
	}

	void ResetButtonScale(List<GameObject> buttonList) {
		buttonList.ForEach(go => go.transform.localScale = Vector3.one);
	}

	void ResetAllButtonScale() {
		ResetButtonScale(BackNumButtons);
		ResetButtonScale(LengthButtons);
		ResetButtonScale(MenuButtons);
	}
	
	void AnimateButtonScale(List<GameObject> buttonList, int index, Action onComplete = null) {
		ResetButtonScale(buttonList);
		TweenPlayer.Play(gameObject, new Tween(0.13f)
		                 .ScaleTo(buttonList[index], Vector3.one * 1.3f, EaseType.easeOutBack)
		                 .Complete(onComplete));
	}

	bool isMoving = false;
	void SlideMenu(int screen) {
		if (isMoving) return;
		isMoving = true;

		if (screen > 0) {
			leftArrow.SetActive(true);

		} else {
			leftArrow.SetActive(false);
		}

		var buttonAreaTransform = buttonArea.GetComponent<RectTransform>();
		var textAreaTransform = textArea.GetComponent<RectTransform>();
		var textAreaPos = textAreaTransform.localPosition;
		var fromPosition = buttonAreaTransform.localPosition;
		var toPosition = fromPosition;
		toPosition.x = screen * -1200;

		TweenPlayer.Play(gameObject, new Tween(0.5f)
		                 .ValueTo(fromPosition, toPosition, EaseType.easeInOutExpo, pos => {
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
		AnimateButtonScale(MenuButtons, 0, () => SlideMenu(1));
	}
	
	public void OnClickRankingButton() {
		AnimateButtonScale(MenuButtons, 1, () => fadeManager.FadeOut(0.4f, EaseType.easeOutQuart, () => Application.LoadLevel ("Ranking")));
	}
	
	public void OnClickLeftArrow() {
		ResetAllButtonScale();
		SlideMenu(currentScreen - 1);
	}
	
	public void OnClickNumButton(string n) {
		SetBackNum(int.Parse(n));
	}
	
	public void OnClickChainButton(string chain) {
		SetChain(int.Parse(chain));
	}
}
