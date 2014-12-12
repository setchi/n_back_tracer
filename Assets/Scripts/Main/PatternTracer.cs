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
		patternGenerator.FieldWidth = 4;
		patternGenerator.FieldHeight = 5;
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
				return false;
			
			timer = 0;
			tileEffectEmitter (tiles [targetPattern [index]]);

			if (drawLine)
				DrawLine(targetPattern, index, startIndex);
			
			index++;
			if (index < targetPattern.Count) {
				return false;
			}
			return true;
		});
	}

	void DrawLine(List<int> targetPattern, int index, int currentIndex) {
		Tile currentTile = tiles [targetPattern [index]];
		Tile prevTile = tiles[targetPattern [index == 0 ? index : currentIndex != 0 && index == currentIndex ? index : index - 1]];

		currentTile.DrawLine(
			prevTile.gameObject.transform.position - currentTile.gameObject.transform.position
		);
	}
	
	float timer = 0;
	void Update() {
		if (updateActions.Count > 0) {
			var completeActions = new Stack<Predicate<int>>();

			foreach (var updateAction in updateActions) {
				if (updateAction(0)) {
					completeActions.Push(updateAction);
				}
			}

			foreach (var completeAction in completeActions) {
				updateActions.Remove(completeAction);
			}
		}

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
				0.40f / patternGenerator.ChainLength,
				currentPattern,
				currentIndex,
				(Tile tile) => tile.EmitHintEffect(),
				false
			);
		}
	}

	public void StartPriorNRun() {
		StartPatternTrace (
			0.4f / patternGenerator.ChainLength,
			currentPattern,
			0,
			(Tile tile) => tile.EmitMarkEffect(),
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

	void PatternIncrement() {
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
					0.40f / patternGenerator.ChainLength,
					CirculatoryIndex (currentPattern + backNum, backNum),
					0,
					(Tile tile) => tile.EmitMarkEffect(),
					true
				);

				StartPatternTrace(
					0.40f / patternGenerator.ChainLength,
					CirculatoryIndex(currentPattern, backNum),
					0,
					(Tile tile) => tile.EmitPatternCorrectEffect(),
					true
				);

				scoreManager.CorrectPattern();
				PatternIncrement();
			}
		
		// Miss touch
		} else if (!patterns[currentPattern].Contains(tileId)) {
			scoreManager.MissTouch ();
			tiles [tileId].EmitMissEffect ();
		}
	}
}
