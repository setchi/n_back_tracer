using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternRunner : MonoBehaviour {
	const int tileNum = 4 * 5;
	Tile[] tiles = new Tile[tileNum];

	const int backNum = 1;
	List<List<int>> patterns = new List<List<int>>();

	PatternGenerator patternGenerator;
	int currentPattern = 0;
	int currentIndex = 0;

	void Awake() {
		patternGenerator = GetComponent<PatternGenerator>();
		patternGenerator.TileNum = tileNum;

		// パターン初期化
		for (int i = 0; i <= backNum; i++) {
			patterns.Add(new List<int>());
			SetNewPattern (currentPattern + i);
		}

		// タイル配列初期化
		for (int i = 0; i < tileNum; i++) {
			tiles[i] = GameObject.Find("Tile" + i.ToString()).GetComponent<Tile>();		
			tiles[i].TileId = i;
		}
	}

	void Start() {
		// 仮ペイント
		foreach (var i in patterns[backNum]) {
			tiles[i].Lighting();
		}
	}
	
	int LoopIndex(int next, int end) {
		return next > end ? LoopIndex(--next - end, end) : next;
	}

	void SetNewPattern(int currentPattern) {
		patterns [currentPattern] = patternGenerator.Generate (4);
	}

	void PatternIncrement() {
		currentPattern = LoopIndex (currentPattern + 1, backNum);
		SetNewPattern (LoopIndex (currentPattern + backNum, backNum));
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
		tiles [patterns[currentPattern] [currentIndex]].LightsOff ();
		tiles [patterns[LoopIndex (currentPattern + backNum, backNum)][currentIndex]].Lighting ();
		IndexIncrement ();
	}
}
