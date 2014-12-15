using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	public int TileId {
		set { tileId = value; }
	}
	int tileId;

	SpriteRenderer spriteRenderer;
	GameController gameController;
	LineRenderer lineRenderer;
	List<Func<int, bool>> UpdateActions;
	
	Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 1);

	void Awake() {
		gameController = GameObject.Find ("Tiles").GetComponent<GameController>();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		lineRenderer = GetComponentInChildren<LineRenderer>();
		lineRenderer.SetWidth (0.13f, 0.13f);
		// 線のスタート位置は常にタイルの中心
		lineRenderer.SetPosition(0, Vector3.zero);

		UpdateActions = new List<Func<int, bool>>();
	}

	void Update() {
		UpdateActions = UpdateActions.Where(action => action(0)).ToList();
	}

	public void DrawLine(Vector3 endPosition) {
		lineRenderer.SetPosition(1, endPosition);
	}
	
	void EraseLine() { DrawLine (Vector3.zero); }

	void CompleteEffect() {
		if (UpdateActions.Count == 1)
			EraseLine();
	}

	void OnMouseEnter() {
		gameController.TouchedTile (tileId);
	}

	void SetTimer(bool important, float endTime, Func<float, float, float, float> easing, Action<float> onUpdate, Action onComplete = null) {
		var currentTime = 0f;
		Func<int, bool> action = i => {
			if (currentTime < endTime) {
				onUpdate(easing(0, 1, currentTime / endTime));
				currentTime += Time.deltaTime;
				return true;
			}
			onUpdate(1);
			
			if (onComplete != null)
			onComplete();
			
			return false;
		};

		if (important) UpdateActions = new List<Func<int, bool>> () {action};
		else UpdateActions.Add(action);
	}

	public void EmitMarkEffect() {
		var currentScale = transform.localScale.x;

		SetTimer (true, 1f, Easing.linear, position => {
			//*
			var threshold = 0.35f;
			if (position < threshold) {
				UpdateColor (Color.white , Color.green, position * (1 / threshold));
			} else {
				UpdateColor (Color.green, defaultColor, (position - threshold) / (1 - threshold));
			}//*/
			UpdateScale (currentScale, 1, position);
			// UpdateColor(Color.green, defaultColor, position);
		}, CompleteEffect);
	}

	public void EmitCorrectTouchEffect() {
		// UpdateColor (Color.white, Color.cyan, 1);

		SetTimer (true, 0.4f, Easing.easeOutBounce, position => {
			UpdateScale (1.3f, 1, position);
			UpdateColor (Color.white, Color.cyan, position);
		});
	}

	public void EmitPatternCorrectEffect() {
		SetTimer (false, 0.4f, Easing.linear, position => {
			UpdateColor (Color.cyan, defaultColor, position);
		}, CompleteEffect);
	}

	public void EmitMissEffect() {
		EraseLine();

		SetTimer (true, 0.6f, Easing.linear, position => {
			UpdateColor ((Color.white + Color.red * 2) / 2.5f, defaultColor, position);
			UpdateScale (1.3f, 1, position);
		}, CompleteEffect);
	}

	public void EmitHintEffect() {
		SetTimer (true, 0.6f, Easing.linear, position => {
			UpdateColor (Color.cyan, defaultColor, position);
		}, CompleteEffect);
	}

	void UpdateColor(Color from, Color to, float position) {
		var color = from - (from - to) * position;
		spriteRenderer.color = color;
		lineRenderer.material.color = color;
	}
	
	void UpdateScale(float from, float to, float position) {
		var scale = from - (from - to) * position;
		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
