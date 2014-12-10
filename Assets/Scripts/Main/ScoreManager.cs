using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {
	float score = 0;

	public void CorrectPattern() {
		score += 10; // てきとう
	}

	public void CorrectTouch() {
		score += 1; // てきとう
	}

	public void MissTouch() {
		// miss
		score *= 0.95f; // てきとう
	}

	public int GetScore() {
		return Mathf.RoundToInt(score);
	}
}
