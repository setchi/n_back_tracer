using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RankingGUI : MonoBehaviour {
	public GameObject rankingPanel;
	public GameObject rankingTable;
	public GameObject rankRecordPrefab;
	public InputField nameInputField;
	public Text messageText;

	List<GameObject> rankingTableContents = new List<GameObject>();
	bool isHiding = true;

	void Awake () {
		FetchRanking();
		rankingPanel.transform.localPosition = new Vector3(0, -100000, 0);

		// ローカルにユーザ情報が無ければ新規ID取得
		if (LocalData.Read().playerInfo == null) {
			FetchPlayerId();
		}
	}

	void Update() {
		if (Input.GetKey(KeyCode.Escape)) {
			Hide();
		}
	}
	
	void Retry(float waitTime, Action action) { StartCoroutine(StartRetry(waitTime, action)); }
	IEnumerator StartRetry(float waitTime, Action action) {
		yield return new WaitForSeconds(waitTime);
		action();
	}

	void FetchPlayerId() {
		API.CreatePlayerId(playerInfo => {
			LocalData.Rewrite(localData => {
				localData.playerInfo = playerInfo;
				return localData;
			});

		}, www => Retry(3f, FetchPlayerId));
	}

	void FetchRanking() {
		API.FetchRanking(records => {
			// テーブルの内容をリセット
			rankingTableContents.ForEach(DestroyObject);
			rankingTableContents.Clear();

			DispRanking(records);

		}, www => Retry(5f, FetchRanking));
	}

	void DispRanking(JsonModel.Record[] records) {
		var playerInfo = LocalData.Read().playerInfo;
		var selfPlayerId = playerInfo == null ? "" : playerInfo.id;

		foreach (var elem in records.Select((record, index) => new {record, index})) {
			var record = (GameObject) Instantiate(rankRecordPrefab);
			record.transform.SetParent(rankingTable.transform);
			record.transform.localScale = Vector3.one;
			record.GetComponent<RankRecord>().Set(elem.index + 1, elem.record, selfPlayerId);
			rankingTableContents.Add(record);
		}
	}

	public void Show() {
		if (!isHiding)return;
		isHiding = false;

		FetchRanking();
		nameInputField.text = LocalData.Read().playerInfo.name;
		messageText.text = "";

		TweenPlayer.Play(gameObject, new Tween(1f)
		                 .ValueTo(new Vector3(0, -1700, 0), Vector3.zero, EaseType.easeOutExpo, pos => rankingPanel.transform.localPosition = pos));
	}

	public void Hide() {
		if (isHiding) return;
		isHiding = true;

		TweenPlayer.Play(gameObject, new Tween(1f)
		                 .ValueTo(Vector3.zero, new Vector3(0, 1700, 0), EaseType.easeOutExpo, pos => rankingPanel.transform.localPosition = pos)
		                 .Complete(() => rankingPanel.transform.localPosition = new Vector3(0, -100000, 0)));
	}

	public void OnClickNameChangeButton() {
		var name = nameInputField.text;

		var maxLength = 8;
		if (name == "" || name.Length > maxLength) {
			messageText.text = "名前は1~" + maxLength.ToString() + "文字で入力してください";
			return;
		}

		var playerInfo = LocalData.Read().playerInfo;
		if (playerInfo == null) {
			messageText.text = "通信環境の良い場所でもう一度お試しください";
			return;
		}

		playerInfo.name = name;
		UpdatePlayerName(playerInfo);
	}
	
	void UpdatePlayerName(JsonModel.PlayerInfo playerInfo) {
		API.UpdatePlayerName(playerInfo, () => {

			LocalData.Rewrite(localData => {
				localData.playerInfo = playerInfo;
				return localData;
			});
			messageText.text = "名前を変更しました";
			
			// ランキングテーブル再取得
			FetchRanking();
			
		}, www => messageText.text = "通信に失敗しました");
	}
}
