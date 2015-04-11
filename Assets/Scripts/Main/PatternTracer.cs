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
	Queue<Stack<int>> patternQueue;

	void Awake() {
		int hNum = 4, vNum = 5;
		var tileNum = hNum * vNum;
		patternGenerator = new PatternGenerator(hNum, vNum);
		patternGenerator.ChainLength = int.Parse(Storage.Get("Chain") ?? "4") /* default Chain Num */;
		var backNum = int.Parse(Storage.Get("BackNum") ?? "2") /* default N */;

		// Init pattern queue
		var ignoreIndexes = new List<int>();
		patternQueue = new Queue<Stack<int>>(Enumerable.Range(0, backNum)
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
			.Subscribe(__ => StartTrace (0.4f, patternQueue.Peek (), 0, false, tile => tile.EmitHintEffect ())).AddTo(gameObject))
		.AddTo(gameObject);

		var missTouchStream = touchStream.Where (id => !patternQueue.Peek ().Where ((_, i) => i > 0).Contains (id));
		missTouchStream.Subscribe (id => {
			scoreManager.MissTouch ();
			tiles[id].EmitMissEffect();
		});

		var correctTouchStream = touchStream.Where (id => patternQueue.Peek () .First() == id)
			.Select(id => tiles[id])
			.Do(tile => tile.EmitCorrectTouchEffect())
			.Do(_ => scoreManager.CorrectTouch());

		// DrawLineStream
		correctTouchStream.Buffer(2, 1).Take(patternGenerator.ChainLength - 1).Repeat()
				.Subscribe(b => b[1].DrawLine(b[0].gameObject.transform.position));

		// パターン入力完了後のエフェクト
		var correctPatternStream = correctTouchStream.Buffer(patternGenerator.ChainLength);
		correctPatternStream.Select(b => b.Select(tile => tile.TileId))
				.Do(_ => scoreManager.CorrectPattern())
				.Do(_ => patternQueue.Dequeue())
				.Subscribe(buffer => StartTrace(0, new Stack<int>(buffer), 0, true, tile => tile.EmitPatternCorrectEffect()));

		correctTouchStream.Subscribe(_ => {
			patternQueue.Peek ().Pop();

			if (patternQueue.Peek ().Count == patternGenerator.ChainLength - 1) {
				EnqueueNewPattern();
				StartTrace(0.4f, patternQueue.Last(), 0, true, tile => tile.EmitMarkEffect());
			}
		});
	}

	void StartTrace(float time, Stack<int> pattern, int startIndex, bool drawLine, Action<Tile> tileEffectEmitter) {
		var traceStream = Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (time / patternGenerator.ChainLength))
			.Zip(pattern.ToObservable(), (_, p) => tiles[p]);

		traceStream.Do(tileEffectEmitter)
			// Drawing ilne
			.Where (i => drawLine)
			.Buffer(2, 1).Where(b => b.Count > 1)
			.Subscribe(b => b[1].DrawLine(b[0].gameObject.transform.position));
	}

	public void StartPriorNRun() {
		Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (1.3f))
			.Zip(patternQueue.ToObservable(), (_, p) => p)
				.Take (patternQueue.Count)
				.Subscribe (
					pattern => StartTrace (0.4f, pattern, 0, true, tile => tile.EmitMarkEffect ())
					, () => Observable.Timer(TimeSpan.FromSeconds(1.3f)).Subscribe(_ => PriorNRunEnded ()));
	}

	void EnqueueNewPattern() {
		var ignoreIndexes = new List<int>();
		ignoreIndexes.AddRange(patternQueue.Peek());
		ignoreIndexes.AddRange(patternQueue.Last());
		patternQueue.Enqueue(patternGenerator.Generate (ignoreIndexes));
	}
}
