using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BackgroundPattern : MonoBehaviour {
	static int col = 10;
	static int row = 16;
	int max = 30;
	PatternGenerator patternGenerator = new PatternGenerator(col, row);

	List<List<int>> pattern_;
	public List<List<int>> Pattern {
		get {
			if (pattern_ == null) {
				pattern_ = new List<List<int>>();
				patternGenerator.ChainLength = 20;

				foreach (var i in Enumerable.Range(0, max)) {
					var pattern = patternGenerator.Generate(IgnoreList);
					pattern_.Add(pattern);
					ignoreList_.Add(pattern);
				}
			}
			return pattern_;
		}
	}

	List<List<int>> ignoreList_ = new List<List<int>>();
	List<int> IgnoreList {
		get {
			var list = new List<int>();
			var range = 3;
			for (int i = ignoreList_.Count - 1, j = 0; i >= 0 && j < range; i--, j++) {
				list.AddRange(ignoreList_[i]);
			}

			var overflowed = (ignoreList_.Count + range) - max;
			if (overflowed > 0) {
				foreach (var i in Enumerable.Range(0, overflowed)) {
					list.AddRange(ignoreList_[i]);
				}
			}

			return list;
		}
	}


	void Awake() {
		DontDestroyOnLoad(this);
	}
}
