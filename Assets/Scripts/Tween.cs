using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EaseDelegate = System.Func<float, float, System.Func<float, float, float, float>, float>;

public class Tween : MonoBehaviour {
	Queue<Func<bool>> UpdateActionQueue = new Queue<Func<bool>>();
	
	// Update is called once per frame
	void Update () {
		if (UpdateActionQueue.Count > 0)
			if (!UpdateActionQueue.Peek()())
				UpdateActionQueue.Dequeue();
	}

	public void SeriesExecute(params Func<bool>[] updateActions) {
		foreach (var action in updateActions) {
			UpdateActionQueue.Enqueue(action);
		}
	}
	
	public Func<bool> Animate(float endTime, Action<EaseDelegate> onUpdate, Action onComplete = null) {
		var currentTime = 0f;
		
		return () => {
			if (currentTime < endTime) {
				onUpdate((from, to, ease) => ease(from, to, currentTime / endTime));
				currentTime += Time.deltaTime;
				return true;
			}
			
			onUpdate((from, to, ease) => ease(from, to, 1));
			if (onComplete != null)
				onComplete();
			
			return false;
		};
	}

	public Func<bool> Wait(float time) {
		return Animate(time, tween => {});
	}

	public void Stop() {
		UpdateActionQueue.Clear();
	}
}
