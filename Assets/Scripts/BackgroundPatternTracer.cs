using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class BackgroundPatternTracer : MonoBehaviour {
	public GameObject Tile;
	public int col;
	public int row;

	List<Tile> tiles = new List<Tile>();

	void Awake () {
		transform.localPosition = new Vector3(-col * 1.7f / 2, -row * 1.7f / 2, 10);

		for (int y = 0; y < row; y++) {
			for (int x = 0; x < col; x++) {
				var obj = Instantiate(Tile) as GameObject;
				obj.transform.SetParent(transform);
				obj.transform.localPosition = new Vector3(x * 1.7f, y * 1.7f);
				tiles.Add(obj.GetComponent<Tile>());
			}
		}

		var patterns = BackgroundPatternStore.GetPatterns();
	 	var tileEffectEmitters = new List<Action<Tile>> {
			tile => tile.EmitMarkEffect(),
			tile => tile.EmitHintEffect()
		};

		Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (0.7f))
			.Select(i => Mathf.FloorToInt(i) % patterns.Count)
				.Subscribe(pi => {
		
			Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (0.1f))
				.Select(i => Mathf.FloorToInt(i))
					.Take (patterns[pi].Count)
					.Subscribe (ti => {
			
				DrawLine(patterns[pi], ti, 0);
				tileEffectEmitters[pi % tileEffectEmitters.Count](tiles[patterns[pi][ti]]);
			});
		});
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
