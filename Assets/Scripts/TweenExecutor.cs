using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class TweenExecutor {
	List<List<Func<float, bool>>> UpdateActionLists = new List<List<Func<float, bool>>>();
	List<List<Func<float, bool>>> UpdateActionListsAddRequests = new List<List<Func<float, bool>>>();

	public void Update () {
		UpdateActionLists = UpdateActionLists.Where(updateActionList => {
			if (updateActionList.Count == 0)
				return false;

			if (!updateActionList[0](Time.deltaTime * Time.timeScale))
				updateActionList.RemoveAt(0);

			return true;
		}).Union (UpdateActionListsAddRequests).ToList();
	}

	public void SeriesExecute(params Tween[] tweens) {
		UpdateActionListsAddRequests.Add(tweens
			.Select(tween => tween.GetUpdateAction()).ToList());
	}

	public TweenExecutor CancelAll() {
		UpdateActionLists.Clear();
		UpdateActionListsAddRequests.Clear();
		return this;
	}
}
