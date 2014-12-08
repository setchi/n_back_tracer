using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternGenerator : MonoBehaviour {
	
	public int FieldWidth {
		set { fieldWidth = value; }
	}
	public int FieldHeight {
		set { fieldHeight = value; }
	}
	int fieldWidth;
	int fieldHeight;
	
	Vector2[] directions = {
		Vector2.up,
		Vector2.right,
		-Vector2.up,
		-Vector3.right
	};
	
	public List<int> Generate(int length, List<int> ignorePattern) {
		int x = fieldWidth + 2, y = fieldHeight + 2;
		int fieldSize = y * x + x;
		
		int[] fieldArr = SetSentinelsOfWall (new int[fieldSize], x, y);
		fieldArr = PatternWriteToField (fieldArr, ignorePattern, x, y);
		// ArrPrint (testArr, x, y);
		
		Stack<int> patternStack = new Stack<int> ();
		
		while (!PatternBuildDFS (
			fieldArr,
			new Vector2(Random.Range(0, fieldWidth - 1), Random.Range(0, fieldHeight - 1)),
			Random.Range(0, 3),
			ref patternStack
		));
		
		return new List<int>(patternStack.ToArray ());
		
		/**
		int row = 0;
		List<int> pattern = new List<int> ();

		for (int i = 0; i < length; i++) {
			pattern.Add(i * 4 + row);
		}
		row = ++row < 4 ? row : 0;
		Debug.Log (pattern.ToArray().ToString());
		return pattern;
		/**/
	}
	
	/**
	void ArrPrint(int[] arr, int endX, int endY) {
		string str = "";
		for (int y = 0; y < endY; y++) {
			for (int x = 0; x < endX; x++) {
				str += arr[y * endX + x].ToString();
			}
			str += "\n";
		}
		Debug.Log (str);
	}
	/**/
	
	bool PatternBuildDFS(int[] field, Vector2 current, int direction, ref Stack<int> stack) {
		if (stack.Count == 4)
			return true;
		
		if (IsWall(field, current))
			return false;
		
		stack.Push (fieldWidth * (int)current.y + (int)current.x);
		
		// 後ろは見ない
		List<int> notBackIndexes = new List<int>(new int[] {0, 1, 3});
		for (int i = 0, end = notBackIndexes.Count ; i < end; i++) {
			int notBackIndex = notBackIndexes[Random.Range(0, end - i - 1)];
			int newDirection = LoopIndex (direction + notBackIndex, directions.Length - 1);
			
			// 進めるところまで進む
			if (PatternBuildDFS(field, current + directions[newDirection], newDirection, ref stack)) {
				return true;
			}
			notBackIndexes.Remove(notBackIndex);
		}
		
		stack.Pop ();
		return false;
	}
	
	bool IsWall(int[] fieldArr, Vector2 current) {
		return fieldArr [((int)current.y + 1) * (fieldWidth + 2) + (int)current.x + 1] == 1;
	}
	
	int[] PatternWriteToField(int[] arr, List<int> pattern, int endX, int endY) {
		foreach (int i in pattern) {
			int x = i % (endX - 2);
			int y = i / (endY - 2);
			
			arr[y * endX + x + 1] = 1;
		}
		
		return arr;
	}
	
	int[] SetSentinelsOfWall(int[] arr, int endX, int endY) {
		for (int y = 0; y < endY; y++)
			for (int x = 0; x < endX; x++)
				arr[y * endX + x] = (
					x == 0 ||
					y == 0 ||
					x == endX - 1 ||
					y == endY - 1
				) ? 1 : 0;
		
		return arr;
	}
	
	int LoopIndex(int next, int end) {
		return next > end ? LoopIndex(--next - end, end) : next;
	}
}
