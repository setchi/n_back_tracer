using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;

public class Tile : MonoBehaviour {
	public Subject<int> onTouchEnter = new Subject<int>();

	public int TileId { set { tileId = value; } }
	int tileId;

	SpriteRenderer spriteRenderer;
	LineRenderer lineRenderer;

	Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 1);

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		lineRenderer = GetComponentInChildren<LineRenderer>();
		lineRenderer.SetWidth (0.13f, 0.13f);
		// 線のスタート位置は常にタイルの中心
		lineRenderer.SetPosition(0, Vector3.zero);
	}

	public void DrawLine(Vector3 endPosition) {
		lineRenderer.SetPosition(1, endPosition);
	}
	
	void EraseLine() { DrawLine (Vector3.zero); }

	void CompleteEffect() {
		EraseLine();
	}

	void OnMouseEnter() {
		onTouchEnter.OnNext (tileId);
	}

	public void EmitMarkEffect() {
		DOTween.Kill(gameObject);
		DOTween.To(() => Color.green, UpdateColor, defaultColor, 1f).SetId(gameObject).OnComplete(CompleteEffect);
	}

	public void EmitCorrectTouchEffect() {
		DOTween.Kill(gameObject);
		DOTween.To(() => Color.white, UpdateColor, Color.cyan, 0.4f).SetEase(Ease.OutBounce).SetId(gameObject);
		DOTween.To(() => Vector3.one * 1.3f, scale => transform.localScale = scale, Vector3.one, 0.4f)
			.SetEase(Ease.OutBounce).SetId(gameObject);
	}

	public void EmitPatternCorrectEffect() {
		DOTween.To(() => Color.cyan, UpdateColor, defaultColor, 0.4f).OnComplete(CompleteEffect).SetId(gameObject);
	}

	public void EmitMissEffect() {
		EraseLine();
		DOTween.Kill(gameObject);
		DOTween.To(() => Color.white + Color.red * 2 / 2.5f, UpdateColor, defaultColor, 0.4f).SetId(gameObject);
		DOTween.To(() => Vector3.one * 1.3f, scale => transform.localScale = scale, Vector3.one, 0.4f)
			.SetId(gameObject).OnComplete(CompleteEffect);
	}

	public void EmitHintEffect() {
		DOTween.To(() => Color.cyan, UpdateColor, defaultColor, 0.6f).SetId(gameObject).OnComplete(CompleteEffect);
	}

	void UpdateColor(Color color) {
		lineRenderer.material.color = spriteRenderer.color = color;
	}
}
