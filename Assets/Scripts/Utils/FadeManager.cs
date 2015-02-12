using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

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

	public void FadeIn(float time, Ease ease, Action onComplete = null) {
		fadeMaskImage.color = maskColor_;
		fadeMask.SetActive(true);
		fadeMaskImage.DOFade(0, time).SetEase(ease).OnComplete(() => {
			fadeMask.SetActive(false);
			
			if (onComplete != null)
				onComplete();
		});
	}

	public void FadeOut(float time, Ease ease, Action onComplete = null) {
		var color = maskColor_;
		color.a = 0;
		fadeMaskImage.color = color;
		fadeMask.SetActive(true);
		fadeMaskImage.DOFade(1, time).SetEase(ease).OnComplete(() => { if (onComplete != null) onComplete(); });
	}

	void SetAlpha(float alpha) {
		maskColor_.a = alpha;
		fadeMaskImage.color = maskColor_;
	}
}
