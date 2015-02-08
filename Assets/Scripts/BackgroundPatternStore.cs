using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BackgroundPatternStore : SingletonGameObject<BackgroundPatternStore> {
	static int col = 10;
	static int row = 16;
	int max = 30;
	PatternGenerator patternGenerator = new PatternGenerator(col, row);
	List<List<int>> patternList;
	List<List<int>> ignoreList = new List<List<int>>();
	
	void Awake() {
		DontDestroyOnLoad(this);
	}

	List<List<int>> SetupPatternList() {
		patternList = new List<List<int>>();
		patternGenerator.ChainLength = 20;
		
		foreach (var i in Enumerable.Range(0, max)) {
			var pattern = patternGenerator.Generate(GenerateIgnoreList());
			patternList.Add(pattern);
			ignoreList.Add(pattern);
		}

		return patternList;
	}

	List<int> GenerateIgnoreList() {
		var list = new List<int>();
		var range = 3;
		for (int i = ignoreList.Count - 1, j = 0; i >= 0 && j < range; i--, j++) {
			list.AddRange(ignoreList[i]);
		}
		
		var overflowed = (ignoreList.Count + range) - max;
		if (overflowed > 0) {
			foreach (var i in Enumerable.Range(0, overflowed)) {
				list.AddRange(ignoreList[i]);
			}
		}
		
		return list;
	}
	
	public static List<List<int>> GetPatterns() {
		return Instance.patternList ?? Instance.SetupPatternList();
	}
}
