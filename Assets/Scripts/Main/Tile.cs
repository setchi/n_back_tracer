using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public int TileId {
		set { tileId = value; }
	}
	int tileId;

	SpriteRenderer spriteRenderer;
	GameController gameController;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		gameController = GameObject.Find ("Tiles").GetComponent<GameController>();
	}

	void OnMouseEnter() {
		gameController.TouchedTile (tileId);
	}

	void EmitEffect(float time, iTween.EaseType easeType, string updateMethodName) {
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", Vector2.zero,
			"to", new Vector2(1, 1),
			"time", time,
			"easetype", easeType,
			"onupdate", updateMethodName
		));
	}

	public void EmitMarkEffect() {
		EmitEffect (0.9f, iTween.EaseType.linear, "UpdateMarkEffect");
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

	float alpha = 0.9f;
	void UpdateMarkEffect(Vector2 value) {
		SetScale ((1 - value.x) * 0.8f + 1);
		SetColor (value.x, 1, value.x, 1 - value.x * alpha);
	}

	void UpdateCorrectEffect(Vector2 value) {
		SetScale ((1 - value.x) * 0.8f + 1);
		SetColor (value.x, 1, 1, 1 - value.x * alpha);
	}

	void UpdateMissEffect(Vector2 value) {
		SetScale ((1 - value.x) * 0.8f + 1);
		SetColor (1, value.x, value.x, 1 - value.x * alpha);
	}

	void UpdateHintEffect(Vector2 value) {
		SetColor (value.x, 1, 1, 1 - value.x * alpha);
	}

	
	void SetColor(float r, float g, float b, float a) {
		spriteRenderer.color = new Color(r, g, b, a);
	}
	
	void SetScale(float scale) {
		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
