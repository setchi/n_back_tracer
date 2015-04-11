using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {
	float score = 0;
	int chain;
	int backNum;

	void Awake() {
		chain = int.Parse(Storage.Get("Chain") ?? "4");
		backNum = int.Parse(Storage.Get("BackNum") ?? "2");
	}

	public void CorrectPattern() {
		score += chain * Mathf.Pow(backNum, 1.5f);
	}

	public void CorrectTouch() {
		score += backNum;
	}

	public void IncorrectTouch() {
		// miss
		score *= 0.99f;
	}

	public int GetScore() {
		return Mathf.RoundToInt(score);
	}
}
