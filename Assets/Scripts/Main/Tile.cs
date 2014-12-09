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

	void OnMouseOver() {
		gameController.OnTouchTile (tileId);
	}

	void StartEffect(float time, string onUpdateFunName) {
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", Vector2.zero,
			"to", new Vector2(1, 1),
			"time", time,
			"onupdate", onUpdateFunName
		));
	}

	public void StartMarkEffect() {
		StartEffect (0.9f, "OnUpdateMarkEffect");
	}

	public void StartCorrectEffect() {
		StartEffect (0.6f, "OnUpdateCorrectEffect");
	}

	public void StartMissEffect() {
		StartEffect (0.6f, "OnUpdateMissEffect");
	}

	void OnUpdateMarkEffect(Vector2 value) {
		SetScale ((1 - value.x) + 1);
		SetColor (value.x, 1, value.x);
	}

	void OnUpdateCorrectEffect(Vector2 value) {
		SetScale ((1 - value.x) + 1);
		SetColor (1, value.x, 1);
	}

	void OnUpdateMissEffect(Vector2 value) {
		SetScale ((1 - value.x) + 1);
		SetColor (1, value.x, value.x);
	}
	
	void SetColor(float r, float g, float b) {
		spriteRenderer.color = new Color(r, g, b);
	}
	
	void SetScale(float scale) {
		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
