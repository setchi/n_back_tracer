using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TimeKeeper : MonoBehaviour {
	public event Action TimeUp;
	public float timeLimit;
	public Slider slider;

	float remainingTime;
	bool isPlaying = false;

	void Update() {
		if (!isPlaying)
			return;

		remainingTime -= Time.deltaTime;

		if (remainingTime < 0) {
			isPlaying = false;
			TimeUp();
		}
		slider.value = remainingTime / timeLimit;
	}

	public void StartCountdown() {
		isPlaying = true;
		remainingTime = timeLimit;
	}

	public float GetRemainingTime() {
		return remainingTime;
	}
}
