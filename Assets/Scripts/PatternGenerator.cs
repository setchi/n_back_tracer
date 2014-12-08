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
		
		int[] field = SetSentinelsOfWall (new int[fieldSize], x, y);
		field = PatternWriteToField (field, ignorePattern);
		
		Stack<int> patternStack = new Stack<int> ();
		
		while (!PatternBuildDFS (
			field,
			new Vector2(Random.Range(0, fieldWidth - 1), Random.Range(0, fieldHeight - 1)),
			Random.Range(0, 3),
			ref patternStack
		));

		return new List<int>(patternStack.ToArray ());
	}
	
	/**
	string ArrToString(int[] arr, int endX, int endY) {
		string str = "";
		for (int y = 0; y < endY; y++) {
			for (int x = 0; x < endX; x++) {
				str += arr[y * endX + x].ToString();
			}
			str += "\n";
		}
		return str;
	}

	string ListToString(List<int> list) {
		string res = "";
		
		foreach (var i in list) {
			res += i.ToString() + " ";
		}
		return res;
	}
	/**/
	
	bool PatternBuildDFS(int[] field, Vector2 currentPos, int direction, ref Stack<int> stack) {
		if (stack.Count == 4)
			return true;
		
		if (IsWall(field, currentPos))
			return false;
		
		stack.Push (fieldWidth * (int)currentPos.y + (int)currentPos.x);
		
		// 後ろは見ない
		List<int> notBackIndexes = new List<int>(new int[] {0, 1, 3});
		for (int i = 0, end = notBackIndexes.Count ; i < end; i++) {
			int notBackIndex = notBackIndexes[Random.Range(0, end - i - 1)];
			int newDirection = LoopIndex (direction + notBackIndex, directions.Length - 1);
			
			// 進めるところまで進む
			if (PatternBuildDFS(field, currentPos + directions[newDirection], newDirection, ref stack)) {
				return true;
			}
			notBackIndexes.Remove(notBackIndex);
		}
		
		stack.Pop ();
		return false;
	}
	
	bool IsWall(int[] field, Vector2 pos) {
		return field [((int)pos.y + 1) * (fieldWidth + 2) + (int)pos.x + 1] == 1;
	}
	
	int[] PatternWriteToField(int[] field, List<int> pattern) {
		foreach (int idx in pattern) {
			int fieldX = idx % fieldWidth + 1;
			int fieldY = idx / fieldWidth + 1;

			field[fieldY * (fieldWidth + 2) + fieldX] = 1;
		}

		return field;
	}
	
	int[] SetSentinelsOfWall(int[] field, int endX, int endY) {
		for (int y = 0; y < endY; y++)
			for (int x = 0; x < endX; x++)
				field[y * endX + x] = (
					x == 0 ||
					y == 0 ||
					x == endX - 1 ||
					y == endY - 1
				) ? 1 : 0;
		
		return field;
	}
	
	int LoopIndex(int next, int end) {
		return next > end ? LoopIndex(--next - end, end) : next;
	}
}
