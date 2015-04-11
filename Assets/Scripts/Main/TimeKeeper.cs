using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;

public class TimeKeeper : MonoBehaviour {
	public event Action TimeUp;
	[SerializeField] float timeLimit;
	[SerializeField] Slider slider;

	public void StartCountdown() {
		Observable.EveryUpdate()
			.TakeUntil(Observable.Timer(TimeSpan.FromSeconds(timeLimit)))
				.Select(_ => Time.deltaTime)
				.Scan((total, delta) => total + delta)
				.Subscribe(elapsedTime => slider.value = (timeLimit - elapsedTime) / timeLimit, TimeUp);
	}
}
