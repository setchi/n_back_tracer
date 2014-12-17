using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	PatternTracer patternTracer;
	TimeKeeper timeKeeper;
	Text timeLimitText;
	ScreenEffectManager screenEffectManager;

	int cachedTouchTileId = -1;
	int currentTouchTileId = -1;

	enum GameState {
		Standby,
		PriorNRun,
		Wait,
		Play,
		Timeup,
		Finish
	};
	GameState gameState = GameState.Standby;

	void Awake() {
		timeKeeper = GetComponent<TimeKeeper>();
		patternTracer = GetComponent<PatternTracer> ();
		screenEffectManager = GetComponent<ScreenEffectManager>();

		timeKeeper.TimeUp += () => {
			gameState = GameState.Timeup;
		};
		patternTracer.PriorNRunEnded += () => {
			gameState = GameState.Play;
			timeKeeper.StartCountdown ();
			screenEffectManager.CancelAllAnimate();
			screenEffectManager.EmitGoAnimation();
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
			patternTracer.StartPriorNRun();
			screenEffectManager.EmitReadyAnimation();
			gameState = GameState.PriorNRun;
			break;
		
		case GameState.PriorNRun:
			gameState = GameState.PriorNRun;
			break;
		
		case GameState.Wait:
			// do nothing
			break;
		
		case GameState.Play:
			if (cachedTouchTileId != currentTouchTileId) {
				patternTracer.Touch(currentTouchTileId);
				cachedTouchTileId = currentTouchTileId;
			}
			timeLimitText.text = "Limit: " + timeKeeper.GetRemainingTime().ToString ();
			break;
		
		case GameState.Timeup:
			timeLimitText.text = "Limit: 0";
			screenEffectManager.EmitTimeupAnimation(() => {
				gameState = GameState.Finish;
			});
			gameState = GameState.Wait;
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
