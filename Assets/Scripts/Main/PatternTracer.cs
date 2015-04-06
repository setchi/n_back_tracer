using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class PatternTracer : MonoBehaviour {
	public event Action PriorNRunEnded;
	public ScoreManager scoreManager;
	public GameController gameController;

	List<Tile> tiles;
	PatternGenerator patternGenerator;
	Queue<List<int>> patternQueue;
	int currentIndex = 0;

	void Awake() {
		int hNum = 4, vNum = 5;
		var tileNum = hNum * vNum;
		patternGenerator = new PatternGenerator(hNum, vNum);
		patternGenerator.ChainLength = int.Parse(Storage.Get("Chain") ?? "4") /* default Chain Num */;
		var backNum = int.Parse(Storage.Get("BackNum") ?? "2") /* default N */;

		// Init pattern queue
		var ignoreIndexes = new List<int>();
		patternQueue = new Queue<List<int>>(Enumerable.Range(0, backNum)
		    .Select(i => patternGenerator.Generate(ignoreIndexes))
		    	.Select(p => { ignoreIndexes.AddRange(p); return p; }));

		// Init tile
		tiles = Enumerable.Range (0, tileNum)
			.Select (i => GameObject.Find ("Tile " + i).GetComponent<Tile> ())
				.Select ((tile, i) => {tile.TileId = i; return tile; }).ToList ();

		// Touch event main stream
		var touchStream = tiles.Select (tile => tile.onTouchEnter.AsObservable ())
			.Aggregate(Observable.Merge)
				.DistinctUntilChanged()
				.Where (_ => gameController.gameState == GameController.GameState.Play);

		var showHintStream = touchStream.Throttle (TimeSpan.FromSeconds (2)).Repeat();
		showHintStream.Subscribe(_ => Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(2))
			.TakeUntil(touchStream)
			.Subscribe(__ => StartTrace (0.4f, patternQueue.Peek (), currentIndex, false, tile => tile.EmitHintEffect ())));

		var missTouchStream = touchStream.Where (id => !patternQueue.Peek ().Where ((_, i) => i <= currentIndex).Contains (id));
		missTouchStream.Subscribe (id => {
			scoreManager.MissTouch ();
			tiles[id].EmitMissEffect();
		});

		var correctTouchStream = touchStream.Where (id => patternQueue.Peek () [currentIndex] == id);
		correctTouchStream.Subscribe(_ => {
			var currentPattern = patternQueue.Peek ();

			scoreManager.CorrectTouch();
			tiles[currentPattern[currentIndex]].EmitCorrectTouchEffect();
			DrawLine(currentPattern, currentIndex, 0);

			currentIndex = ++currentIndex % currentPattern.Count;

			if (currentIndex == 1) {
				EnqueueNewPattern();
				StartTrace(0.4f, patternQueue.Last(), 0, true, tile => tile.EmitMarkEffect());
			}

			// Correct Pattern
			if (currentIndex == 0) {
				StartTrace(0.0f, patternQueue.Dequeue(), 0, true, tile => tile.EmitPatternCorrectEffect());
				scoreManager.CorrectPattern();
			}
		});
	}

	void StartTrace(float time, List<int> pattern, int startIndex, bool drawLine, Action<Tile> tileEffectEmitter) {
		var tickStream = Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (time / patternGenerator.ChainLength))
			.Select (i => startIndex + Mathf.FloorToInt (i))
				.Take (pattern.Count - startIndex);

		tickStream.Subscribe (i => tileEffectEmitter (tiles [pattern [i]]));
		tickStream.Where (i => drawLine)
				.Subscribe (i => DrawLine (pattern, i, startIndex));
	}

	public void StartPriorNRun() {
		Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (1.3f))
			.Select (i => Mathf.FloorToInt (i))
				.Take (patternQueue.Count)
				.Subscribe (
					i => StartTrace (0.4f, patternQueue.ElementAt (i), 0, true, tile => tile.EmitMarkEffect ())
					, () => Observable.Timer(TimeSpan.FromSeconds(1.3f)).Subscribe(_ => PriorNRunEnded ()));
	}

	void EnqueueNewPattern() {
		var ignoreIndexes = new List<int>();
		ignoreIndexes.AddRange(patternQueue.Peek());
		ignoreIndexes.AddRange(patternQueue.Last());
		patternQueue.Enqueue(patternGenerator.Generate (ignoreIndexes));
	}

	void DrawLine(List<int> targetPattern, int index, int startIndex) {
		Tile currentTile = tiles[targetPattern[index]];
		Tile prevTile = tiles[targetPattern[index == 0 ? index : startIndex != 0 && index == startIndex ? index : index - 1]];

		currentTile.DrawLine(
			0.8f * /* ← 反対側に飛び出るのを防ぐ暫定対応 */
			(prevTile.gameObject.transform.position - currentTile.gameObject.transform.position)
		);
	}
}
