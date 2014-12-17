using UnityEngine;
using System;
using System.Collections.Generic;

public class ScreenEffectManager : MonoBehaviour {
	GameObject effectBackObject;
	GameObject timeupTextObject;
	GameObject readyTextObject;
	GameObject goTextObject;
	SpriteRenderer maskSpriteRenderer;
	TweenExecutor tweenExecutor;

	// Use this for initialization
	void Awake () {
		effectBackObject = GameObject.Find("EffectBack");
		timeupTextObject = GameObject.Find("TimeupText");
		readyTextObject = GameObject.Find("ReadyText");
		goTextObject = GameObject.Find("GoText");
		maskSpriteRenderer = GameObject.Find("Mask").GetComponent<SpriteRenderer>();
		tweenExecutor = new TweenExecutor();
		HideAll();
	}

	void Update() {
		tweenExecutor.Update();
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
		effectBackObject.SetActive(false);
		timeupTextObject.SetActive(false);
		readyTextObject.SetActive(false);
		goTextObject.SetActive(false);
	}

	public void CancelAllAnimate() {
		tweenExecutor.CancelAll();
		HideAll();
	}

	public void EmitReadyAnimation() {
		Show (readyTextObject);

		tweenExecutor.SeriesExecute(
			new Tween(0.65f)
				.ScaleTo(readyTextObject, new Vector3(1.3f, 1.3f * 0.8f, 1.3f), EaseType.easeOutCirc),

			new Tween(0.65f)
				.ScaleTo(readyTextObject, new Vector3(1.3f, 1.3f * 0.8f, 1.3f), new Vector3(1, 0.8f, 1), EaseType.easeInCirc)
				.Complete(() => EmitReadyAnimation())
		);
	}

	public void EmitGoAnimation() {
		Show(goTextObject);
		var spriteRenderer = goTextObject.GetComponent<SpriteRenderer>();

		tweenExecutor.CancelAll();
		tweenExecutor.SeriesExecute(
			new Tween(0.7f)
				.ScaleTo(goTextObject, new Vector3(1, 0.8f, 1), EaseType.easeOutCirc)
				.FadeTo(spriteRenderer, 0, EaseType.linear)
				.Complete(() => Hide(goTextObject))
		);
	}

	public void EmitTimeupAnimation(Action callback) {
		Show (effectBackObject, timeupTextObject);
		var textSpriteRenderer = timeupTextObject.GetComponent<SpriteRenderer>();

		tweenExecutor.SeriesExecute(
			new Tween(0.4f)
				.ScaleTo(timeupTextObject, Vector3.one * 20, new Vector3(1, 0.8f, 1), EaseType.easeOutQuint)
				.ScaleTo(effectBackObject, new Vector3(600, 20, 0), new Vector3(600, 1, 0), EaseType.easeOutQuint)
				.RotateTo(effectBackObject, new Vector3(60, 60, 0), new Vector3(0, 0, 0), EaseType.linear)
				.RotateTo(timeupTextObject, new Vector3(60, -40, 40), new Vector3(0, 0, 0), EaseType.linear),

			new Tween(0.5f).ScaleTo(timeupTextObject, new Vector3(0.95f, 0.76f, 0.5f), EaseType.linear),

			new Tween(1)
				.ScaleTo(effectBackObject, new Vector3(600, 0, 0), EaseType.easeOutExpo)
				.ScaleTo(timeupTextObject, Vector3.one * 9, EaseType.easeOutExpo)
				.FadeTo(textSpriteRenderer, 0, EaseType.easeOutExpo)
				.FadeTo(maskSpriteRenderer, 1, EaseType.linear)
				.Complete(() => callback())
		);
	}
}
