using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	PatternRunner patternRunner;

	int cachedTouchTileId = -1;
	int currentTouchTileId = -1;

	enum GameState {
		Standby,
		nBackRun,
		Wait,
		Playing,
		Finish
	};
	GameState gameState = GameState.Standby;

	void Awake() {
		patternRunner = GetComponent<PatternRunner> ();
	}

	public void OnTouchTile(int tileId) {
		if (gameState != GameState.Playing)
			return;

		currentTouchTileId = tileId;
	}

	void Update () {

		switch (gameState) {
		case GameState.Standby:
			gameState = GameState.nBackRun;
			break;

		case GameState.nBackRun:
			// run start
			patternRunner.StartNBackRun();
			gameState = GameState.Wait;
			break;
		
		case GameState.Wait:
			break;

		case GameState.Playing:
			if (cachedTouchTileId != currentTouchTileId) {
				patternRunner.Touch(currentTouchTileId);
				cachedTouchTileId = currentTouchTileId;
			}
			break;

		case GameState.Finish:
			break;

		default:
			break;

		}
	}

	public void FinishNBackRun() {
		gameState = GameState.Playing;
	}
}
