using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	PatternRunner patternRunner;

	int cachedTouchTileId = -1;
	int currentTouchTileId = -1;

	void Awake() {
		patternRunner = GetComponent<PatternRunner> ();
	}

	public void OnTouchTile(int tileId) {
		currentTouchTileId = tileId;
	}

	void Update () {
		if (cachedTouchTileId != currentTouchTileId) {
			// 次のパターンの
			// Debug.Log ("MouseOver!: " + currentTouchTile.ToString());
			patternRunner.Touch(currentTouchTileId);
			cachedTouchTileId = currentTouchTileId;
		}
	}
}
