using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DispRank : MonoBehaviour {
	GameObject canvas;
	public GameObject rankTableRow;

	void Start () {
		API.FetchRanking(response => {
			DispRankTable(response);
		});

		canvas = GameObject.Find("Canvas");

		// フェードイン
		TweenPlayer.Play(gameObject, new Tween(0.4f).FadeTo(GameObject.Find("FadeMask").GetComponent<SpriteRenderer>(), 0f,  EaseType.linear));
	}

	void DispRankTable(JsonModel.Record[] rankTable) {
		var index = 1;

		foreach (var record in rankTable) {
			DispRecord(index, record);
			index++;
		}
	}

	void DispRecord(int rank, JsonModel.Record record) {
		var row = (GameObject) Instantiate(rankTableRow);
		row.transform.parent = canvas.transform;
		var rectTransform = row.GetComponent<RectTransform>();
		rectTransform.localPosition = new Vector3(0, rank * -20, 0);
		rectTransform.localScale = Vector3.one;
		
		row.transform.FindChild("RankTable_Rank").GetComponent<Text>().text = rank.ToString();
		row.transform.FindChild("RankTable_Name").GetComponent<Text>().text = record.name;
		row.transform.FindChild("RankTable_Score").GetComponent<Text>().text = record.score;
	}
}
