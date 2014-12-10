using UnityEngine;
using System;
using System.Collections;

public class TimeKeeper : MonoBehaviour {
	public event Action TimedOut;
	public float timeLimit;

	float remainingTime;
	bool isPlaying = false;

	void Update() {
		if (!isPlaying)
			return;

		remainingTime -= Time.deltaTime;

		if (remainingTime < 0) {
			isPlaying = false;
			TimedOut();
		}
	}

	public void StartCountdown() {
		isPlaying = true;
		remainingTime = timeLimit;
	}

	public float GetRemainingTime() {
		return remainingTime;
	}
}
