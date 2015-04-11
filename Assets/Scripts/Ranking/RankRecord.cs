using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankRecord : MonoBehaviour {
	[SerializeField] Text rankTextUI;
	[SerializeField] Text nameTextUI;
	[SerializeField] Text chainAndNUI;
	[SerializeField] Text socreTextUI;

	public void Set(int rank, JsonModel.Record record, string selfPlayerId) {
		rankTextUI.text = rank.ToString();
		nameTextUI.text = record.name;
		chainAndNUI.text = record.chain_n;
		socreTextUI.text = record.score;

		if (record.id == selfPlayerId) {
			var selfColor = Color.yellow;

			rankTextUI.color = selfColor;
			nameTextUI.color = selfColor;
			chainAndNUI.color = selfColor;
			socreTextUI.color = selfColor;
		}
	}
}
