using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScreenEffectManager : MonoBehaviour {
	List<Func<int, bool>> AnimationList;
	GameObject effectBackObject;
	GameObject finishTextObject;
	GameObject effectObject;
	SpriteRenderer maskSpriteRenderer;

	// Use this for initialization
	void Awake () {
		effectBackObject = GameObject.Find("EffectBack");
		finishTextObject = GameObject.Find("FinishText");
		maskSpriteRenderer = GameObject.Find("Mask").GetComponent<SpriteRenderer>();
		effectObject = GameObject.Find("Effect");
		effectObject.SetActive(false);
		AnimationList = new List<Func<int, bool>>();
	}
	
	// Update is called once per frame
	void Update () {
		if (AnimationList.Count > 0)
			if (!AnimationList[0](0))
				AnimationList.RemoveAt(0);
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

	public void StartFinishAnimation(Action callback) {
		effectObject.SetActive(true);

		var backSpriteRenderer = effectBackObject.GetComponent<SpriteRenderer>();
		var textSpriteRenderer = finishTextObject.GetComponent<SpriteRenderer>();

		AnimationList.Add(SetAnimation(0.4f, pos => {
			var backColor = backSpriteRenderer.color;
			var backScale = effectBackObject.transform.localScale;
			var backRot = effectBackObject.transform.localRotation;

			backColor.a = Easing.easeOutQuint(0, 1, pos);
			var rot = Easing.linear(60, 0, pos);
			var scale = Easing.easeOutQuint(20f, 1f, pos);
			
			backSpriteRenderer.color = backColor;
			finishTextObject.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
			effectBackObject.transform.localScale = new Vector3(600, scale, 0);
			effectBackObject.transform.localRotation = Quaternion.Euler(rot, rot, 0);
			finishTextObject.transform.localRotation = Quaternion.Euler(rot, -rot / 1.3f, rot / 1.3f);
		}));

		AnimationList.Add(SetAnimation(0.5f, pos => {}));

		AnimationList.Add(SetAnimation(1f, pos => {
			var backColor = backSpriteRenderer.color;
			var textColor = textSpriteRenderer.color;
			var maskColor = maskSpriteRenderer.color;
			var backScale = effectBackObject.transform.localScale;
			var textScale = finishTextObject.transform.localScale;

			textColor.a = Easing.easeOutExpo(1, 0, pos);
			maskColor.a = Easing.linear(0, 1, pos);
			backScale.y = textColor.a;
			textScale.x = Easing.easeOutExpo(1, 9, pos);
			
			backSpriteRenderer.color = backColor;
			textSpriteRenderer.color = textColor;
			effectBackObject.transform.localScale = backScale;
			finishTextObject.transform.localScale = textScale;
			maskSpriteRenderer.color = maskColor;
		}, () => {
			callback();
		}));
	}
}
