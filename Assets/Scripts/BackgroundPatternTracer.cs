using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class BackgroundPatternTracer : MonoBehaviour {
	[SerializeField] GameObject Tile;
	[SerializeField] int col;
	[SerializeField] int row;

	void Awake () {
		transform.localPosition = new Vector3(-col * 1.7f / 2, -row * 1.7f / 2, 10);
		
		var tiles = Enumerable.Range(0, row).SelectMany(y => Enumerable.Range(0, col).Select(x => {
			var obj = Instantiate(Tile) as GameObject;
			obj.transform.SetParent(transform);
			obj.transform.localPosition = new Vector3(x, y) * 1.7f;
			return obj.GetComponent<Tile>();
		})).ToArray();

		var patterns = BackgroundPatternStore.GetPatterns();
	 	var tileEffectEmitters = new List<Action<Tile>> {
			tile => tile.EmitMarkEffect(),
			tile => tile.EmitHintEffect()
		};

		Observable.Interval (TimeSpan.FromMilliseconds (700))
			.Zip(patterns.ToObservable(), (_, p) => p).Repeat().Subscribe(pattern => {

			Observable.Interval (TimeSpan.FromMilliseconds (100))
				.Zip(pattern.ToObservable(), (_, i) => tiles[i])
				.Do(tileEffectEmitters[pattern.Peek() % 2])
				.Buffer(2, 1).Where(b => b.Count > 1)
					.Subscribe(b => b[0].DrawLine(b[1].gameObject.transform.position))
					.AddTo(gameObject);
		
		}).AddTo(gameObject);
	}
}
