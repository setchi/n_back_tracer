using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HTTP : MonoBehaviour {
	private static HTTP instance;
	
	private HTTP () { // Private Constructor
		Debug.Log("Create SampleSingleton GameObject instance.");
	}
	
	static HTTP Instance {
		get {
			if( instance == null ) {
				GameObject go = new GameObject("HTTPSingleton");
				instance = go.AddComponent<HTTP>();
			}
			return instance;
		}
	}

	public static WWW GET(string url, Action<string> onSuccess, Action<string> onError = null) {
		WWW www = new WWW (url);
		Instance.StartCoroutine (Instance.WaitForRequest (www, onSuccess, onError));
		return www;
	}
	
	public static WWW POST(string url, Dictionary<string, string> post, Action<string> onSuccess, Action<string> onError = null) {
		WWWForm form = new WWWForm();
		
		foreach (var post_arg in post) {
			form.AddField(post_arg.Key, post_arg.Value);
		}
		
		WWW www = new WWW(url, form);
		Instance.StartCoroutine(Instance.WaitForRequest(www, onSuccess, onError));
		return www;
	}
	
	IEnumerator WaitForRequest(WWW www, Action<string> onSuccess, Action<string> onError) {
		yield return www;

		// check for errors
		if (www.error == null) {
			Debug.Log("WWW Ok!: " + www.text);
			onSuccess(www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
			if (onError != null)
				onError(www.error);
		}
	}
}
