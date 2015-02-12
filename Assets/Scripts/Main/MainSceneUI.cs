using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MainSceneUI : MonoBehaviour {
	public FadeManager fadeManager;

	public void OnClickReturnButton() {
		fadeManager.FadeOut(0.3f, Ease.InQuart, () => Application.LoadLevel ("Title"));
	}
}
