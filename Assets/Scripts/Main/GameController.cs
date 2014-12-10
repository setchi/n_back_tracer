using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	PatternRunner patternRunner;
	TimeKeeper timeKeeper;
	Text timeLimitText;

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
		timeKeeper = GetComponent<TimeKeeper>();
		patternRunner = GetComponent<PatternRunner> ();
		
		timeKeeper.OnTimeOut += () => {
			gameState = GameState.Finish;
		};
		patternRunner.OnFinishPriorNRun += () => {
			gameState = GameState.Playing;
			timeKeeper.StartCountdown ();
		};

		timeLimitText = GameObject.Find ("TimeLimit").GetComponent<Text>();
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
			patternRunner.StartPriorNRun();
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
			GameObject storageObject = GameObject.Find ("StorageObject");
			Storage storage = storageObject ? storageObject.GetComponent<Storage>() : null;

			if (storage) {
				storage.Set("Score", GetComponent<ScoreManager>().GetScore());
			}
			Application.LoadLevel ("Result");
			break;

		default:
			break;
		
		}
	}
}
