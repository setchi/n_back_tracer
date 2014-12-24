using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultMain : MonoBehaviour {
	Storage storage;
	GameObject sendScoreButton;
	GameObject registNameRegion;

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
		DisplayScore();
		
		sendScoreButton = GameObject.Find("SendScoreButton");
		sendScoreButton.SetActive(false);

		registNameRegion = GameObject.Find("RegistNameRegion");
		
		JsonModel.PlayerInfo playerInfo = LocalStorage.Read<JsonModel.PlayerInfo>();

		if (playerInfo == null) {
			sendScoreButton.SetActive(true);
			
		} else {
			// スコア更新していたらランキング登録ボタン表示
			API.CheckRecord(playerInfo, score, response => {
				if (response.is_new_record) {
					sendScoreButton.SetActive(true);
				}
			});
		}
	}
}
