using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankingSceneUI : MonoBehaviour {
	public GameObject rankingView;
	public GameObject rankRecordPrefab;
	public FadeManager fadeManager;

	string selfPlayerId;

	void Start () {
		var playerInfo = LocalStorage.Read<JsonModel.PlayerInfo>();
		selfPlayerId = playerInfo != null ? playerInfo.id : "";

		API.FetchRanking(response => {
			DispRankTable(response);
			fadeManager.FadeIn(0.3f, EaseType.easeInQuart);
		});
	}

	void DispRankTable(JsonModel.Record[] rankTable) {
		var index = 1;

		foreach (var record in rankTable) {
			DispRecord(index, record);
			index++;
		}
	}

	void DispRecord(int rank, JsonModel.Record record) {
		var row = (GameObject) Instantiate(rankRecordPrefab);
		row.transform.parent = rankingView.transform;
		row.transform.localScale = Vector3.one;

		row.GetComponent<RankRecord>().Set(rank, record.name, record.score, record.id == selfPlayerId);
	}

	public void OnClickReturnButton() {
		fadeManager.FadeOut(0.3f, EaseType.easeInQuart, () => Application.LoadLevel("Title"));
	}

}
