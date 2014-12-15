using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PatternGenerator : MonoBehaviour {
	int col;
	int row;
	int chainLength = 4; /* default 4 */

	public int Col { set { col = value; } }
	public int Row { set { row = value; } }
	public int ChainLength {
		set { chainLength = value; }
		get { return chainLength; }
	}
	
	public List<int> Generate(List<int> ignoreIndexes) {
		var workX = col + 2;
		var workY = row + 2;

		Func<int, bool> isWall = i => {
			var x = i % workX;
			var y = Mathf.FloorToInt (i / workX);
			return y == 0 || x == 0 || y == workY - 1 || x == workX - 1;
		};
		Func<Func<int, bool>, Func<int, bool>> not = func => i => !func(i);

		var field = InitField (workX, workY, ignoreIndexes, isWall);
		var shuffledStartPos = GenerateShuffledIndexes (0, workX * workY)
			.Where (not (isWall)).ToArray ();

		var patternStack = new Stack<int> ();
		var index = 0;

		while (!PatternDFS (
			ref field,
			shuffledStartPos[index],
			UnityEngine.Random.Range(0, 3),
			ref patternStack
		)) {
			if (++index == shuffledStartPos.Length) {
				index = 0;
				ignoreIndexes.Remove(ignoreIndexes.Last());
				field = InitField(workX, workY, ignoreIndexes, isWall);
			}
		};

		return patternStack.ToList();
	}
	
	int[] InitField(int x, int y, IEnumerable<int> ignoreIndexes, Func<int, bool> isWall) {
		var fieldSize = y * x + x;

		var wallIndexes = Enumerable.Range (0, fieldSize).Where (isWall)
			.Union (
				ignoreIndexes
					// 六角形のはみ出てる部分を除外(仮)
					//  .Union (Enumerable.Range (0, col * row) .Where (i => i % (col * 2) == col - 1))
					// work配列座標に変換
			        .Select (i => Mathf.FloorToInt (i / col + 1) * x + (i % col + 1)
			));

		return Enumerable.Repeat (0, fieldSize)
			.Select ((v, i) => wallIndexes.Contains (i) ? 1 : 0).ToArray ();
	}

	IEnumerable<int> GenerateShuffledIndexes(int start, int count) {
		return Enumerable.Range (start, count).OrderBy (i => Guid.NewGuid ());
	}
	
	bool PatternDFS(ref int[] field, int currentPos, int dirIndex, ref Stack<int> pattern) {
		if (pattern.Count == chainLength)
			return true;

		if (field[currentPos] == 1)
			return false;
		
		field [currentPos] = 1;
		pattern.Push (CalcPatternIndex(currentPos));
		
		int[] directions = { -col - 2, 1, col + 2, -1 };
		foreach (int i in GenerateShuffledIndexes (0, 4)) {
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
		var x = fieldPos % (col + 2) - 1;
		var y = Mathf.FloorToInt (fieldPos / (col + 2)) - 1;
		return y * col + x;
	}
	
	int CirculatoryIndex(int next, int end) {
		if (next < 0)
			return CirculatoryIndex (end + (next + 1), end);
		return next > end ? CirculatoryIndex (--next - end, end) : next;
	}
}
