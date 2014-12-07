using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	SpriteRenderer spriteRenderer;
	GameManager gameManager;

	public int TileId {
		set { tileId = value; }
	}
	int tileId;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		gameManager = GameObject.Find ("Tiles").GetComponent<GameManager>();
	}

	void OnMouseOver() {
		gameManager.OnTouchTile (tileId);
	}

	public void Lighting() {
		spriteRenderer.color = new Color (0, 1, 0);
	}

	public void LightsOff() {
		spriteRenderer.color = new Color (1, 0, 0);
	}
}
