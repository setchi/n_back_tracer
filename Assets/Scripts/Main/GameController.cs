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
		PriorNRun,
		Wait,
		Play,
		Finish
	};
	GameState gameState = GameState.Standby;

	void Awake() {
		timeKeeper = GetComponent<TimeKeeper>();
		patternRunner = GetComponent<PatternRunner> ();
		
		timeKeeper.TimeUp += () => {
			gameState = GameState.Finish;
		};
		patternRunner.PriorNRunEnded += () => {
			gameState = GameState.Play;
			timeKeeper.StartCountdown ();
		};

		timeLimitText = GameObject.Find ("TimeLimit").GetComponent<Text>();
	}

	public void TouchedTile(int tileId) {
		if (gameState != GameState.Play)
			return;

		currentTouchTileId = tileId;
	}

	void Update () {
		switch (gameState) {
		case GameState.Standby:
			gameState = GameState.PriorNRun;
			break;
		
		case GameState.PriorNRun:
			patternRunner.StartPriorNRun();
			gameState = GameState.Wait;
			break;
		
		case GameState.Wait:
			// do nothing
			break;
		
		case GameState.Play:
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
