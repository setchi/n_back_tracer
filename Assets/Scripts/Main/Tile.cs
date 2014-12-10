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
		gameController.OnTouchTile (tileId);
	}

	void StartEffect(float time, string onUpdateFunName, iTween.EaseType easeType) {
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", Vector2.zero,
			"to", new Vector2(1, 1),
			"time", time,
			"easetype", easeType,
			"onupdate", onUpdateFunName
		));
	}

	public void StartMarkEffect() {
		StartEffect (0.9f, "OnUpdateMarkEffect", iTween.EaseType.linear);
	}

	public void StartCorrectEffect() {
		StartEffect (0.6f, "OnUpdateCorrectEffect", iTween.EaseType.linear);
	}

	public void StartMissEffect() {
		StartEffect (0.6f, "OnUpdateMissEffect", iTween.EaseType.linear);
	}

	public void StartHintEffect() {
		StartEffect (0.6f, "OnUpdateHintEffect", iTween.EaseType.linear);
	}

	float alpha = 0.8f;
	void OnUpdateMarkEffect(Vector2 value) {
		SetScale ((1 - value.x) * 0.8f + 1);
		SetColor (value.x, 1, value.x, 1 - value.x * alpha);
	}

	void OnUpdateCorrectEffect(Vector2 value) {
		SetScale ((1 - value.x) * 0.8f + 1);
		SetColor (1, value.x, 1, 1 - value.x * alpha);
	}

	void OnUpdateMissEffect(Vector2 value) {
		SetScale ((1 - value.x) * 0.8f + 1);
		SetColor (1, value.x, value.x, 1 - value.x * alpha);
	}

	void OnUpdateHintEffect(Vector2 value) {
		SetColor (1, value.x, 1, 1 - value.x * alpha);
	}

	
	void SetColor(float r, float g, float b, float a) {
		spriteRenderer.color = new Color(r, g, b, a);
	}
	
	void SetScale(float scale) {
		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
