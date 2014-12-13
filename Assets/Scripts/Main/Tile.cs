using UnityEngine;
using System;
using System.Collections;

public class Tile : MonoBehaviour {
	public int TileId {
		set { tileId = value; }
	}
	int tileId;

	SpriteRenderer spriteRenderer;
	GameController gameController;
	LineRenderer lineRenderer;
	Action UpdateTime;
	
	Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 1);

	void Awake() {
		gameController = GameObject.Find ("Tiles").GetComponent<GameController>();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		lineRenderer = GetComponentInChildren<LineRenderer>();
		lineRenderer.SetWidth (0.15f, 0.15f);
		// 線のスタート位置は常にタイルの中心
		lineRenderer.SetPosition(0, Vector3.zero);
	}

	void Update() {
		if (UpdateTime != null)
			UpdateTime ();
	}

	public void DrawLine(Vector3 endPosition) {
		lineRenderer.SetPosition(1, endPosition);
	}
	
	void EraseLine() { DrawLine (Vector3.zero); }

	void OnMouseEnter() {
		gameController.TouchedTile (tileId);
	}

	void SetTimer(float endTime, Action<float> onUpdate, Action onComplete = null) {
		var currentTime = 0f;

		UpdateTime = () => {
			if (currentTime < endTime) {
				onUpdate(currentTime / endTime);
				currentTime += Time.deltaTime;
			
			} else {
				UpdateTime = null;
				onUpdate(1);

				if (onComplete != null)
					onComplete();
			}
		};
	}

	public void EmitMarkEffect() {
		var currentScale = transform.localScale.x;

		SetTimer (1f, position => {
			/*
			var threshold = 0.35f;
			if (position < threshold) {
				UpdateColor (Color.white , Color.green, position * (1 / threshold));
			} else {
				UpdateColor (Color.green, defaultColor, (position - threshold) / (1 - threshold));
			}*/
			UpdateScale (currentScale, 1, position);
			UpdateColor(Color.green, defaultColor, position);
		}, EraseLine);
	}

	public void EmitCorrectTouchEffect() {
		// UpdateColor (Color.white, Color.cyan, 1);

		SetTimer (0.4f, position => {
			UpdateScale (1.3f, 1, position);
			UpdateColor (Color.white, Color.cyan, position);
		});
	}

	public void EmitPatternCorrectEffect() {
		var currentScale = transform.localScale.x;

		SetTimer (0.4f, position => {
			UpdateColor (Color.cyan, defaultColor, position);
			UpdateScale (currentScale, 1, position);
		}, EraseLine);
	}

	public void EmitMissEffect() {
		SetTimer (0.6f, position => {
			UpdateColor ((Color.white + Color.red * 2) / 2.5f, defaultColor, position);
			UpdateScale (1.3f, 1, position);
		}, EraseLine);
	}

	public void EmitHintEffect() {
		SetTimer (0.6f, position => {
			UpdateColor (Color.cyan, defaultColor, position);
		}, EraseLine);
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
