using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScreenEffectManager : MonoBehaviour {
	List<Func<int, bool>> AnimateActionList;
	GameObject effectBackObject;
	GameObject timeupTextObject;
	GameObject readyTextObject;
	GameObject goTextObject;
	GameObject effectObject;
	SpriteRenderer maskSpriteRenderer;

	// Use this for initialization
	void Awake () {
		effectBackObject = GameObject.Find("EffectBack");
		timeupTextObject = GameObject.Find("TimeupText");
		readyTextObject = GameObject.Find("ReadyText");
		goTextObject = GameObject.Find("GoText");
		maskSpriteRenderer = GameObject.Find("Mask").GetComponent<SpriteRenderer>();
		effectObject = GameObject.Find("Effect");
		AnimateActionList = new List<Func<int, bool>>();

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
		if (AnimateActionList.Count > 0)
			if (!AnimateActionList[0](0))
				AnimateActionList.RemoveAt(0);
	}

	Func<int, bool> SetAnimation(float endTime, Action<float> onUpdate, Action onComplete = null) {
		var currentTime = 0f;

		return i => {
			if (currentTime < endTime) {
				onUpdate(currentTime / endTime);
				currentTime += Time.deltaTime;
				return true;
			}
			onUpdate(1);
			if (onComplete != null)
				onComplete();
			
			return false;
		};
	}

	public void AnimationStop() {
		AnimateActionList.RemoveAll(i => true);
		HidingAll();
	}

	public void EmitReadyAnimation() {
		Show (readyTextObject);
		
		AnimateActionList.Add(SetAnimation(0.7f, pos => {
			var scale = Easing.easeOutCirc(1, 1.3f, pos);
			readyTextObject.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
		}));
		
		AnimateActionList.Add(SetAnimation(0.7f, pos => {
			var scale = Easing.easeInCirc(1.3f, 1, pos);
			readyTextObject.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
		}, () => {
			EmitReadyAnimation();
		}));
	}

	public void EmitGoAnimation() {
		Show(goTextObject);

		var spriteRenderer = goTextObject.GetComponent<SpriteRenderer>();
		var startScale = goTextObject.transform.localScale.x;
		var startColor = spriteRenderer.color;

		AnimateActionList.Add(SetAnimation(0.7f, pos => {
			var scale = Easing.easeOutCirc(startScale * 0.8f, startScale, pos);
			var color = startColor;
			color.a *= Easing.linear(1, 0, pos);

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

		AnimateActionList.Add(SetAnimation(0.4f, pos => {
			var backColor = backSpriteRenderer.color;
			var backScale = effectBackObject.transform.localScale;
			var backRot = effectBackObject.transform.localRotation;

			backColor.a = Easing.easeOutQuint(0, 1, pos);
			var rot = Easing.linear(60, 0, pos);
			var scale = Easing.easeOutQuint(20f, 1f, pos);
			
			backSpriteRenderer.color = backColor;
			timeupTextObject.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
			effectBackObject.transform.localScale = new Vector3(600, scale, 0);
			effectBackObject.transform.localRotation = Quaternion.Euler(rot, rot, 0);
			timeupTextObject.transform.localRotation = Quaternion.Euler(rot, -rot / 1.5f, rot / 1.5f);
		}));

		AnimateActionList.Add(SetAnimation(0.5f, pos => {}));

		AnimateActionList.Add(SetAnimation(1f, pos => {
			var textColor = textSpriteRenderer.color;
			var maskColor = maskSpriteRenderer.color;
			var backScale = effectBackObject.transform.localScale;
			var textScale = timeupTextObject.transform.localScale;

			textColor.a = Easing.easeOutExpo(1, 0, pos);
			maskColor.a = Easing.linear(0, 1, pos);
			backScale.y = textColor.a;
			textScale.x = Easing.easeOutExpo(1, 9, pos);

			textSpriteRenderer.color = textColor;
			effectBackObject.transform.localScale = backScale;
			timeupTextObject.transform.localScale = textScale;
			maskSpriteRenderer.color = maskColor;
		}, () => {
			callback();
		}));
	}
}
