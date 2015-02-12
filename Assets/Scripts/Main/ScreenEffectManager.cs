using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ScreenEffectManager : MonoBehaviour {
	public GameObject effectBackObject;
	public GameObject timeupTextObject;
	public GameObject readyTextObject;
	public GameObject goTextObject;
	public FadeManager fadeManager;

	void Awake () {
		HideAll();
	}

	void Show(params GameObject[] objects) {
		foreach (var go in objects) {
			go.SetActive(true);
		}
	}

	void Hide(params GameObject[] objects) {
		foreach (var go in objects) {
			go.SetActive(false);
		}
	}
	
	void HideAll() {
		Hide(effectBackObject, timeupTextObject, readyTextObject, goTextObject);
	}

	public void CancelAllAnimate() {
		DOTween.Kill(gameObject);
		HideAll();
	}

	public void EmitReadyAnimation() {
		Show (readyTextObject);
		var time = 0.65f;
		DOTween.Sequence()
			.Append(readyTextObject.transform.DOScale(new Vector3(1.3f, 1.3f * 0.8f, 1.3f), time).SetEase(Ease.OutCirc))
			.Append(readyTextObject.transform.DOScale(new Vector3(1, 0.8f, 1), time).SetEase(Ease.InCirc))
			.OnComplete(EmitReadyAnimation).SetId(gameObject);
	}

	public void EmitGoAnimation() {
		Show(goTextObject);
		goTextObject.transform.DOScale(new Vector3(1, 0.8f, 1), 0.7f).SetEase(Ease.OutCubic);
		goTextObject.GetComponent<Text>().DOFade(0, 0.7f).OnComplete(() => Hide(goTextObject));
	}

	public void EmitTimeupAnimation(Action onComplete) {
		StartCoroutine(StartTimeupAnimation(onComplete));
	}

	IEnumerator StartTimeupAnimation(Action onComplete) {
		Show (effectBackObject, timeupTextObject);
		var timeupText = timeupTextObject.GetComponent<Text>();

		timeupTextObject.transform.DOScale(new Vector3(1, 0.8f, 1) * 0.8f, 0.4f).SetEase(Ease.OutQuint);
		effectBackObject.transform.DOScale(new Vector3(600, 1, 0), 0.4f).SetEase(Ease.OutQuint);
		effectBackObject.transform.DORotate(Vector3.zero, 0.4f);
		yield return timeupTextObject.transform.DORotate(Vector3.zero, 0.4f).WaitForCompletion();

		yield return timeupTextObject.transform.DOScale(new Vector3(0.95f, 0.76f, 0.5f) * 0.8f, 0.5f).WaitForCompletion();
		
		fadeManager.FadeOut(1, DG.Tweening.Ease.Linear);
		effectBackObject.transform.DOScale(new Vector3(600, 0, 0), 1f).SetEase(Ease.OutExpo);
		timeupTextObject.transform.DOScale(Vector3.one * 11, 1f).SetEase(Ease.OutExpo);
		yield return timeupText.DOFade(0, 1f).SetEase(Ease.OutExpo).WaitForCompletion();

		onComplete();
	}
}
