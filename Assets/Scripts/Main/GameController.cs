using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	PatternRunner patternRunner;
	TimeKeeper timeKeeper;
	ScoreManager scoreManager;
	Text timeLimitText;
	Storage storage;

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
		timeKeeper = GetComponent<TimeKeeper>();
		scoreManager = GetComponent<ScoreManager>();
		timeLimitText = GameObject.Find ("TimeLimit").GetComponent<Text>();

		GameObject storageObject = GameObject.Find ("StorageObject");
		storage = storageObject ? storageObject.GetComponent<Storage>() : null;
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
			patternRunner.StartNBackRun();
			gameState = GameState.Wait;
			break;
		
		case GameState.Wait:
			// do nothing
			break;

		case GameState.Playing:
			if (cachedTouchTileId != currentTouchTileId) {
				patternRunner.Touch(currentTouchTileId);
				cachedTouchTileId = currentTouchTileId;
			}

			timeLimitText.text = "Limit: " + timeKeeper.GetRemainingTime().ToString ();

			break;

		case GameState.Finish:
			if (storage) {
				storage.Set("Score", scoreManager.GetScore());
			}
			Application.LoadLevel ("Result");
			break;

		default:
			break;

		}
	}

	public void FinishNBackRun() {
		gameState = GameState.Playing;
		timeKeeper.StartCountdown ();
	}

	public void TimeOver() {
		gameState = GameState.Finish;
	}

}
