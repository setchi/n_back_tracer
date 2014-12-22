using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultSceneUIEventListener : MonoBehaviour {
	GameObject registNameRegion;
	GameObject sendScoreButton;
	GameObject displayRankingButton;

	void Awake() {
		registNameRegion = GameObject.Find("RegistNameRegion");
		sendScoreButton = GameObject.Find("SendScoreButton");
		displayRankingButton = GameObject.Find("DisplayRankingButton");
		registNameRegion.SetActive(false);
		displayRankingButton.SetActive(false);
	}

	void OnClickGoBackButton() {
		Destroy (GameObject.Find ("StorageObject"));
		Application.LoadLevel ("Title");
	}

	void OnClickSendScoreButton() {
		Debug.Log ("OnClickSendScoreButton");
		
		JsonModel.PlayerInfo playerInfo = LocalStorage.Read<JsonModel.PlayerInfo>();
		Debug.Log (playerInfo);

		if (playerInfo == null) {
			registNameRegion.SetActive(true);
			sendScoreButton.SetActive(false);
		} else {
			// ランキング登録
			StartCoroutine(Server.RankEntry(playerInfo, ResultMain.Score, () => {
				sendScoreButton.SetActive(false);
				displayRankingButton.SetActive(true);
			}));
		}
	}

	void OnClickDisplayRankingButton() {
		Debug.Log ("OnClickDisplayRankingButton");
	}

	void OnClickRegistNameButton() {
		// スコア送信
		var nameInputPlaceholder = GameObject.Find("RegistNameInputPlaceholder").GetComponent<Text>();
		var nameInput = GameObject.Find("RegistNameInputText").GetComponent<Text>();

		if (nameInput.text == "") {
			nameInputPlaceholder.color = new Color(1, 0, 0, 0.8f);
			return;
		}

		StartCoroutine(Server.RequestNewPlayerId(response => {
			response.name = nameInput.text;
			LocalStorage.Write<JsonModel.PlayerInfo>(response);

			StartCoroutine(Server.RankEntry(response, ResultMain.Score, () => {
				registNameRegion.SetActive(false);
				displayRankingButton.SetActive(true);
			}));
		}));
	}

	void OnClickRegistNameCancelButton() {
		GameObject.Find("RegistNameInputPlaceholder")
			.GetComponent<Text>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
		registNameRegion.SetActive(false);
		sendScoreButton.SetActive(true);
	}
}
