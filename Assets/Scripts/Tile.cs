using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public int TileId {
		set { tileId = value; }
	}
	int tileId;

	SpriteRenderer spriteRenderer;
	GameManager gameManager;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		gameManager = GameObject.Find ("Tiles").GetComponent<GameManager>();
	}

	void OnMouseOver() {
		gameManager.OnTouchTile (tileId);
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
		StartEffect (1, "OnUpdatePatternEffect");
	}

	public void StartTouchEffect() {
		StartEffect (1, "OnUpdateTouchEffect");
	}

	void OnUpdateMarkEffect(Vector2 value) {
		spriteRenderer.color = new Color (value.x, 1, value.x);
	}

	void OnUpdateTouchEffect(Vector2 value) {
		spriteRenderer.color = new Color (1, value.x, 1);
	}
}
