using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HTTP : MonoBehaviour {
	private static HTTP instance;
	
	// Singleton
	private HTTP () {}
	
	static HTTP Instance {
		get {
			if( instance == null ) {
				GameObject go = new GameObject("HTTPSingleton");
				instance = go.AddComponent<HTTP>();
			}
			return instance;
		}
	}
	
	public static WWW Get(string url, Action<WWW> onSuccess, Action<WWW> onError = null) {
		WWW www = new WWW (url);
		Instance.StartCoroutine (Instance.WaitForRequest (www, onSuccess, onError));
		return www;
	}
	
	public static WWW Post(string url, Dictionary<string, string> postParams, Action<WWW> onSuccess, Action<WWW> onError = null) {
		WWWForm form = new WWWForm();
		
		foreach (var param in postParams) {
			form.AddField(param.Key, param.Value);
		}
		
		WWW www = new WWW(url, form);
		Instance.StartCoroutine(Instance.WaitForRequest(www, onSuccess, onError));
		return www;
	}
	
	IEnumerator WaitForRequest(WWW www, Action<WWW> onSuccess, Action<WWW> onError) {
		yield return www;
		
		// check for errors
		if (string.IsNullOrEmpty(www.error)) {
			Debug.Log("WWW Ok!: " + www.text);
			onSuccess(www);
			
		} else {
			Debug.Log("WWW Error: "+ www.error);
			if (onError != null)
				onError(www);
		}
	}
}
