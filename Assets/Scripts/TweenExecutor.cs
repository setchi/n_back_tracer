using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class TweenExecutor : MonoBehaviour {
	List<List<Func<bool>>> UpdateActionLists = new List<List<Func<bool>>>();
	List<List<Func<bool>>> UpdateActionListsAddRequests = new List<List<Func<bool>>>();

	void Update () {
		UpdateActionLists = UpdateActionLists.Where(updateActionList => {
			if (updateActionList.Count == 0)
				return false;

			if (!updateActionList[0]())
				updateActionList.RemoveAt(0);

			return true;
		}).Union (UpdateActionListsAddRequests).ToList();
	}

	public void SeriesExecute(params Tween[] tweens) {
		UpdateActionListsAddRequests.Add(tweens
			.Select(tween => tween.GetUpdateAction()).ToList());
	}

	public TweenExecutor Stop() {
		UpdateActionLists.Clear();
		UpdateActionListsAddRequests.Clear();
		return this;
	}
}
