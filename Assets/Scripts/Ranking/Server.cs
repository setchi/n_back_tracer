using UnityEngine;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour {
	static string hostName = "http://setchi.jp/unity/b/";

	public static IEnumerator CheckRecord(JsonModel.PlayerInfo playerInfo, int score, Action<JsonModel.CheckRecord> onSuccess) {
		var form = new WWWForm();
		form.AddField("id", playerInfo.id);
		form.AddField("score", score);

		var www = new WWW(hostName + "home/check_record.json", form);
		yield return www;

		Debug.Log (www.text);
		onSuccess(JsonMapper.ToObject<JsonModel.CheckRecord>(www.text));
	}

	public static IEnumerator FetchRanking() {
		var www = new WWW(hostName);
		yield return www;
	}
	
	public static IEnumerator RequestNewPlayerId(Action<JsonModel.PlayerInfo> onSuccess) {
		var www = new WWW(hostName + "home/create_player_id.json");
		yield return www;
		
		onSuccess(JsonMapper.ToObject<JsonModel.PlayerInfo>(www.text));
	}

	public static IEnumerator RankEntry(JsonModel.PlayerInfo playerInfo, int score, Action onSuccess) {
		var form = new WWWForm();
		form.AddField("id", playerInfo.id);
		form.AddField("name", playerInfo.name);
		form.AddField("score", score);

		var www = new WWW(hostName + "home/rank_entry.json", form);
		yield return www;

		onSuccess();
	}
	/*
	public WWW GET(string url) {
		WWW www = new WWW (url);
		StartCoroutine (WaitForRequest (www));
		return www;
	}

	public WWW POST(string url, Dictionary<string,string> post) {
		WWWForm form = new WWWForm();

		foreach(KeyValuePair<String,String> post_arg in post) {
			form.AddField(post_arg.Key, post_arg.Value);
		}

		WWW www = new WWW(url, form);
		StartCoroutine(WaitForRequest(www));
		return www;
	}

	private IEnumerator WaitForRequest(WWW www) {
		yield return www;
		// check for errors
		if (www.error == null) {
			Debug.Log("WWW Ok!: " + www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}
	*/
}
