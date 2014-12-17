using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class Tween {
	List<Action<float>> UpdateActions = new List<Action<float>>();
	float animateTime;
	Action callback;

	public Func<float, bool> GetUpdateAction() {
		var runningTime = 0f;

		Action<float> updateAll = percentage => {
			foreach (var updateAction in UpdateActions)
				updateAction(percentage);
		};
		
		return deltaTime => {
			if (runningTime < animateTime) {
				updateAll(runningTime / animateTime);
				runningTime += deltaTime;
				return true;
			}
			
			updateAll(1);
			if (callback != null)
				callback();
			
			return false;
		};
	}
	
	public Tween(float time) {
		this.animateTime = time;
	}

	public Tween Complete(Action callback) {
		this.callback = callback;
		return this;
	}

	public Tween Wait() {
		UpdateActions.Add(per => {});
		return this;
	}
	
	public Tween ScaleTo(GameObject target, Vector3 to, Func<float, float, float, float> ease) {
		Vector3 from = Vector3.zero;
		bool init = false;
		UpdateActions.Add(percentage => {
			if (!init) {
				from = target.transform.localScale;
				init = true;
			}
			target.transform.localScale = from - (from - to) * ease(0, 1, percentage);
		});
		return this;
	}
	
	public Tween ScaleTo(GameObject target, Vector3 from, Vector3 to, Func<float, float, float, float> ease) {
		UpdateActions.Add(percentage => {
			target.transform.localScale = from - (from - to) * ease(0, 1, percentage);
		});
		return this;
	}
	
	public Tween MoveTo(GameObject target, Vector3 to, Func<float, float, float, float> ease) {
		Vector3 from = Vector3.zero;
		bool init = false;
		UpdateActions.Add(percentage => {
			if (!init) {
				from = target.transform.position;
				init = true;
			}
			target.transform.position = from - (from - to) * ease(0, 1, percentage);
		});
		return this;
	}
	
	public Tween MoveTo(GameObject target, Vector3 from, Vector3 to, Func<float, float, float, float> ease) {
		UpdateActions.Add(percentage => {
			target.transform.position = from - (from - to) * ease(0, 1, percentage);
		});
		return this;
	}
	
	public Tween ColorTo(SpriteRenderer target, Color from, Color to, Func<float, float, float, float> ease) {
		UpdateActions.Add(percentage => {
			target.color = from - (from - to) * ease(0, 1, percentage);
		});
		return this;
	}
	
	public Tween ColorTo(SpriteRenderer target, Color to, Func<float, float, float, float> ease) {
		Color from = Color.white;
		bool init = false;
		UpdateActions.Add(percentage => {
			if (!init) {
				from = target.color;
				init = true;
			}
			target.color = from - (from - to) * ease(0, 1, percentage);
		});
		return this;
	}
	
	public Tween FadeTo(SpriteRenderer target, float to, Func<float, float, float, float> ease) {
		Color color = Color.white;
		float from = 0;
		bool init = false;
		UpdateActions.Add(percentage => {
			if (!init) {
				color = target.color;
				from = color.a;
				init = true;
			}
			color.a = from - (from - to) * ease(0, 1, percentage);
			target.color = color;
		});
		return this;
	}
	
	public Tween FadeTo(SpriteRenderer target, float from, float to, Func<float, float, float, float> ease) {
		Color color = Color.white;
		bool init = false;
		UpdateActions.Add(percentage => {
			if (!init) {
				color = target.color;
				init = true;
			}
			color.a = from - (from - to) * ease(0, 1, percentage);
			target.color = color;
		});
		return this;
	}

	public Tween RotateTo(GameObject target, Vector3 to, Func<float, float, float, float> ease) {
		var from = Vector3.zero;
		bool init = false;
		UpdateActions.Add(percentage => {
			if (!init) {
				var startVal = target.transform.localRotation;
				from = new Vector3(startVal.x, startVal.y, startVal.z);
				init = true;
			}
			var euler = from - (from - to) * ease(0, 1, percentage);
			target.transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
		});
		return this;
	}

	public Tween RotateTo(GameObject target, Vector3 from, Vector3 to, Func<float, float, float, float> ease) {
		UpdateActions.Add(percentage => {
			var euler = from - (from - to) * ease(0, 1, percentage);
			target.transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
		});
		return this;
	}

	public Tween ValueTo(Vector3 from, Vector3 to, Func<float, float, float, float> ease, Action<Vector3> onUpdate) {
		UpdateActions.Add(percentage => onUpdate(from - (from - to) * ease(0, 1, percentage)));
		return this;
	}
}
