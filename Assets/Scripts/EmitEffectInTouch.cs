using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using UniRx;

public class EmitEffectInTouch : MonoBehaviour {
	[SerializeField] GameObject effect;

	[Header("ヒエラルキー上の並び順")]
	[SerializeField] int siblingIndex;

	void Awake() {
		var everyUpdate = Observable.EveryUpdate();
		everyUpdate.SelectMany(_ => Enumerable.Range(0, Input.touchCount))
			.Select(i => Input.GetTouch(i))
			.Where(touch => touch.phase == TouchPhase.Began)
			.Select(touch => (Vector3)touch.position)
			.Merge(everyUpdate.Where(_ => Input.GetMouseButtonDown(0)).Select(_ => Input.mousePosition))
			.Subscribe(pos => {
				var obj = Instantiate(effect) as GameObject;
				obj.transform.SetParent(transform);
				obj.transform.position = pos;
				obj.transform.SetSiblingIndex(siblingIndex);
			});
	}
}
