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
	
	public List<int> Generate(ref List<int> ignorePattern) {
		int workX = fieldWidth + 2;
		int workY = fieldHeight + 2;
		int[] field = SetupField (workX, workY, ref ignorePattern);

		Stack<int> patternStack = new Stack<int> ();
		Stack<int> shuffledStartPos = new Stack<int> (
			GenerateShuffledStackFromRange (0, workX * workY).Where ((i) => {
				int y = Mathf.FloorToInt(i / workX);
				int x = i % workX;
				return y != 0 && x != 0 && y != workY - 1 && x != workX - 1;
			})
		);

		while (!PatternDFS (
			ref field,
			shuffledStartPos.Pop(),
			UnityEngine.Random.Range(0, 3),
			ref patternStack
		)) {
			if (shuffledStartPos.Count == 0) {
				ignorePattern.RemoveAt(ignorePattern.Count - 1);
				field = SetupField(workX, workY, ref ignorePattern);
				shuffledStartPos = GenerateShuffledStackFromRange(0, fieldWidth * fieldHeight);
			}
		};

		return new List<int>(patternStack.ToArray ());
	}
	
	int[] SetupField(int x, int y, ref List<int> ignorePattern) {
		int fieldSize = y * x + x;
		int[] field = SetSentinelsOfWall (new int[fieldSize], x, y);
		return PatternWriteToField (ref field, ref ignorePattern);
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
	
	int[] PatternWriteToField(ref int[] field, ref List<int> pattern) {
		foreach (int idx in pattern) {
			int x = idx % fieldWidth + 1;
			int y = idx / fieldWidth + 1;
			field[y * (fieldWidth + 2) + x] = 1;
		}
		
		return field;
	}

	Stack<int> GenerateShuffledStackFromRange(int start, int count) {
		return new Stack<int> (Enumerable.Range(start, count).OrderBy (i => Guid.NewGuid ()));
	}
	
	bool PatternDFS(ref int[] field, int currentPos, int dirIndex, ref Stack<int> pattern) {
		if (pattern.Count == chainLength)
			return true;

		if (field[currentPos] == 1)
			return false;
		
		field [currentPos] = 1;
		pattern.Push (CalcPatternIndex(currentPos));
		
		int[] directions = { -fieldWidth - 2, 1, fieldWidth + 2, -1 };
		foreach (int i in GenerateShuffledStackFromRange (0, 4)) {
			int newDirIndex = CirculatoryIndex (dirIndex + i, directions.Length - 1);
			// 進めるところまで進む
			if (PatternDFS(ref field, currentPos + directions[newDirIndex], newDirIndex, ref pattern)) {
				return true;
			}
		}
		
		field[currentPos] = 0;
		pattern.Pop ();
		return false;
	}

	int CalcPatternIndex(int fieldPos) {
		int x = fieldPos % (fieldWidth + 2) - 1;
		int y = Mathf.FloorToInt (fieldPos / (fieldWidth + 2)) - 1;
		return y * fieldWidth + x;
	}
	
	
	int CirculatoryIndex(int next, int end) {
		if (next < 0)
			return CirculatoryIndex(end + (next + 1), end);
		return next > end ? CirculatoryIndex(--next - end, end) : next;
	}
}
