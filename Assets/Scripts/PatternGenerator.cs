using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternGenerator : MonoBehaviour {
	public int TileNum {
		set{ tileNum = value; }
		get { return tileNum; }
	}
	int tileNum;
	int row = 0;

	public List<int> Generate(int length) {
		List<int> pattern = new List<int> ();

		/**/
		for (int i = 0; i < length; i++) {
			pattern.Add(i * 4 + row);
		}
		row = ++row < 4 ? row : 0;
		Debug.Log (pattern.ToArray().ToString());
		return pattern;
		/*/

		pattern.Add(Random.Range (0, tileNum));
		
		for (int i = 1; i < length; i++) {
			int nextIndex;
			do {
				nextIndex = GetChainingIndex(pattern[i - 1]);
			} while (IsCrossed(pattern, nextIndex, i));
			
			pattern.Add(nextIndex);
		}
		
		return pattern;
		/**/
	}
	
	int GetChainingIndex(int current) {
		int[] diff = {1, -1, 4, -4};
		
		while (true) {
			int next = current + diff[Random.Range (0, diff.Length)];
			
			if (IsChaining(current, next)) {
				return next;
			}
		}
	}
	
	bool IsChaining(int current, int next) {
		if (!IsInRange (next)) {
			return false;
		}
		
		if (Mathf.Abs (current - next) == 4) {
			return true;
		}
		
		return current / 4 == next / 4;
	}
	
	bool IsCrossed(List<int> pattern, int next, int max) {
		for (int i = 0; i < max; i++) {
			if (pattern[i] == next)
				return true;
		}
		return false;
	}
	
	bool IsInRange(int index) {
		return index >= 0 && index < tileNum;
	}
}
