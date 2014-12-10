using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PatternGenerator : MonoBehaviour {
	int fieldWidth;
	int fieldHeight;
	int chainLength = 4; /* default 4 */

	public int FieldWidth {
		set { fieldWidth = value; }
	}

	public int FieldHeight {
		set { fieldHeight = value; }
	}
	
	public int ChainLength {
		set { chainLength = value; }
		get { return chainLength; }
	}

	Vector2[] directions = {
		Vector2.up,
		Vector2.right,
		-Vector2.up,
		-Vector3.right
	};
	
	public List<int> Generate(ref List<int> ignorePattern) {
		int x = fieldWidth + 2;
		int y = fieldHeight + 2;

		int[] field = SetupField (x, y, ref ignorePattern);
		Stack<int> patternStack = new Stack<int> ();

		int maxTry = 100;
		while (!PatternDFS (
			ref field,
			new Vector2(UnityEngine.Random.Range(0, fieldWidth - 1), UnityEngine.Random.Range(0, fieldHeight - 1)),
			UnityEngine.Random.Range(0, 3),
			ref patternStack
		)) {
			if (--maxTry < 0) {
				ignorePattern.RemoveAt(ignorePattern.Count - 1);
				field = SetupField(x, y, ref ignorePattern);
				maxTry = 100;
			}
		};

		return new List<int>(patternStack.ToArray ());
	}

	int[] SetupField(int x, int y, ref List<int> ignorePattern) {
		int fieldSize = y * x + x;

		int[] field = SetSentinelsOfWall (new int[fieldSize], x, y);
		return PatternWriteToField (ref field, ref ignorePattern);
	}
	
	bool PatternDFS(ref int[] field, Vector2 currentPos, int dirIndex, ref Stack<int> pattern) {
		if (pattern.Count == chainLength)
			return true;

		int fieldPos = CalcFieldPos (currentPos);
		if (field[fieldPos] == 1)
			return false;
		
		field [fieldPos] = 1;
		pattern.Push (fieldWidth * (int)currentPos.y + (int)currentPos.x);

		int[] shuffledIndexes = new int[] {0, 1, 2, 3}.OrderBy (i => Guid.NewGuid ()).ToArray ();
		foreach (int i in shuffledIndexes) {
			int newDirIndex = LoopIndex (dirIndex + i, directions.Length - 1);
			// 進めるところまで進む
			if (PatternDFS(ref field, currentPos + directions[newDirIndex], newDirIndex, ref pattern)) {
				return true;
			}
		}
		
		field[fieldPos] = 0;
		pattern.Pop ();
		return false;
	}

	int CalcFieldPos(Vector2 pos) {
		return ((int)pos.y + 1) * (fieldWidth + 2) + (int)pos.x + 1;
	}
	
	int[] PatternWriteToField(ref int[] field, ref List<int> pattern) {
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
