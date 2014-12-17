using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class TweenPlayer : MonoBehaviour {
	static Dictionary<GameObject, List<List<Func<float, bool>>>> UpdateActions = new Dictionary<GameObject, List<List<Func<float, bool>>>>();
	static Dictionary<GameObject, List<List<Func<float, bool>>>> AdditionalUpdateActions = new Dictionary<GameObject, List<List<Func<float, bool>>>>();

	void Update () {
		if (!UpdateActions.ContainsKey(gameObject))
			InitActionLists(UpdateActions, gameObject);

		UpdateActions[gameObject] = UpdateActions[gameObject].Where(updateActionList => {
			if (updateActionList.Count == 0)
				return false;

			if (!updateActionList[0](Time.deltaTime * Time.timeScale))
				updateActionList.RemoveAt(0);

			return true;
		}).Union (AdditionalUpdateActions[gameObject]).ToList();

		AdditionalUpdateActions[gameObject].Clear();
	}

	static public void Play(GameObject obj, params Tween[] tweens) {
		if (!AdditionalUpdateActions.ContainsKey(obj)) {
			obj.AddComponent<TweenPlayer>();
			InitActionLists(AdditionalUpdateActions, obj);
		}

		AdditionalUpdateActions[obj].Add(tweens
			.Select(tween => tween.GetUpdateAction()).ToList());
	}

	static public void CancelAll(GameObject obj) {
		if(UpdateActions.ContainsKey(obj))
			UpdateActions[obj].Clear();

		if (AdditionalUpdateActions.ContainsKey(obj))
			AdditionalUpdateActions[obj].Clear();
	}

	static void InitActionLists(Dictionary<GameObject, List<List<Func<float, bool>>>> dictionary, GameObject obj) {
		dictionary.Add(obj, new List<List<Func<float, bool>>>());
	}
}
