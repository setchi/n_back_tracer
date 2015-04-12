using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class PatternTracer : MonoBehaviour {
	public event Action PriorNRunEnded;
	[SerializeField] ScoreManager scoreManager;
	[SerializeField] GameController gameController;

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
		var patternCache = new Queue<List<int>>();
		patternQueue = new Queue<Stack<int>>(Enumerable.Range(0, backNum)
	    	.Select(i => patternGenerator.Generate(patternCache.SelectMany(stack => stack).ToList()))
	    		.Select(p => { patternCache.Enqueue(p.ToList()); return p; }));

		// Init tile
		tiles = Enumerable.Range (0, tileNum)
			.Select (i => GameObject.Find ("Tile " + i).GetComponent<Tile> ())
				.Select ((tile, i) => {tile.TileId = i; return tile; }).ToList ();

		// Touch event main stream
		var touchStream = Observable.Merge(tiles.Select(tile => tile.onTouchEnter.AsObservable()))
			.Where (_ => gameController.gameState == GameController.GameState.Play)
				.DistinctUntilChanged();

		// Show hint stream
		touchStream.Throttle (TimeSpan.FromSeconds (2)).Repeat()
			.Subscribe(_ => Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(2))
				.TakeUntil(touchStream)
				.Subscribe(__ => StartTrace (0.4f, patternQueue.Peek (), false, tile => tile.EmitHintEffect ())).AddTo(gameObject))
		.AddTo(gameObject);

		// Incorrect touch stream
		touchStream.Where (id => !patternCache.Peek().Where((_, i) => i <= patternGenerator.ChainLength - patternQueue.Peek().Count).Contains(id))
			.Do(_ => scoreManager.IncorrectTouch())
				.Subscribe (id => tiles[id].EmitIncorrectTouchEffect());

		var correctTouchStream = touchStream.Where (id => patternQueue.Peek ().Peek() == id)
			.Select(id => tiles[id])
			.Do(tile => tile.EmitCorrectTouchEffect())
			.Do(_ => scoreManager.CorrectTouch());

		// Correct pattern stream
		correctTouchStream.Buffer(patternGenerator.ChainLength)
			.Select(b => b.Select(tile => tile.TileId))
			.Do(_ => scoreManager.CorrectPattern())
			.Do(_ => patternQueue.Dequeue())
			.Do(_ => patternCache.Dequeue())
				.Subscribe(buffer => StartTrace(0, new Stack<int>(buffer), true, tile => tile.EmitPatternCorrectEffect()));
		
		var drawLineStream = correctTouchStream.Publish();
		drawLineStream.Buffer(2, 1).Take(patternGenerator.ChainLength - 2).Repeat()
			.Subscribe(b => b[1].DrawLine(b[0].gameObject.transform.position));
		drawLineStream.Connect();

		correctTouchStream.Do(_ => patternQueue.Peek().Pop())
			.Where(_ => patternQueue.Peek ().Count == patternGenerator.ChainLength - 1)
			.Subscribe(_ => {
				var ignoreIndexes = patternCache.Peek().Concat(patternCache.Last()).ToList();
				var pattern = patternGenerator.Generate (ignoreIndexes);
				patternQueue.Enqueue(pattern);
				patternCache.Enqueue(pattern.ToList());
				StartTrace(0.4f, pattern, true, tile => tile.EmitMarkEffect());
			});
	}

	void StartTrace(float time, Stack<int> pattern, bool drawLine, Action<Tile> tileEffectEmitter) {
		var traceStream = Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (time / patternGenerator.ChainLength))
			.Zip(pattern.ToObservable(), (_, p) => tiles[p]);

		traceStream.Do(tileEffectEmitter)
			// Drawing line
			.Where (i => drawLine)
				.Buffer(2, 1).Where(b => b.Count > 1)
				.Subscribe(b => b[1].DrawLine(b[0].gameObject.transform.position));
	}

	public void StartPriorNRun() {
		Observable.Timer (TimeSpan.Zero, TimeSpan.FromSeconds (1.3f))
			.Zip(patternQueue.ToObservable(), (_, p) => p)
				.Take (patternQueue.Count)
				.Subscribe (
					pattern => StartTrace (0.4f, pattern, true, tile => tile.EmitMarkEffect ())
					, () => Observable.Timer(TimeSpan.FromSeconds(1.3f)).Subscribe(_ => PriorNRunEnded ()));
	}
}
