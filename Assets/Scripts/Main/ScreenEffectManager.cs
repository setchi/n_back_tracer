using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using EaseDelegate = System.Func<float, float, System.Func<float, float, float, float>, float>;

public class ScreenEffectManager : MonoBehaviour {
	GameObject effectBackObject;
	GameObject timeupTextObject;
	GameObject readyTextObject;
	GameObject goTextObject;
	GameObject effectObject;
	SpriteRenderer maskSpriteRenderer;
	Tween tween;

	// Use this for initialization
	void Awake () {
		effectBackObject = GameObject.Find("EffectBack");
		timeupTextObject = GameObject.Find("TimeupText");
		readyTextObject = GameObject.Find("ReadyText");
		goTextObject = GameObject.Find("GoText");
		effectObject = GameObject.Find("Effect");
		maskSpriteRenderer = GameObject.Find("Mask").GetComponent<SpriteRenderer>();
		tween = GetComponent<Tween>();

		HidingAll();
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
	
	void HidingAll() {
		effectBackObject.SetActive(false);
		timeupTextObject.SetActive(false);
		readyTextObject.SetActive(false);
		goTextObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void AnimationStop() {
		tween.Stop();
		HidingAll();
	}

	public void EmitReadyAnimation() {
		Show (readyTextObject);

		tween.SeriesExecute(
			tween.Animate(0.65f, easeDelegate => {
				var scale = easeDelegate(1, 1.3f, EaseType.easeOutCirc);
				readyTextObject.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
			}),

			tween.Animate(0.65f, easeDelegate => {
				var scale = easeDelegate(1.3f, 1, EaseType.easeInCirc);
				readyTextObject.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
			}, () => {
				EmitReadyAnimation();
			})
		);
	}

	public void EmitGoAnimation() {
		Show(goTextObject);

		var spriteRenderer = goTextObject.GetComponent<SpriteRenderer>();
		var startScale = goTextObject.transform.localScale.x;
		var startColor = spriteRenderer.color;

		tween.SeriesExecute(tween.Animate(0.7f, easeDelegate => {
			var scale = easeDelegate(startScale * 0.8f, startScale, EaseType.easeOutCirc);
			var color = startColor;
			color.a = easeDelegate(1, 0, EaseType.linear);
			
			spriteRenderer.color = color;
			goTextObject.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
		}, () => {
			Hide(goTextObject);
		}));
	}

	public void EmitTimeupAnimation(Action callback) {
		Show (effectBackObject, timeupTextObject);

		var backSpriteRenderer = effectBackObject.GetComponent<SpriteRenderer>();
		var textSpriteRenderer = timeupTextObject.GetComponent<SpriteRenderer>();

		tween.SeriesExecute(
			tween.Animate(0.4f, easeDelegate => {
				var backColor = backSpriteRenderer.color;
				var backScale = effectBackObject.transform.localScale;
				var backRot = effectBackObject.transform.localRotation;
				
				backColor.a = easeDelegate(0, 1, EaseType.easeOutQuint);
				var rot = easeDelegate(60, 0, EaseType.linear);
				var scale = easeDelegate(20, 1, EaseType.easeOutQuint);
				
				backSpriteRenderer.color = backColor;
				timeupTextObject.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
				effectBackObject.transform.localScale = new Vector3(600, scale, 0);
				effectBackObject.transform.localRotation = Quaternion.Euler(rot, rot, 0);
				timeupTextObject.transform.localRotation = Quaternion.Euler(rot, -rot / 1.5f, rot / 1.5f);

			}),

			tween.Wait(0.5f),

			tween.Animate(1f, easeDelegate => {
				var textColor = textSpriteRenderer.color;
				var maskColor = maskSpriteRenderer.color;
				var backScale = effectBackObject.transform.localScale;
				var textScale = timeupTextObject.transform.localScale;
				
				textColor.a = easeDelegate(1, 0, EaseType.easeOutExpo);
				maskColor.a = easeDelegate(0, 1, EaseType.linear);
				backScale.y = textColor.a;
				textScale.x = easeDelegate(1, 9, EaseType.easeOutExpo);
				
				textSpriteRenderer.color = textColor;
				effectBackObject.transform.localScale = backScale;
				timeupTextObject.transform.localScale = textScale;
				maskSpriteRenderer.color = maskColor;
			}, () => {
				callback();
			})
		);
	}
}
