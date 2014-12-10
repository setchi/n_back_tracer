using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternRunner : MonoBehaviour {
	const int tileNum = 4 * 5;
	Tile[] tiles = new Tile[tileNum];

	int backNum;
	List<List<int>> patterns = new List<List<int>>();

	PatternGenerator patternGenerator;
	ScoreManager scoreManager;
	GameController gameController;

	int currentPattern = 0;
	int currentIndex = 0;

	void ApplyStates(GameObject storageObject) {
		Storage storage = storageObject ? storageObject.GetComponent<Storage> () : null;

		backNum = storage ? storage.Get("BackNum") : 1 /* default N */;
		patternGenerator.ChainLength = storage ? storage.Get ("Length") : 4 /* default Chain Num */;
	}

	void Awake() {
		gameController = GetComponent<GameController>();
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
	
	bool isNBackRun = false;
	// Mark Animation
	bool isMarkAnimation = false;
	int markAnimationPattern = 0;
	int markAnimationCurrentIndex = 0;

	// Hint Animation
	bool isHintAnimation = false;
	int hintAnimationCurrentIndex = 0;
	int hintAnimationPattern = 0;
	float hintAnimationTriggerTimer = 0;


	float timer = 0;

	void Update() {
		// HintAnimation
		if (isHintAnimation) {
			timer += Time.deltaTime;

			if (timer < 0.10f)
				return;
			timer = 0;

			tiles[patterns[currentPattern][hintAnimationCurrentIndex]].StartHintEffect();

			hintAnimationCurrentIndex++;
			if (hintAnimationCurrentIndex >= patterns[hintAnimationPattern].Count) {
				hintAnimationCurrentIndex = 0;
				hintAnimationPattern = 0;
				isHintAnimation = false;
				return;
			}
		}

		// MarkAnimation
		if (isMarkAnimation) {
			timer += Time.deltaTime;
			
			if (timer < 0.10f)
				return;
			timer = 0;
			
			// 発光
			tiles[patterns[markAnimationPattern][markAnimationCurrentIndex]].StartMarkEffect();
			markAnimationCurrentIndex++;
			
			// アニメーション終了
			if (markAnimationCurrentIndex >= patterns [markAnimationPattern].Count) {
				markAnimationCurrentIndex = 0;
				markAnimationPattern = 0;
				isMarkAnimation = false;
				return;
			}
		}





		// スタート時のnBarkRun
		if (!isNBackRun) {
			hintAnimationTriggerTimer += Time.deltaTime;
			if (hintAnimationTriggerTimer < 2f) {
				return;
			}

			hintAnimationTriggerTimer = 0;
			hintAnimationCurrentIndex = currentIndex;
			hintAnimationPattern = currentPattern;
			isHintAnimation = true;
			return;
		}
		
		timer += Time.deltaTime;

		if (timer < 0.12f)
			return;
		timer = 0;

		// 発光
		tiles[patterns[currentPattern][currentIndex]].StartMarkEffect();
		currentIndex++;

		if (currentIndex < patterns [currentPattern].Count) {
			return;
		}

		// 次のパターンを走らせるまで少し時間を置く
		timer = -0.9f;

		currentIndex = 0;
		currentPattern++;

		// 条件も仮
		if (currentPattern >= backNum) {
			// finish
			gameController.FinishNBackRun();
			currentIndex = 0;
			currentPattern = 0;
			timer = 0;
			isNBackRun = false;
		}
	}

	public void StartNBackRun() {
		isNBackRun = true;
	}
	
	int LoopIndex(int next, int end) {
		if (next < 0) {
			return LoopIndex(end + (next + 1), end);
		}
		return next > end ? LoopIndex(--next - end, end) : next;
	}

	void UpdatePattern(int targetPattern, List<int> ignoreList) {
		patterns [targetPattern] = patternGenerator.Generate (ref ignoreList);
	}

	List<int> BuildIgnoreList(int targetPattern) {
		List<int> ignoreList = new List<int>();

		// トリガーになったやつ
		// List<int> triggerPattern = patterns[LoopIndex (currentPattern - backNum, backNum)];
		// ignoreList.Add (triggerPattern[triggerPattern.Count - 1]);

		foreach (int i in patterns[LoopIndex (targetPattern - backNum, backNum)]) {
			ignoreList.Add(i);
		}

		// 次に出す一個前のパターン全部
		foreach (int i in patterns[LoopIndex(targetPattern - 1, backNum)]) {
			ignoreList.Add(i);
		}

		return ignoreList;
	}

	void PatternIncrement() {
		currentPattern = LoopIndex (currentPattern + 1, backNum);

		int targetPattern = LoopIndex (currentPattern + backNum, backNum);
		List<int> ignoreList = BuildIgnoreList (targetPattern);
		UpdatePattern (targetPattern, ignoreList);
	}

	void IndexIncrement() {
		currentIndex = LoopIndex (currentIndex + 1, patterns [currentPattern].Count - 1);

		if (currentIndex == 0) {
			scoreManager.CorrectPattern();
			PatternIncrement();
		}
	}

	public void Touch(int tileId) {
		hintAnimationTriggerTimer = 0;

		// 正解
		if (patterns [currentPattern] [currentIndex] == tileId) {
			scoreManager.CorrectTouch();
			tiles [patterns [currentPattern] [currentIndex]].StartCorrectEffect ();

			if (currentIndex == patternGenerator.ChainLength - 1) { // mark animation start index.
				isMarkAnimation = true;
				markAnimationCurrentIndex = 0;
				markAnimationPattern = LoopIndex (currentPattern + backNum, backNum);
			}
			IndexIncrement ();
			return;
		}
		
		scoreManager.Miss ();
		tiles [tileId].StartMissEffect ();
	}
}
