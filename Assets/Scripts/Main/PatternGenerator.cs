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
	
	public List<int> Generate(ref List<int> ignoreIndexes) {
		int workX = fieldWidth + 2;
		int workY = fieldHeight + 2;

		Func<int, bool> isWall = i => {
			int x = i % workX;
			int y = Mathf.FloorToInt (i / workX);
			return y == 0 || x == 0 || y == workY - 1 || x == workX - 1;
		};

		int[] field = InitField (workX, workY, ignoreIndexes, isWall);
		int[] shuffledStartPos = GenerateShuffledIndexFromRange (workX, workY)
			.Where (i => !isWall (i)).ToArray ();

		Stack<int> patternStack = new Stack<int> ();
		int index = 0;

		while (!PatternDFS (
			ref field,
			shuffledStartPos[index],
			UnityEngine.Random.Range(0, 3),
			ref patternStack
		)) {
			if (++index == shuffledStartPos.Length) {
				index = 0;
				ignoreIndexes.RemoveAt(ignoreIndexes.Count - 1);
				field = InitField(workX, workY, ignoreIndexes, isWall);
			}
		};

		return patternStack.ToList();
	}
	
	int[] InitField(int x, int y, List<int> ignoreIndexes, Func<int, bool> isWall) {
		int fieldSize = y * x + x;
		var wallIndexes = Enumerable.Range (0, fieldSize).Where (isWall)
			.Union (ignoreIndexes.Select (
				i => Mathf.FloorToInt (i / fieldWidth + 1) * (fieldWidth + 2) + i % fieldWidth + 1
			));

		return Enumerable.Repeat (0, fieldSize)
			.Select ((v, i) => wallIndexes.Contains (i) ? 1 : v).ToArray ();
	}

	IEnumerable<int> GenerateShuffledIndexFromRange(int start, int count) {
		return Enumerable.Range (start, count).OrderBy (i => Guid.NewGuid ());
	}
	
	bool PatternDFS(ref int[] field, int currentPos, int dirIndex, ref Stack<int> pattern) {
		if (pattern.Count == chainLength)
			return true;

		if (field[currentPos] == 1)
			return false;
		
		field [currentPos] = 1;
		pattern.Push (CalcPatternIndex(currentPos));
		
		int[] directions = { -fieldWidth - 2, 1, fieldWidth + 2, -1 };
		foreach (int i in GenerateShuffledIndexFromRange (0, 4)) {
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
			return CirculatoryIndex (end + (next + 1), end);
		return next > end ? CirculatoryIndex (--next - end, end) : next;
	}
}
