using UnityEngine;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;

public class API {
	static string hostName = "http://setchi.jp/unity/b/";

	public static void CheckRecord(JsonModel.PlayerInfo playerInfo, int score, Action<JsonModel.CheckRecord> onSuccess) {
		var form = new Dictionary<string, string>();
		form.Add("id", playerInfo.id);
		form.Add("score", score.ToString());

		HTTP.POST(hostName + "home/check_record.json", form, response => {
			onSuccess(JsonMapper.ToObject<JsonModel.CheckRecord>(response));
		});
	}

	public static IEnumerator FetchRanking() {
		var www = new WWW(hostName);
		yield return www;
	}
	
	public static void RequestNewPlayerId(Action<JsonModel.PlayerInfo> onSuccess) {
		HTTP.GET(hostName + "home/create_player_id.json", response => {
			onSuccess(JsonMapper.ToObject<JsonModel.PlayerInfo>(response));
		});
	}

	public static void RankEntry(JsonModel.PlayerInfo playerInfo, int score, Action onSuccess) {
		var form = new Dictionary<string, string>();
		form.Add("id", playerInfo.id);
		form.Add("name", playerInfo.name);
		form.Add("score", score.ToString());
		HTTP.POST(hostName + "home/rank_entry.json", form, response => onSuccess());
	}
}
