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
	TweenExecutor tweenExecutor;

	Vector3 defaultColor = Vector3.one * 0.2f;

	void Awake() {
		gameController = GameObject.Find ("Tiles").GetComponent<GameController>();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		lineRenderer = GetComponentInChildren<LineRenderer>();
		lineRenderer.SetWidth (0.13f, 0.13f);
		// 線のスタート位置は常にタイルの中心
		lineRenderer.SetPosition(0, Vector3.zero);

		tweenExecutor = new TweenExecutor();
	}

	void Update() {
		tweenExecutor.Update();
	}

	public void DrawLine(Vector3 endPosition) {
		lineRenderer.SetPosition(1, endPosition);
	}
	
	void EraseLine() { DrawLine (Vector3.zero); }

	void CompleteEffect() {
		EraseLine();
	}

	void OnMouseEnter() {
		gameController.TouchedTile (tileId);
	}

	public void EmitMarkEffect() {
		tweenExecutor.CancelAll().SeriesExecute(new Tween(1f).ScaleTo(gameObject, Vector3.one, EaseType.linear));
		tweenExecutor.SeriesExecute(
			new Tween(1f)
				.ValueTo(new Vector3(0, 1, 0), defaultColor, EaseType.linear, value => UpdateColor(new Color(value.x, value.y, value.z)))
				.Complete(CompleteEffect)
		);
	}

	public void EmitCorrectTouchEffect() {
		tweenExecutor.CancelAll().SeriesExecute(
			new Tween(0.4f)
				.ScaleTo(gameObject, Vector3.one * 1.3f, Vector3.one, EaseType.easeOutBounce)
				.ValueTo(Vector3.one, new Vector3(0, 1, 1), EaseType.easeOutBounce, value => UpdateColor(new Color(value.x, value.y, value.z)))
		);
	}

	public void EmitPatternCorrectEffect() {
		tweenExecutor.SeriesExecute(
			new Tween(0.4f)
				.ValueTo(new Vector3(0, 1, 1), defaultColor, EaseType.linear, value => UpdateColor(new Color(value.x, value.y, value.z)))
				.Complete(CompleteEffect)
		);
	}

	public void EmitMissEffect() {
		EraseLine();
		tweenExecutor.CancelAll().SeriesExecute(
			new Tween(0.6f)
				.ScaleTo(gameObject, Vector3.one * 1.3f, Vector3.one, EaseType.linear)
				.ValueTo(Vector3.one + new Vector3(1, 0, 0) * 2/ 2.5f, defaultColor, EaseType.linear, value => UpdateColor(new Color(value.x, value.y, value.z)))
				.Complete(CompleteEffect)
		);
	}

	public void EmitHintEffect() {
		tweenExecutor.SeriesExecute(
			new Tween(0.6f)
				.ValueTo(new Vector3(0, 1, 1), defaultColor, EaseType.linear, value => UpdateColor(new Color(value.x, value.y, value.z)))
				.Complete(CompleteEffect)
		);
	}

	void UpdateColor(Color color) {
		spriteRenderer.color = color;
		lineRenderer.material.color = color;
	}
}
