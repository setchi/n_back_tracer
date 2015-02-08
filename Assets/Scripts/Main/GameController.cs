using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	public FadeManager fadeManager;
	public ScreenEffectManager screenEffectManager;

	public PatternTracer patternTracer;
	public TimeKeeper timeKeeper;
	public ScoreManager scoreManager;

	public GameObject timeBar;

	int cachedTouchTileId = -1;
	int currentTouchTileId = -1;

	enum GameState {
		Standby,
		Start,
		Wait,
		Play,
		Timeup,
		Finish
	};
	GameState gameState = GameState.Wait;

	void Awake() {
		fadeManager.FadeIn(0.3f, EaseType.easeInQuart, () => {
			gameState = GameState.Standby;
		});

		timeKeeper.TimeUp += () => {
			gameState = GameState.Timeup;
		};

		patternTracer.PriorNRunEnded += () => {
			gameState = GameState.Start;
		};
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
			gameState = GameState.Wait;
			break;
		
		case GameState.Start:
			gameState = GameState.Play;
			timeBar.SetActive(true);
			timeKeeper.StartCountdown ();
			screenEffectManager.CancelAllAnimate();
			screenEffectManager.EmitGoAnimation();
			break;
		
		case GameState.Wait:
			// do nothing
			break;
		
		case GameState.Play:
			if (cachedTouchTileId != currentTouchTileId) {
				patternTracer.Touch(currentTouchTileId);
				cachedTouchTileId = currentTouchTileId;
			}
			break;
		
		case GameState.Timeup:
			screenEffectManager.EmitTimeupAnimation(() => {
				gameState = GameState.Finish;
			});
			gameState = GameState.Wait;
			break;
		
		case GameState.Finish:
			Storage.Set("Score", scoreManager.GetScore().ToString());
			Application.LoadLevel ("Result");
			break;

		default:
			break;
		
		}
	}
}
