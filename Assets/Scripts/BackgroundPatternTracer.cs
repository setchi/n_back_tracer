﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class BackgroundPatternTracer : MonoBehaviour {
	public GameObject Tile;
	public int col;
	public int row;

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

		Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (0.7f))
			.Zip(patterns.ToObservable(), (_, p) => p).Repeat().Subscribe(pattern => {

			var traceStream = Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (0.1f))
				.Zip(pattern.ToObservable(), (_, i) => tiles[i]);

			traceStream
				.Do(tile => tileEffectEmitters[pattern.Peek() % 2](tile))
				.Zip(traceStream.Skip(1), (prev, current) => new { prev, current })
				.Subscribe(tile => tile.current.DrawLine(tile.prev.gameObject.transform.position))
				.AddTo(gameObject);
		
		}).AddTo(gameObject);
	}
}
