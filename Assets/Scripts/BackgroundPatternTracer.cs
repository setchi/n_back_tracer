using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BackgroundPatternTracer : MonoBehaviour {
	public GameObject Tile;
	public int col;
	public int row;

	List<Tile> tiles = new List<Tile>();
	List<List<int>> patterns;

	void Awake () {
		patterns = BackgroundPatternStore.GetPatterns();
		gameObject.transform.localPosition = new Vector3(-col * 1.7f / 2, -row * 1.7f / 2, 10);

		for (int y = 0; y < row; y++) {
			for (int x = 0; x < col; x++) {
				var obj = (GameObject)Instantiate(Tile, Vector3.zero, transform.rotation);
				obj.transform.SetParent(gameObject.transform);
				obj.transform.localPosition = new Vector3(x * 1.7f, y * 1.7f);
				tiles.Add(obj.GetComponent<Tile>());
			}
		}

		StartCoroutine(StartTrace());
	}

	IEnumerator StartTrace() {
		List<Action<Tile>> tileEffectEmitters = new List<Action<Tile>> {
			tile => tile.EmitMarkEffect(),
			tile => tile.EmitHintEffect()
		};
		int current = 0;

		for (;;) {
			StartCoroutine(Trace(
				patterns[current = ++current % patterns.Count],
				tileEffectEmitters[UnityEngine.Random.Range(0, tileEffectEmitters.Count)]
			));

			yield return new WaitForSeconds(0.7f);
		}
	}

	IEnumerator Trace(List<int> pattern, Action<Tile> emitTileEffect) {
		for (int i = 0, l = pattern.Count; i < l; i++) {
			DrawLine(pattern, i, 0);
			emitTileEffect(tiles[pattern[i]]);
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	void DrawLine(List<int> targetPattern, int index, int currentIndex) {
		Tile currentTile = tiles [targetPattern [index]];
		Tile prevTile = tiles[targetPattern [index == 0 ? index : currentIndex != 0 && index == currentIndex ? index : index - 1]];
		
		currentTile.DrawLine(
			0.8f * /* ← 反対側に飛び出るのを防ぐ暫定対応 */
			(prevTile.gameObject.transform.position - currentTile.gameObject.transform.position)
		);
	}
	
}
