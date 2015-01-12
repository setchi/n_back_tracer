using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankRecord : MonoBehaviour {
	public Text rankTextUI;
	public Text nameTextUI;
	public Text socreTextUI;

	public void Set(int rank, string name, string score, bool isSelf) {
		rankTextUI.text = rank.ToString();
		nameTextUI.text = name;
		socreTextUI.text = score;

		if (isSelf) {
			var selfColor = Color.yellow;

			rankTextUI.color = selfColor;
			nameTextUI.color = selfColor;
			socreTextUI.color = selfColor;
		}
	}
}
