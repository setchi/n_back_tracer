using UnityEngine;
using System.Collections;

public class RankingSceneUIEventListener : MonoBehaviour {

	void OnClickReturnButton() {
		TweenPlayer.Play(gameObject, new Tween(0.4f)
		                 .FadeTo(GameObject.Find("FadeMask").GetComponent<SpriteRenderer>(), 1f,  EaseType.linear)
		                 // フェードアウトしてTitleへ遷移
		                 .Complete(() => Application.LoadLevel("Title")));
	}
}
