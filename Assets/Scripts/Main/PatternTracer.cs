using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PatternTracer : MonoBehaviour {
	public event Action PriorNRunEnded;
	public ScoreManager scoreManager;
	public TimeKeeper timeKeeper;
	PatternGenerator patternGenerator;

	const int tileNum = 4 * 5;
	Tile[] tiles = new Tile[tileNum];
	int backNum;
	List<List<int>> patterns;
	int currentPattern = 0;
	int currentIndex = 0;
	bool isStopping = true;
	float timer = 0;

	void SetStatus() {
		backNum = int.Parse(Storage.Get("BackNum") ?? "1") /* default N */;
		patternGenerator.ChainLength = int.Parse(Storage.Get("Chain") ?? "4") /* default Chain Num */;
	}

	void Awake() {
		patternGenerator = new PatternGenerator(4, 5);
		SetStatus ();
		timeKeeper.TimeUp += () => isStopping = true;

		// パターン初期化
		var ignoreIndexes = new List<int> ();
		patterns = Enumerable.Range (0, backNum + 1)
			.Select (i => {
				var newPattern = patternGenerator.Generate (ignoreIndexes);
				ignoreIndexes.AddRange(newPattern);
				return newPattern;
			}).ToList ();

		// タイル配列初期化
		tiles = Enumerable.Range (0, tileNum)
			.Select (i => {
				var tile = GameObject.Find ("Tile " + i.ToString ()).GetComponent<Tile> ();
				tile.TileId = i;
				return tile;
			}).ToArray ();
	}

	IEnumerator StartPriorRun() {
		foreach (var i in Enumerable.Range(0, backNum)) {
			StartTrace (0.4f, i, 0, true, tile => tile.EmitMarkEffect());

			yield return new WaitForSeconds(1.3f);
		}
		PriorNRunEnded();
		isStopping = false;
	}

	void StartTrace(float time, int patternIndex, int startIndex, bool drawLine, Action<Tile> tileEffectEmitter) {
		StartCoroutine(Trace(time / patternGenerator.ChainLength, patterns[patternIndex], startIndex, drawLine, tileEffectEmitter));
	}
	
	IEnumerator Trace(float interval, List<int> pattern, int startIndex, bool drawLine, Action<Tile> emitTileEffect) {
		foreach (var i in Enumerable.Range(startIndex, pattern.Count)) {
			emitTileEffect(tiles[pattern[i]]);
			if (drawLine)
				DrawLine(pattern, i, startIndex);
			
			yield return new WaitForSeconds(interval);
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
	
	public void StartPriorNRun() {
		StartCoroutine(StartPriorRun());
	}
	
	int CirculatoryIndex(int next, int end) {
		if (next < 0)
			return CirculatoryIndex(end + (next + 1), end);
		return next > end ? CirculatoryIndex(--next - end, end) : next;
	}
	
	List<int> BuildIgnoreIndexes(int targetPattern) {
		return (patterns [CirculatoryIndex (targetPattern - backNum, backNum)])
			.Union (patterns [CirculatoryIndex (targetPattern - 1, backNum)]).ToList ();
	}
	
	void UpdatePattern() {
		var targetPattern = CirculatoryIndex (currentPattern + backNum, backNum);
		var ignoreIndexes = BuildIgnoreIndexes (targetPattern);
		patterns [targetPattern] = patternGenerator.Generate (ignoreIndexes);
	}
	
	public void Touch(int tileId) {
		timer = 0;
		
		// Correct touch
		if (patterns [currentPattern] [currentIndex] == tileId) {
			scoreManager.CorrectTouch();
			tiles [patterns [currentPattern] [currentIndex]].EmitCorrectTouchEffect ();
			DrawLine(patterns[currentPattern], currentIndex, 0);
			
			// index increment
			currentIndex = CirculatoryIndex (currentIndex + 1, patterns [currentPattern].Count - 1);
			
			if (currentIndex == 1) {
				// start next pattern animation
				StartTrace(0.4f, CirculatoryIndex (currentPattern + backNum, backNum), 0, true, tile => tile.EmitMarkEffect());
			}
			
			// Correct Pattern
			if (currentIndex == 0) {
				// tile fade out
				StartTrace(0.0f, CirculatoryIndex(currentPattern, backNum), 0, true, tile => tile.EmitPatternCorrectEffect());
				scoreManager.CorrectPattern();

				// increment pattern index
				currentPattern = CirculatoryIndex (currentPattern + 1, backNum);
				UpdatePattern();
			}
			
			// Miss touch
		} else if (!patterns[currentPattern].Where ((v, i) => i < currentIndex).Contains(tileId)) {
			scoreManager.MissTouch ();
			tiles [tileId].EmitMissEffect ();
		}
	}

	void Update() {
		if (isStopping) return;
		
		timer += Time.deltaTime;
		if (timer < 2f) {
			return;
		}
		timer = 0;
		
		// start hint animation
		StartTrace(0.4f, currentPattern, currentIndex, false, tile => tile.EmitHintEffect());
	}
}
