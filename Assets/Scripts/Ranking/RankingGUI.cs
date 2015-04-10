using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;

public class RankingGUI : MonoBehaviour {
	[SerializeField] GameObject rankingPanel;
	[SerializeField] GameObject rankingTable;
	[SerializeField] GameObject rankRecordPrefab;
	[SerializeField] InputField nameInputField;
	[SerializeField] Text messageText;
	[SerializeField] Button nameChangeButton;

	List<GameObject> rankingTableContents = new List<GameObject>();
	bool isHiding = true;

	void Awake () {
		FetchRanking();
		rankingPanel.transform.localPosition = new Vector3(0, -100000, 0);

		// ローカルにユーザ情報が無ければ新規ID取得
		if (LocalData.PlayerInfo == null) {
			API.CreatePlayerId()
				.Subscribe(playerInfo => LocalData.PlayerInfo = playerInfo)
				.AddTo(gameObject);
		}

		Observable.EveryUpdate()
			.Where(_ => Input.GetKey(KeyCode.Escape))
				.Subscribe(_ => Hide());

		var maxLength = 8;
		Func<string, bool> isInvalidName = name => string.IsNullOrEmpty(name) || name.Length > maxLength;
		var nameInputStream = nameInputField.OnEndEditAsObservable()
			.Merge(nameChangeButton.OnClickAsObservable().Select(_ => nameInputField.text));

		nameInputStream.Where(isInvalidName)
				.SubscribeToText(messageText, _ => "名前は1~" + maxLength.ToString() + "文字で入力してください");

		nameInputStream.Where(_ => LocalData.PlayerInfo == null)
				.SubscribeToText(messageText, _ => "通信環境の良い場所でもう一度お試しください");

		var updateNameStream = nameInputStream
			.Where(name => !isInvalidName(name))
				.Where(_ => LocalData.PlayerInfo != null)
				.Select(name => {
					var playerInfo = LocalData.PlayerInfo;
					playerInfo.name = name;
					LocalData.PlayerInfo =  playerInfo;
					return name;
				})
				.SelectMany(name => API.UpdatePlayerName(name));

		updateNameStream.SubscribeToText(messageText, _ => "名前を変更しました");
		updateNameStream.Subscribe(_ => FetchRanking(), e => messageText.text = "通信に失敗しました");
	}

	void FetchRanking() {
		API.FetchRanking()
			.Do(_ => rankingTableContents.ForEach(DestroyObject))
			.Do(_ => rankingTableContents.Clear())
				.SelectMany(records => records.Select((record, index) => new { record, index }))
				.Subscribe(elem => {
					var playerInfo = LocalData.PlayerInfo;
					var selfPlayerId = playerInfo == null ? "" : playerInfo.id;
					var obj = Instantiate(rankRecordPrefab) as GameObject;
					obj.transform.SetParent(rankingTable.transform);
					obj.transform.localScale = Vector3.one;
					obj.GetComponent<RankRecord>().Set(elem.index + 1, elem.record, selfPlayerId);
					rankingTableContents.Add(obj);
				}, e => Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(_ => FetchRanking()).AddTo(gameObject)).AddTo(gameObject);
	}

	public void Show() {
		if (!isHiding)
			return;
		isHiding = false;

		FetchRanking();
		nameInputField.text = LocalData.PlayerInfo.name;
		messageText.text = "";

		rankingPanel.SetActive(true);
		DOTween.Kill(gameObject);
		DOTween.To(() => new Vector3(0, -1700, 0), pos => rankingPanel.transform.localPosition = pos, Vector3.zero, 1f).SetEase(Ease.OutExpo).SetId(gameObject);
	}

	public void Hide() {
		if (isHiding)
			return;
		isHiding = true;

		DOTween.Kill(gameObject);
		DOTween.To(() => Vector3.zero, pos => rankingPanel.transform.localPosition = pos, new Vector3(0, 1700, 0), 1f).SetEase(Ease.OutExpo).SetId(gameObject).OnComplete(() => rankingPanel.SetActive(false));
	}
}
