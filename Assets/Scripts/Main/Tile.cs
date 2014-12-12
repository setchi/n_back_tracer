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
		lineRenderer.SetWidth (0.1f, 0.1f);
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
		SetTimer (1f, position => {
			UpdateColor (Color.white * 0.07f + Color.green * 0.93f, defaultColor, position);
			UpdateScale ((1 - position) * 0.3f + 1);
		}, EraseLine);
	}

	public void EmitCorrectTouchEffect() {
		UpdateColor ((Color.white + Color.cyan * 2) / 2.5f, defaultColor, 0);

		SetTimer (0.4f, position => {
			UpdateScale ((1 - position) * 0.3f + 1);
		});
	}

	public void EmitPatternCorrectEffect() {
		SetTimer (0.4f, position => {
			UpdateColor (Color.white, defaultColor, position);
			// UpdateScale ((1 - position) * 0.3f + 1);
		}, EraseLine);
	}

	public void EmitMissEffect() {
		SetTimer (0.6f, position => {
			UpdateColor ((Color.white + Color.red * 2) / 2.5f, defaultColor, position);
			UpdateScale ((1 - position) * 0.3f + 1);
		}, EraseLine);
	}

	public void EmitHintEffect() {
		SetTimer (0.6f, position => {
			UpdateColor ((Color.white + Color.cyan * 2) / 2.5f, defaultColor, position);
		}, EraseLine);
	}

	void UpdateColor(Color from, Color to, float position) {
		var color = from - (from - to) * position;
		spriteRenderer.color = color;
		lineRenderer.material.color = color;
	}
	
	void UpdateScale(float scale) {
		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
