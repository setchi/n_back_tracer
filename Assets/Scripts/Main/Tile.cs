using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public int TileId {
		set { tileId = value; }
	}
	int tileId;

	SpriteRenderer spriteRenderer;
	GameController gameController;
	LineRenderer lineRenderer;
	int effectDuplicateCount = 0;

	void Awake() {
		gameController = GameObject.Find ("Tiles").GetComponent<GameController>();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		lineRenderer = GetComponentInChildren<LineRenderer>();
		lineRenderer.SetWidth (0.1f, 0.1f);
		// 線のスタート位置は常にタイルの中心
		lineRenderer.SetPosition(0, Vector3.zero);
	}

	public void DrawLine(Vector3 endPos) {
		lineRenderer.SetPosition(1, endPos);
	}
	
	void EraseLine() { DrawLine (Vector3.zero); }

	void OnMouseEnter() {
		gameController.TouchedTile (tileId);
	}

	void EmitEffect(float time, iTween.EaseType easeType, string updateMethodName) {
		effectDuplicateCount++;
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", Vector2.zero,
			"to", new Vector2(1, 1),
			"time", time,
			"easetype", easeType,
			"onupdate", updateMethodName,
			"oncomplete", "CompleteEffect"
		));
	}

	public void EmitMarkEffect() {
		EmitEffect (1f, iTween.EaseType.linear, "UpdateMarkEffect");
	}

	public void EmitCorrectEffect() {
		EmitEffect (0.4f, iTween.EaseType.linear, "UpdateCorrectEffect");
	}

	public void EmitMissEffect() {
		EmitEffect (0.6f, iTween.EaseType.linear, "UpdateMissEffect");
	}

	public void EmitHintEffect() {
		EmitEffect (0.6f, iTween.EaseType.linear, "UpdateHintEffect");
	}


	Color defaultColor = new Color(0.1f, 0.1f, 0.1f, 1);
	void UpdateMarkEffect(Vector2 value) {
		UpdateColor (Color.white * 0.07f + Color.green * 0.93f, defaultColor, value.x);
		UpdateScale ((1 - value.x) * 0.3f + 1);
	}

	void UpdateCorrectEffect(Vector2 value) {
		UpdateColor ((Color.white + Color.cyan * 2) / 2.5f, defaultColor, value.x);
		UpdateScale ((1 - value.x) * 0.3f + 1);
	}

	void UpdateMissEffect(Vector2 value) {
		UpdateColor ((Color.white + Color.red * 2) / 2.5f, defaultColor, value.x);
		UpdateScale ((1 - value.x) * 0.3f + 1);
	}

	void UpdateHintEffect(Vector2 value) {
		UpdateColor ((Color.white + Color.cyan * 2) / 2.5f, defaultColor, value.x);
	}

	void UpdateColor(Color from, Color to, float value) {
		Color color = from - (from - to) * value;
		spriteRenderer.color = color;
		lineRenderer.material.color = color;
	}
	
	void UpdateScale(float scale) {
		transform.localScale = new Vector3 (scale, scale, scale);
	}
	
	void CompleteEffect() {
		// 複数のエフェクトが同時進行している場合、
		// 線を消す処理は最後のエフェクトで行う
		effectDuplicateCount--;
		if (effectDuplicateCount == 0) {
			EraseLine();
		}
	}
}
