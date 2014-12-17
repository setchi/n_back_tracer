using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class TweenExecutor : MonoBehaviour {
	static Dictionary<GameObject, List<List<Func<float, bool>>>> UpdateActionListsDic = new Dictionary<GameObject, List<List<Func<float, bool>>>>();
	static Dictionary<GameObject, List<List<Func<float, bool>>>> UpdateActionListsAddRequestsDic = new Dictionary<GameObject, List<List<Func<float, bool>>>>();

	void Update () {
		if (!UpdateActionListsDic.ContainsKey(gameObject))
			InitActionLists(UpdateActionListsDic, gameObject);

		UpdateActionListsDic[gameObject] = UpdateActionListsDic[gameObject].Where(updateActionList => {
			if (updateActionList.Count == 0)
				return false;

			if (!updateActionList[0](Time.deltaTime * Time.timeScale))
				updateActionList.RemoveAt(0);

			return true;
		}).Union (UpdateActionListsAddRequestsDic[gameObject]).ToList();

		UpdateActionListsAddRequestsDic[gameObject].Clear();
	}

	static public void SeriesExecute(GameObject obj, params Tween[] tweens) {
		if (!UpdateActionListsAddRequestsDic.ContainsKey(obj)) {
			obj.AddComponent<TweenExecutor>();
			InitActionLists(UpdateActionListsAddRequestsDic, obj);
		}

		UpdateActionListsAddRequestsDic[obj].Add(tweens
			.Select(tween => tween.GetUpdateAction()).ToList());
	}

	static public void CancelAll(GameObject obj) {
		if(UpdateActionListsDic.ContainsKey(obj))
			UpdateActionListsDic[obj].Clear();

		if (UpdateActionListsAddRequestsDic.ContainsKey(obj))
			UpdateActionListsAddRequestsDic[obj].Clear();
	}

	static void InitActionLists(Dictionary<GameObject, List<List<Func<float, bool>>>> dictionary, GameObject obj) {
		dictionary.Add(obj, new List<List<Func<float, bool>>>());
	}
}
