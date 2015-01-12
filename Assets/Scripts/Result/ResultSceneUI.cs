using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultSceneUI : MonoBehaviour {
	public GameObject registNameRegion;
	public GameObject rankEntryButton;
	public GameObject displayRankingButton;
	public FadeManager fadeManager;
	public Text nameInputPlaceholder;
	public InputField nameInputField;
	Storage storage;

	public static int Score {
		get { return score; }
	}
	static int score = 0;
	
	void DisplayScore() {
		GameObject storageObject = GameObject.Find ("StorageObject");
		storage = storageObject ? storageObject.GetComponent<Storage>() : null;
		
		if (storage && storage.Has ("Score")) {
			score = storage.Get("Score");
			GameObject.Find ("Score").GetComponent<Text>().text = "Score: " + storage.Get("Score").ToString();
		}
	}
	
	void Awake() {
		fadeManager.FadeIn(0.4f, EaseType.easeInQuad);
		DisplayScore();
		var playerInfo = LocalStorage.Read<JsonModel.PlayerInfo>();

		if (playerInfo == null) {
			rankEntryButton.SetActive(true);
			
		} else {
			// スコア更新していたらランキング登録ボタン表示
			API.CheckRecord(playerInfo, score, response => {
				if (response.is_new_record) {
					rankEntryButton.SetActive(true);
				}
			});
		}
	}

	public void OnClickReturnButton() {
		LoadLevel("Title");
	}
	
	public void OnClickRankEntryButton() {
		Debug.Log ("OnClickSendScoreButton");
		
		JsonModel.PlayerInfo playerInfo = LocalStorage.Read<JsonModel.PlayerInfo>();
		Debug.Log (playerInfo);
		
		if (playerInfo == null) {
			registNameRegion.SetActive(true);
			rankEntryButton.SetActive(false);
		} else {
			// ランキング登録
			API.RankEntry(playerInfo, ResultSceneUI.Score, () => {
				rankEntryButton.SetActive(false);
				displayRankingButton.SetActive(true);
			});
		}
	}
	
	public void OnClickDisplayRankingButton() {
		LoadLevel("Ranking");
	}
	
	public void OnClickRegistNameButton() {
		// スコア送信
		if (nameInputField.text == "") {
			nameInputPlaceholder.color = new Color(1, 0, 0, 0.8f);
			return;
		}
		
		API.RequestNewPlayerId(response => {
			response.name = nameInputField.text;
			LocalStorage.Write<JsonModel.PlayerInfo>(response);
			
			API.RankEntry(response, ResultSceneUI.Score, () => {
				registNameRegion.SetActive(false);
				displayRankingButton.SetActive(true);
			});
		});
	}
	
	public void OnClickRegistNameCancelButton() {
		nameInputPlaceholder.color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
		registNameRegion.SetActive(false);
		rankEntryButton.SetActive(true);
	}

	void LoadLevel(string levelName) {
		fadeManager.FadeOut(0.4f, EaseType.easeInQuad, () => {
			Destroy (GameObject.Find ("StorageObject"));
			Application.LoadLevel (levelName);
		});
	}
}
