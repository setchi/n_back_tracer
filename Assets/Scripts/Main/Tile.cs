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
	Action UpdateEffect;
	
	Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 1);
	float timer = 0;

	void Awake() {
		gameController = GameObject.Find ("Tiles").GetComponent<GameController>();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		lineRenderer = GetComponentInChildren<LineRenderer>();
		lineRenderer.SetWidth (0.1f, 0.1f);
		// 線のスタート位置は常にタイルの中心
		lineRenderer.SetPosition(0, Vector3.zero);
	}

	void Update() {
		if (UpdateEffect != null) {
			UpdateEffect ();
			timer += Time.deltaTime;
		}
	}

	public void DrawLine(Vector3 endPos) {
		lineRenderer.SetPosition(1, endPos);
	}
	
	void EraseLine() { DrawLine (Vector3.zero); }

	void OnMouseEnter() {
		gameController.TouchedTile (tileId);
	}

	void EmitEffect(float time, Action<float> onUpdate) {
		timer = 0;

		UpdateEffect = () => {
			if (timer < time) {
				onUpdate(timer / time);
			
			} else {
				UpdateEffect = null;
				onUpdate(1);
				CompleteEffect();
			}
		};
	}

	public void EmitMarkEffect() {
		EmitEffect (1f, value => {
			UpdateColor (Color.white * 0.07f + Color.green * 0.93f, defaultColor, value);
			UpdateScale ((1 - value) * 0.3f + 1);
		});
	}

	public void EmitCorrectEffect() {
		EmitEffect (0.4f, value => {
			UpdateColor ((Color.white + Color.cyan * 2) / 2.5f, defaultColor, value);
			UpdateScale ((1 - value) * 0.3f + 1);
		});
	}

	public void EmitMissEffect() {
		EmitEffect (0.6f, value => {
			UpdateColor ((Color.white + Color.red * 2) / 2.5f, defaultColor, value);
			UpdateScale ((1 - value) * 0.3f + 1);
		});
	}

	public void EmitHintEffect() {
		EmitEffect (0.6f, value => {
			UpdateColor ((Color.white + Color.cyan * 2) / 2.5f, defaultColor, value);
		});
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
		EraseLine();
	}
}
