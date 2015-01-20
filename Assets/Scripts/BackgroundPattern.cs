using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BackgroundPattern : MonoBehaviour {
	static int col = 10;
	static int row = 16;
	PatternGenerator patternGenerator = new PatternGenerator(col, row);

	List<List<int>> pattern_;
	public List<List<int>> Pattern {
		get {
			if (pattern_ == null) {
				patternGenerator.ChainLength = 30;
				pattern_ = Enumerable.Repeat(0, 40).Select(i => patternGenerator.Generate(new List<int>())).ToList();
			}
			return pattern_;
		}
	}


	void Awake() {
		DontDestroyOnLoad(this);
	}
}
