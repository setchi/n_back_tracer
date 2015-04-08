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
	int currentIndex = 0;

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

		var missTouchStream = touchStream.Where (id => !patternQueue.Peek ().Where ((_, i) => i <= 0).Contains (id));
		missTouchStream.Subscribe (id => {
			scoreManager.MissTouch ();
			tiles[id].EmitMissEffect();
		});

		var correctTouchStream = touchStream.Where (id => patternQueue.Peek () .First() == id);
		//*
		correctTouchStream.Zip(
				correctTouchStream.Skip(1),
				(current, next) => new { current = tiles[current], next = tiles[next] }
		).Subscribe(t => t.next.DrawLine(t.current.gameObject.transform.position));
		//*/


		var correctPatternStream = correctTouchStream.Buffer(patternGenerator.ChainLength);
		correctPatternStream.Do(_ => scoreManager.CorrectPattern())
			.Subscribe(_ => StartTrace(0, new Stack<int>(_), 0, true, tile => tile.EmitPatternCorrectEffect()));

		correctTouchStream.Subscribe(_ => {
			var currentPattern = patternQueue.Peek ();
			scoreManager.CorrectTouch();
			tiles[currentPattern.Peek()].EmitCorrectTouchEffect();
			// DrawLine(currentPattern, 0, 0);

			currentIndex = currentIndex % patternGenerator.ChainLength;
			currentPattern.Pop();

			// if (currentIndex == 1) {
			if (currentPattern.Count == patternGenerator.ChainLength - 1) {
				EnqueueNewPattern();
				StartTrace(0.4f, patternQueue.Last(), 0, true, tile => tile.EmitMarkEffect());
			}

			// Correct Pattern
			//*
			if (currentPattern.Count == 0) {
				patternQueue.Dequeue();

				// StartTrace(0.0f, patternQueue.Dequeue(), 0, true, tile => tile.EmitPatternCorrectEffect());
			}
			//*/
		});

		// Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Take(3).Repeat().Subscribe(_ => Debug.Log(_));

		/* Correct Touch の Buffer
		Observable.EveryUpdate()
			.Where(_ => Input.GetMouseButtonDown(0))
				.Buffer(5)
				.Subscribe(_ => _.ToList().ForEach(i => Debug.Log(i)));
		*/
	}

	void StartTrace(float time, Stack<int> pattern, int startIndex, bool drawLine, Action<Tile> tileEffectEmitter) {
		var tickStream = Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (time / patternGenerator.ChainLength))
			.Select (i => Mathf.FloorToInt (i))
				.Take (pattern.Count);

		tickStream.Subscribe (i => tileEffectEmitter (tiles [pattern.ElementAt(i)]));

		var drawLineStream = tickStream.Where (i => drawLine);
		drawLineStream.Zip(drawLineStream.Skip(1), (current, next) => new { current = tiles[pattern.ElementAt(current)], next = tiles[pattern.ElementAt(next)] })
			.Subscribe(t => t.next.DrawLine(t.current.gameObject.transform.position));
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

	void DrawLine(Stack<int> targetPattern, int index, int startIndex) {
		Tile currentTile = tiles[targetPattern.ElementAt(index)];
		Tile prevTile = tiles[targetPattern.ElementAt(index == 0 ? index : startIndex != 0 && index == startIndex ? index : index - 1)];

		currentTile.DrawLine(prevTile.gameObject.transform.position);
	}
}
