using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PatternTracer : MonoBehaviour {
	public event Action PriorNRunEnded;
	public ScoreManager scoreManager;

	const int tileNum = 4 * 5;
	List<Tile> tiles;
	PatternGenerator patternGenerator;
	Queue<List<int>> patternQueue;
	int currentIndex = 0;
	IEnumerator hintTraceInterval;

	void Awake() {
		patternGenerator = new PatternGenerator(4, 5);
		patternGenerator.ChainLength = int.Parse(Storage.Get("Chain") ?? "4") /* default Chain Num */;
		var backNum = int.Parse(Storage.Get("BackNum") ?? "2") /* default N */;

		// Init pattern queue
		var ignoreIndexes = new List<int>();
		patternQueue = new Queue<List<int>>(Enumerable.Range(0, backNum)
		    .Select(i => patternGenerator.Generate(ignoreIndexes))
		    .Select(p => { ignoreIndexes.AddRange(p); return p; }));

		// Init tile
		tiles = Enumerable.Range (0, tileNum)
			.Select(i => GameObject.Find("Tile " + i.ToString()).GetComponent<Tile>())
			.Select((tile, i) => { tile.TileId = i; return tile; }).ToList();
	}

	void StartTrace(float time, List<int> pattern, int startIndex, bool drawLine, Action<Tile> tileEffectEmitter) {
		StartCoroutine(Trace(time / patternGenerator.ChainLength, pattern, startIndex, drawLine, tileEffectEmitter));
	}
	IEnumerator Trace(float interval, List<int> pattern, int startIndex, bool drawLine, Action<Tile> emitTileEffect) {
		foreach (var i in Enumerable.Range(startIndex, pattern.Count - startIndex)) {
			emitTileEffect(tiles[pattern[i]]);
			if (drawLine)
				DrawLine(pattern, i, startIndex);
			
			yield return new WaitForSeconds(interval);
		}
	}
	
	public void StartPriorNRun() {
		StartCoroutine(StartPriorRun());
	}
	IEnumerator StartPriorRun() {
		foreach (var pattern in patternQueue) {
			StartTrace (0.4f, pattern, 0, true, tile => tile.EmitMarkEffect());
			yield return new WaitForSeconds(1.3f);
		}
		PriorNRunEnded();
		SetTimeoutHintTrace();
	}

	void SetTimeoutHintTrace() {
		if (hintTraceInterval != null) StopCoroutine(hintTraceInterval);
		StartCoroutine(hintTraceInterval = HintTrace(2f));
	}
	IEnumerator HintTrace(float interval) {
		while (true) {
			yield return new WaitForSeconds(interval);
			StartTrace(0.4f, patternQueue.Peek(), currentIndex, false, tile => tile.EmitHintEffect());
		}
	}
	
	public void Touch(int tileId) {
		SetTimeoutHintTrace();
		var currentPattern = patternQueue.Peek();

		// Correct touch
		if (currentPattern[currentIndex] == tileId) {
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
			
		// Miss touch
		} else if (!currentPattern.Where((v, i) => i < currentIndex).Contains(tileId)) {
			scoreManager.MissTouch ();
			tiles[tileId].EmitMissEffect();
		}
	}

	void EnqueueNewPattern() {
		var ignoreIndexes = new List<int>();
		ignoreIndexes.AddRange(patternQueue.Peek());
		ignoreIndexes.AddRange(patternQueue.Last());
		patternQueue.Enqueue(patternGenerator.Generate(ignoreIndexes));
	}
	
	void DrawLine(List<int> targetPattern, int index, int currentIndex) {
		Tile currentTile = tiles[targetPattern[index]];
		Tile prevTile = tiles[targetPattern[index == 0 ? index : currentIndex != 0 && index == currentIndex ? index : index - 1]];
		
		currentTile.DrawLine(
			0.8f * /* ← 反対側に飛び出るのを防ぐ暫定対応 */
			(prevTile.gameObject.transform.position - currentTile.gameObject.transform.position)
		);
	}
}
