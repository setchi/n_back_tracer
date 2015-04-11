using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;

public class ResultSceneUI : MonoBehaviour {
	[SerializeField] FadeManager fadeManager;
	[SerializeField] Text scoreText;
	[SerializeField] Text bestText;

	void Awake() {
		var score = Storage.Get("Score") ?? "0";
		ScoreRegistIfNewRecord(score);

		if (int.Parse(LocalData.BestScore) < int.Parse(score)) {
			LocalData.BestScore = score;
		}
		bestText.text = "Best: " + LocalData.BestScore;
		scoreText.text = "Score: " + score;

		fadeManager.FadeIn(0.4f, DG.Tweening.Ease.InQuad);
	}

	void ScoreRegistIfNewRecord(string score) {
		API.ScoreRegistIfNewRecord(
			Storage.Get("Chain") + "-" + Storage.Get("BackNum"),
			score
			).Subscribe(_ => {}, ex => Observable.Timer(TimeSpan.FromSeconds(3))
			.Subscribe(_ => ScoreRegistIfNewRecord(score)));
	}

	public void OnClickReturnButton() {
		fadeManager.FadeOut(0.4f, DG.Tweening.Ease.InQuad, () => Application.LoadLevel ("Title"));
	}
}
