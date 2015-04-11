using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	[SerializeField] FadeManager fadeManager;
	[SerializeField] ScreenEffectManager screenEffectManager;

	[SerializeField] PatternTracer patternTracer;
	[SerializeField] TimeKeeper timeKeeper;
	[SerializeField] ScoreManager scoreManager;

	[SerializeField] GameObject timeBar;

	public enum GameState {
		Standby,
		Start,
		Wait,
		Play,
		Timeup,
		Finish
	};
	public GameState gameState = GameState.Wait;

	void Awake() {
		fadeManager.FadeIn(0.3f, DG.Tweening.Ease.InQuart, () => {
			gameState = GameState.Standby;
		});

		timeKeeper.TimeUp += () => {
			gameState = GameState.Timeup;
		};

		patternTracer.PriorNRunEnded += () => {
			gameState = GameState.Start;
		};
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
