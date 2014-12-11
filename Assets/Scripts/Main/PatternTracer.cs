using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PatternTracer : MonoBehaviour {
	public event Action PriorNRunEnded;
	PatternGenerator patternGenerator;
	ScoreManager scoreManager;

	const int tileNum = 4 * 5;
	Tile[] tiles = new Tile[tileNum];

	int backNum;
	List<List<int>> patterns = new List<List<int>>();

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

		// nBack分リスト初期化
		for (int i = 0; i <= backNum; i++) {
			patterns.Add(new List<int>());
		}
		
		// nBack分パターン初期化
		for (int i = 0; i <= backNum; i++) {
			UpdatePattern (currentPattern + i, new List<int>());
		}

		// タイル配列初期化
		for (int i = 0; i < tileNum; i++) {
			tiles[i] = GameObject.Find("Tile" + i.ToString()).GetComponent<Tile>();		
			tiles[i].TileId = i;
		}
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
	Action updateTrace;

	// 名前がちょっと変
	void StartPatternTrace(float interval, int targetPattern, int index, Action<Tile> tileEffectEmitter) {
		float timer = 0;

		updateTrace = () => {
			if ((timer += Time.deltaTime) < interval)
				return;
			
			timer = 0;
			tileEffectEmitter (tiles [patterns [targetPattern] [index]]);
			DrawLine(targetPattern, index, currentIndex);

			index++;
			if (index >= patterns [targetPattern].Count) {
				updateTrace = null;
			}
		};
	}

	void DrawLine(int targetPattern, int index, int currentIndex) {
		Tile currentTile = tiles [patterns [targetPattern] [index]];
		Tile prevTile = tiles[patterns[targetPattern][index == 0 ? index : currentIndex != 0 && index == currentIndex ? index : index - 1]];

		currentTile.DrawLine(
			prevTile.gameObject.transform.position - currentTile.gameObject.transform.position
		);
	}
	
	float timer = 0;
	void Update() {
		if (updateTrace != null) {
			updateTrace();
		}

		// スタート時のnBarkRun
		if (isStandby) {
			if (updateTrace != null)
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
				(Tile tile) => tile.EmitHintEffect()
			);
		}
	}

	public void StartPriorNRun() {
		StartPatternTrace (0.4f / patternGenerator.ChainLength, currentPattern, 0, (Tile tile) => tile.EmitMarkEffect());
	}
	
	int CirculatoryIndex(int next, int end) {
		if (next < 0)
			return CirculatoryIndex(end + (next + 1), end);
		return next > end ? CirculatoryIndex(--next - end, end) : next;
	}

	void UpdatePattern(int targetPattern, List<int> ignoreList) {
		patterns [targetPattern] = patternGenerator.Generate (ref ignoreList);
	}

	List<int> BuildIgnoreList(int targetPattern) {
		List<int> ignoreList = new List<int>();

		// トリガーになったやつ
		// List<int> triggerPattern = patterns[LoopIndex (currentPattern - backNum, backNum)];
		// ignoreList.Add (triggerPattern[triggerPattern.Count - 1]);

		foreach (int i in patterns[CirculatoryIndex (targetPattern - backNum, backNum)]) {
			ignoreList.Add(i);
		}

		// 次に出す一個前のパターン全部
		foreach (int i in patterns[CirculatoryIndex(targetPattern - 1, backNum)]) {
			ignoreList.Add(i);
		}

		return ignoreList;
	}

	void PatternIncrement() {
		currentPattern = CirculatoryIndex (currentPattern + 1, backNum);

		int targetPattern = CirculatoryIndex (currentPattern + backNum, backNum);
		List<int> ignoreList = BuildIgnoreList (targetPattern);
		UpdatePattern (targetPattern, ignoreList);
	}

	void IndexIncrement() {
		currentIndex = CirculatoryIndex (currentIndex + 1, patterns [currentPattern].Count - 1);

		// Correct pattern
		if (currentIndex == 0) {
			scoreManager.CorrectPattern();
			PatternIncrement();
		}
	}

	public void Touch(int tileId) {
		hintAnimationTriggerTimer = 0;

		// Correct touch
		if (patterns [currentPattern] [currentIndex] == tileId) {
			scoreManager.CorrectTouch();
			tiles [patterns [currentPattern] [currentIndex]].EmitCorrectEffect ();
			DrawLine(currentPattern, currentIndex, 0);
			
			// start next pattern animation
			if (currentIndex == patternGenerator.ChainLength - 1) {
				StartPatternTrace(
					0.40f / patternGenerator.ChainLength,
					CirculatoryIndex (currentPattern + backNum, backNum),
					0,
					(Tile tile) => tile.EmitMarkEffect()
				);
			}
			IndexIncrement ();
		
		// Miss touch
		} else {
			scoreManager.MissTouch ();
			tiles [tileId].EmitMissEffect ();
		}
	}
}
