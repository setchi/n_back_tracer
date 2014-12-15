using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PatternTracer : MonoBehaviour {
	public event Action PriorNRunEnded;
	PatternGenerator patternGenerator;
	ScoreManager scoreManager;

	const int tileNum = 4 * 5;
	Tile[] tiles = new Tile[tileNum];

	int backNum;
	List<List<int>> patterns;

	int currentPattern = 0;
	int currentIndex = 0;

	void ApplyStates(GameObject storageObject) {
		Storage storage = storageObject ? storageObject.GetComponent<Storage> () : null;

		backNum = storage ? storage.Get("BackNum") : 1 /* default N */;
		patternGenerator.ChainLength = storage ? storage.Get ("Length") : 4 /* default Chain Num */;
	}

	void Awake() {
		scoreManager = GetComponent<ScoreManager>();
		patternGenerator = GetComponent<PatternGenerator>();
		patternGenerator.Col = 4;
		patternGenerator.Row = 5;
		ApplyStates (GameObject.Find ("StorageObject"));

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

	/* テスト用 */
	string ListToString(List<int> list) {
		string res = "";

		foreach (var i in list) {
			res += i.ToString() + " ";
		}
		return res;
	}
	
	bool isStandby = true;
	float hintAnimationTriggerTimer = 0;
	List<Predicate<int>> updateActions = new List<Predicate<int>> ();

	void StartPatternTrace(float interval, int patternIndex, int startIndex, Action<Tile> tileEffectEmitter, bool drawLine) {
		var timer = 0f;
		var index = startIndex;
		var targetPattern = patterns [patternIndex];

		updateActions.Add (i => {
			if ((timer += Time.deltaTime) < interval)
				return true;
			
			timer = 0;
			tileEffectEmitter (tiles [targetPattern [index]]);

			if (drawLine)
				DrawLine(targetPattern, index, startIndex);
			
			index++;
			if (index < targetPattern.Count) {
				return true;
			}
			return false;
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
	
	float timer = 0;
	void Update() {
		updateActions = updateActions.Where (action => action (0)).ToList ();

		// スタート時のnBarkRun
		if (isStandby) {
			if (updateActions.Count > 0)
				return;

			timer += Time.deltaTime;
			if (timer < 0.9f)
				return;

			// 次のパターンを走らせるまで少し時間を置く
			timer = 0;
			currentPattern++;
			
			// 条件も仮
			if (currentPattern >= backNum) {
				PriorNRunEnded();
				currentPattern = 0;
				isStandby = false;
			} else {
				StartPriorNRun();
			}
		
		} else {
			hintAnimationTriggerTimer += Time.deltaTime;
			if (hintAnimationTriggerTimer < 2f) {
				return;
			}
			hintAnimationTriggerTimer = 0;
			
			// start hint animation
			StartPatternTrace(
				0.4f / patternGenerator.ChainLength,
				currentPattern,
				currentIndex,
				tile => tile.EmitHintEffect(),
				false
			);
		}
	}

	public void StartPriorNRun() {
		StartPatternTrace (
			0.4f / patternGenerator.ChainLength,
			currentPattern,
			0,
			tile => tile.EmitMarkEffect(),
			true
		);
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
		currentPattern = CirculatoryIndex (currentPattern + 1, backNum);

		var targetPattern = CirculatoryIndex (currentPattern + backNum, backNum);
		var ignoreIndexes = BuildIgnoreIndexes (targetPattern);
		patterns [targetPattern] = patternGenerator.Generate (ignoreIndexes);
	}

	void IndexIncrement() {
		currentIndex = CirculatoryIndex (currentIndex + 1, patterns [currentPattern].Count - 1);
	}

	public void Touch(int tileId) {
		hintAnimationTriggerTimer = 0;

		// Correct touch
		if (patterns [currentPattern] [currentIndex] == tileId) {
			scoreManager.CorrectTouch();
			tiles [patterns [currentPattern] [currentIndex]].EmitCorrectTouchEffect ();
			DrawLine(patterns[currentPattern], currentIndex, 0);

			IndexIncrement ();

			// Correct Pattern
			if (currentIndex == 0) {
				// start next pattern animation
				StartPatternTrace(
					0.4f / patternGenerator.ChainLength,
					CirculatoryIndex (currentPattern + backNum, backNum),
					0,
					tile => tile.EmitMarkEffect(),
					true
				);

				StartPatternTrace(
					0.0f / patternGenerator.ChainLength,
					CirculatoryIndex(currentPattern, backNum),
					0,
					tile => tile.EmitPatternCorrectEffect(),
					true
				);

				scoreManager.CorrectPattern();
				UpdatePattern();
			}
		
		// Miss touch
		} else if (!patterns[currentPattern].Where ((v, i) => i < currentIndex).Contains(tileId)) {
			scoreManager.MissTouch ();
			tiles [tileId].EmitMissEffect ();
		}
	}
}
