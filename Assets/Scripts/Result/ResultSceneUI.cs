using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ResultSceneUI : MonoBehaviour {
	public FadeManager fadeManager;
	public Text scoreText;
	public Text bestText;

	void Retry(float waitTime, Action action) { StartCoroutine(StartRetry(waitTime, action)); }
	IEnumerator StartRetry(float waitTime, Action action) {
		yield return new WaitForSeconds(waitTime);
		action();
	}
	
	void DisplayScore(string score) {
	}
	
	void Awake() {
		var score = Storage.Get("Score") ?? "0";
		ScoreRegistIfNewRecord(score);

		var localData = LocalData.Read();
		if (int.Parse(localData.bestScore ?? "0") < int.Parse(score)) {
			localData.bestScore = score;
			LocalData.Write(localData);
		}
		bestText.text = "Best: " + (localData.bestScore ?? "0");
		scoreText.text = "Score: " + score;

		fadeManager.FadeIn(0.4f, EaseType.easeInQuad);
	}

	void ScoreRegistIfNewRecord(string score) {
		var localData = LocalData.Read();
		API.ScoreRegistIfNewRecord(
			localData.playerInfo.id,
			Storage.Get("Chain") + "-" + Storage.Get("BackNum"),
			score.ToString(),
			checkRecord => {
			Debug.Log(checkRecord.is_new_record);
		}, www => Retry(3f, () => ScoreRegistIfNewRecord(score)));
	}

	public void OnClickReturnButton() {
		fadeManager.FadeOut(0.4f, EaseType.easeInQuad, () => Application.LoadLevel ("Title"));
	}
}
