using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ResultSceneUI : MonoBehaviour {
	public FadeManager fadeManager;
	public Text scoreText;

	void Retry(float waitTime, Action action) { StartCoroutine(StartRetry(waitTime, action)); }
	IEnumerator StartRetry(float waitTime, Action action) {
		yield return new WaitForSeconds(waitTime);
		action();
	}
	
	void DisplayScore(string score) {
		scoreText.text = "Score: " + score;
	}
	
	void Awake() {
		var score = Storage.Get("Score") ?? "1";
		fadeManager.FadeIn(0.4f, EaseType.easeInQuad);
		DisplayScore(score);
		ScoreRegistIfNewRecord(score);
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
		fadeManager.FadeOut(0.4f, EaseType.easeInQuad, () => {
			Destroy (GameObject.Find ("StorageObject"));
			Application.LoadLevel ("Title");
		});
	}
}
