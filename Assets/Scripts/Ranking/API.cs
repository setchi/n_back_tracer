using UnityEngine;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;

public class API {
	static string hostName = "http://setchi.jp/unity/b/";
	
	public static void FetchRanking(Action<JsonModel.Record[]> onSuccess, Action<WWW> onError = null) {
		HTTP.Get(hostName + "home/ranking.json", www => {
			onSuccess(JsonMapper.ToObject<JsonModel.Record[]>(www.text));
		}, onError);
	}
	
	public static void ScoreRegistIfNewRecord(string id, string chainAndN, string score, Action<JsonModel.CheckRecord> onSuccess, Action<WWW> onError = null) {
		var form = new Dictionary<string, string>();
		form.Add("id", id);
		form.Add("chainAndN", chainAndN);
		form.Add("score", score);
		
		HTTP.Post(hostName + "home/regist_if_new_record.json", form, www => {
			onSuccess(JsonMapper.ToObject<JsonModel.CheckRecord>(www.text));
		}, onError);
	}
	
	public static void CreatePlayerId(Action<JsonModel.PlayerInfo> onSuccess, Action<WWW> onError = null) {
		HTTP.Get(hostName + "home/create_player_id.json", www => {
			onSuccess(JsonMapper.ToObject<JsonModel.PlayerInfo>(www.text));
		}, onError);
	}
	
	public static void UpdatePlayerName(JsonModel.PlayerInfo playerInfo, Action onSuccess, Action<WWW> onError = null) {
		var form = new Dictionary<string, string>();
		form.Add("id", playerInfo.id);
		form.Add("name", playerInfo.name);
		
		HTTP.Post(hostName + "home/update_player_name.json", form, www => onSuccess(), onError);
	}
}
