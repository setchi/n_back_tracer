using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FadeManager : MonoBehaviour {
	public GameObject fadeMask;

	Color maskColor_ = Color.black;
	public Color MaskColor {
		set { maskColor_ = value; }
	}

	Image fadeMaskImage_;
	Image fadeMaskImage {
		get { return fadeMaskImage_ ?? (fadeMaskImage_ = fadeMask.GetComponent<Image>()); }
	}

	public void FadeIn(float time, Func<float, float, float, float> ease, Action onComplete = null) {
		SetAlpha(1);
		fadeMask.SetActive(true);

		TweenPlayer.Play(fadeMask, new Tween(time).ValueTo(Vector3.one, Vector3.zero, ease, pos => SetAlpha(pos.x)).Complete(() => {
			fadeMask.SetActive(false);

			if (onComplete != null)
				onComplete();
		}));
	}

	public void FadeOut(float time, Func<float, float, float, float> ease, Action onComplete = null) {
		SetAlpha(0);
		fadeMask.SetActive(true);

		TweenPlayer.Play(gameObject, new Tween(time).ValueTo(Vector3.zero, Vector3.one, ease, pos => SetAlpha(pos.x)).Complete(onComplete));
	}

	void SetAlpha(float alpha) {
		maskColor_.a = alpha;
		fadeMaskImage.color = maskColor_;
	}
}
