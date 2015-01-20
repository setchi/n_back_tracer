using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
		effectBackObject.SetActive(false);
		timeupTextObject.SetActive(false);
		readyTextObject.SetActive(false);
		goTextObject.SetActive(false);
	}

	public void CancelAllAnimate() {
		TweenPlayer.CancelAll(gameObject);
		HideAll();
	}

	public void EmitReadyAnimation() {
		Show (readyTextObject);

		TweenPlayer.Play(gameObject,
			new Tween(0.65f)
				.ScaleTo(readyTextObject, new Vector3(1.3f, 1.3f * 0.8f, 1.3f), EaseType.easeOutCirc),

			new Tween(0.65f)
				.ScaleTo(readyTextObject, new Vector3(1.3f, 1.3f * 0.8f, 1.3f), new Vector3(1, 0.8f, 1), EaseType.easeInCirc)
				.Complete(EmitReadyAnimation)
		);
	}

	public void EmitGoAnimation() {
		Show(goTextObject);
		var goText = goTextObject.GetComponent<Text>();

		TweenPlayer.CancelAll(gameObject);
		TweenPlayer.Play(gameObject,
			new Tween(0.7f)
				.ScaleTo(goTextObject, new Vector3(1, 0.8f, 1), EaseType.easeOutCirc)
				.ValueTo(Vector3.one, Vector3.zero, EaseType.linear, pos => goText.color = new Color(1, 1, 1, pos.x))
				.Complete(() => Hide(goTextObject))
		);
	}

	public void EmitTimeupAnimation(Action onComplete) {
		Show (effectBackObject, timeupTextObject);
		var textSpriteRenderer = timeupTextObject.GetComponent<SpriteRenderer>();
		var timeupText = timeupTextObject.GetComponent<Text>();

		TweenPlayer.Play(gameObject,
			new Tween(0.4f)
				.ScaleTo(timeupTextObject, Vector3.one * 20, new Vector3(1, 0.8f, 1) * 0.9f, EaseType.easeOutQuint)
				.ScaleTo(effectBackObject, new Vector3(600, 20, 0), new Vector3(600, 1, 0), EaseType.easeOutQuint)
				.RotateTo(effectBackObject, new Vector3(60, 60, 0), new Vector3(0, 0, 0), EaseType.linear)
				.RotateTo(timeupTextObject, new Vector3(60, -40, 40), new Vector3(0, 0, 0), EaseType.linear),

		    new Tween(0.5f).ScaleTo(timeupTextObject, new Vector3(0.95f, 0.76f, 0.5f) * 0.9f, EaseType.linear).Complete(() => {
				fadeManager.FadeOut(1, EaseType.linear);
			}),

			new Tween(1)
				.ScaleTo(effectBackObject, new Vector3(600, 0, 0), EaseType.easeOutExpo)
				.ScaleTo(timeupTextObject, Vector3.one *11, EaseType.easeOutExpo)
				.FadeTo(textSpriteRenderer, 0, EaseType.easeOutExpo)
				.ValueTo(Vector3.one, Vector3.zero, EaseType.easeOutExpo, pos => timeupText.color = new Color(1, 1, 1, pos.x))
				.Complete(onComplete)
		);
	}
}
