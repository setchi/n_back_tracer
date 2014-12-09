using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeKeeper : MonoBehaviour {
	public float timeLimit;
	
	GameController gameController;

	float remainingTime;
	bool isPlaying = false;
	
	void Awake() {
		gameController = GetComponent<GameController>();
	}

	void Update() {
		if (!isPlaying)
			return;

		remainingTime -= Time.deltaTime;

		if (remainingTime < 0) {
			gameController.TimeOver();
			isPlaying = false;
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
