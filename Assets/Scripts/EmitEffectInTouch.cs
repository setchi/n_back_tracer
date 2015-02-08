using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class EmitEffectInTouch : MonoBehaviour {
	public GameObject effect;

	[Header("ヒエラルキー上の並び順")]
	public int siblingIndex;

	void Update() {
		/**
		for (int i = 0, l = Input.touchCount; i < l; i++) {
			var touch = Input.GetTouch(i);
			
			if (touch.phase == TouchPhase.Began) {
				Emit(touch.position);
			}
		}
		/*/

		if (Input.GetMouseButtonDown(0)) {
			Emit(Input.mousePosition);
		}
		/**/
	}

	void Emit(Vector3 pos) {
		var obj = (GameObject) Instantiate(effect, Vector3.zero, Quaternion.identity);
		obj.transform.SetParent(gameObject.transform);
		obj.transform.position = pos;
		obj.transform.SetSiblingIndex(siblingIndex);
	}
}
