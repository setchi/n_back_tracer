using UnityEngine;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class API {
	static string hostName = "http://setchi.jp/unity/b/";

	public static IObservable<JsonModel.CheckRecord> ScoreRegistIfNewRecord(string chainAndN, string score) {
		var form = new WWWForm();
		form.AddField("id", LocalData.PlayerInfo.id);
		form.AddField("chainAndN", chainAndN);
		form.AddField("score", score);
		return ObservableWWW.Post (hostName + "home/regist_if_new_record.json", form)
			.Select(text => JsonMapper.ToObject<JsonModel.CheckRecord>(text));
	}

	public static IObservable<JsonModel.PlayerInfo> UpdatePlayerName(string name) {
		var form = new WWWForm ();
		form.AddField("id", LocalData.PlayerInfo.id);
		form.AddField("name", name);
		return ObservableWWW.Post (hostName + "home/update_player_name.json", form)
			.Select(text => JsonMapper.ToObject<JsonModel.PlayerInfo>(text));
	}

	public static IObservable<JsonModel.PlayerInfo> CreatePlayerId() {
		return ObservableWWW.Get (hostName + "home/create_player_id.json")
			.Select(text => JsonMapper.ToObject<JsonModel.PlayerInfo>(text));
	}

	public static IObservable<JsonModel.Record[]> FetchRanking() {
		return ObservableWWW.Get(hostName + "home/ranking.json")
			.Select(text => JsonMapper.ToObject<JsonModel.Record[]>(text));
	}
}
