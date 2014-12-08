using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternRunner : MonoBehaviour {
	const int tileNum = 4 * 5;
	Tile[] tiles = new Tile[tileNum];

	const int backNum = 1;
	List<List<int>> patterns = new List<List<int>>();

	PatternGenerator patternGenerator;
	GameManager gameManager;

	int currentPattern = 0;
	int currentIndex = 0;

	void Awake() {
		patternGenerator = GetComponent<PatternGenerator>();
		patternGenerator.FieldWidth = 4;
		patternGenerator.FieldHeight = 5;

		gameManager = GetComponent<GameManager>();

		// nBack分リスト初期化
		for (int i = 0; i <= backNum; i++) {
			patterns.Add(new List<int>());
		}
		
		// nBack分パターン初期化
		for (int i = 0; i <= backNum; i++) {
			UpdatePattern (currentPattern + i);
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
	float timer = 0;

	void Update() {
		if (!isNBackRun) {
			return;
		}
		
		timer += Time.deltaTime;

		if (timer < 0.14f)
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
			gameManager.FinishNBackRun();
			currentIndex = 0;
			currentPattern = 0;
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

	void UpdatePattern(int targetPattern) {
		patterns [targetPattern] = patternGenerator.Generate (4, patterns[LoopIndex (targetPattern - backNum, backNum)]);
	}

	void PatternIncrement() {
		currentPattern = LoopIndex (currentPattern + 1, backNum);
		UpdatePattern (LoopIndex (currentPattern + backNum, backNum));
	}

	void IndexIncrement() {
		currentIndex = LoopIndex (currentIndex + 1, patterns [currentPattern].Count - 1);

		if (currentIndex == 0) {
			PatternIncrement();
		}
	}

	public void Touch(int tileId) {
		if (patterns[currentPattern] [currentIndex] != tileId) {
			return;
		}
		tiles [patterns[currentPattern] [currentIndex]].StartTouchEffect();
		tiles [patterns[LoopIndex (currentPattern + backNum, backNum)][currentIndex]].StartMarkEffect ();
		IndexIncrement ();
	}
}
