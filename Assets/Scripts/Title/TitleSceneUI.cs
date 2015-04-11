using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;

public class TitleSceneUI : MonoBehaviour {
	[SerializeField] FadeManager fadeManager;
	[SerializeField] GameObject leftArrow;
	[SerializeField] GameObject buttonArea;
	[SerializeField] GameObject textArea;
	[SerializeField] GameObject[] buttonElements;
	[SerializeField] GameObject[] textElements;

	List<GameObject> BackNumButtons;
	List<GameObject> LengthButtons;
	List<GameObject> MenuButtons;
	List<List<Image>> screenButtonImages;
	List<List<Text>> screenButtonTexts;
	Text[] screenTexts;
	int currentScreen = 0;

	void Awake () {
		fadeManager.FadeIn(0.4f, Ease.InQuart);

		BackNumButtons = Enumerable.Range(2, 3).Select(i => GameObject.Find("BackNum" + i.ToString())).ToList();
		LengthButtons = Enumerable.Range(4, 3).Select(i => GameObject.Find("Chain" + i.ToString())).ToList();
		MenuButtons = new List<GameObject> { GameObject.Find("Start"), GameObject.Find("Ranking") };
		
		screenButtonImages = buttonElements.Select(obj => obj.GetComponentsInChildren<Image>().ToList()).ToList();
		screenButtonTexts = buttonElements.Select(obj => obj.GetComponentsInChildren<Text>().ToList()).ToList();
		screenTexts = textElements.Select(obj => obj.GetComponent<Text>()).ToArray();

		Observable.EveryUpdate()
			.Where(_ => Input.GetKey(KeyCode.Escape))
				.Where(_ => currentScreen > 0)
				.Subscribe(_ => {
					ResetAllButtonScale();
					SlideMenu(currentScreen - 1); });
	}
	
	void TransSceneIfReady() {
		if (Storage.Contains("Chain") && Storage.Contains("BackNum")) {
			fadeManager.FadeOut(0.5f, Ease.InQuad, () => Application.LoadLevel ("Main"));
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
		buttonList[index].transform.DOScale(Vector3.one * 1.3f, 0.13f).SetEase(Ease.InOutBack).OnComplete(() => {
			if (onComplete != null)
				onComplete();
		});
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

		var animateTime = 0.5f;
		DOTween.To(() => fromPosition, pos => {
			buttonAreaTransform.localPosition = pos;
			textAreaPos.x = -pos.x;
			textAreaTransform.localPosition = textAreaPos;
		}, toPosition, animateTime).SetEase(Ease.InOutExpo).OnComplete(() => {
			isMoving = false;
			currentScreen = screen;
		});

		screenButtonImages [currentScreen].ForEach (image => image.DOFade(0, animateTime));
		screenButtonTexts [currentScreen].ForEach (text => text.DOFade(0, animateTime));
		screenTexts[currentScreen].DOFade(0, animateTime);

		if (screen == 3) return;
		screenButtonImages[screen].ForEach(image => image.DOFade(1, animateTime));
		screenButtonTexts[screen].ForEach(text => text.DOFade(1, animateTime));
		screenTexts[screen].DOFade(1, animateTime);
	}
	
	public void OnClickStartButton() {
		AnimateButtonScale(MenuButtons, 0, () => SlideMenu(1));
	}
	
	public void OnClickRankingButton() {
		AnimateButtonScale(MenuButtons, 1, () => fadeManager.FadeOut(0.4f, Ease.OutQuart, () => Application.LoadLevel ("Ranking")));
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
