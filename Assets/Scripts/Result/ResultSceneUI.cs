using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultSceneUI : MonoBehaviour {
	public GameObject registNameRegion;
	public GameObject rankEntryButton;
	public GameObject displayRankingButton;
	public FadeManager fadeManager;
	public Text nameInputPlaceholder;
	public GameObject nameInputField;
	public GameObject entrySuccessText;
	public GameObject registNameRegionDispRankingButton;
	public GameObject registRecordButton;
	public GameObject registRecordCancelButton;
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
		Debug.Log ("OnClickSendScoreButton");
		
		JsonModel.PlayerInfo playerInfo = LocalStorage.Read<JsonModel.PlayerInfo>();
		Debug.Log (playerInfo);
		
		if (playerInfo == null) {
			displayRankingButton.SetActive(false);
			rankEntryButton.SetActive(true);

		} else {
			// ランキング登録
			API.RankEntry(playerInfo, ResultSceneUI.Score, response => {
				rankEntryButton.SetActive(false);
				displayRankingButton.SetActive(true);

				if (response.is_new_record) {
					// new record
				}
			});
		}
	}

	public void OnClickReturnButton() {
		LoadLevel("Title");
	}
	
	public void OnClickDisplayRankingButton() {
		LoadLevel("Ranking");
	}

	public void OnClickRankEntryButton() {
		registNameRegion.SetActive(true);
		rankEntryButton.SetActive(false);
	}

	// 初回更新時
	public void OnClickRegistNameButton() {
		var inputField = nameInputField.GetComponent<InputField>();
		// スコア送信
		if (inputField.text == "" || inputField.text.Length > 7) {
			nameInputPlaceholder.text = "1~7文字以内で入力";
			nameInputPlaceholder.color = new Color(1, 0, 0, 0.8f);
			inputField.text = "";
			return;
		}

		registRecordButton.SetActive(false);
		registRecordCancelButton.SetActive(false);

		API.RankFirstEntry(inputField.text, Score, response => {
			nameInputField.SetActive(false);
			entrySuccessText.SetActive(true);
			registNameRegionDispRankingButton.SetActive(true);

			LocalStorage.Write<JsonModel.PlayerInfo>(response);
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
