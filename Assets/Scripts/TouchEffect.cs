using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class TouchEffect : MonoBehaviour {
	public GameObject[] elements;
	float animTime = 0.4f;

	void Start () {
		StartCoroutine(Emit());
	}
	
	IEnumerator Emit() {
		foreach (var obj in elements) {
			obj.transform.SetParent(gameObject.transform);
			obj.transform.position = transform.position;
			Animate(obj);

			foreach (var i in Enumerable.Range(0, 6))
				yield return new WaitForEndOfFrame();
		}
		
		yield return new WaitForSeconds(animTime);
		DestroyObject(gameObject);
	}
	
	void Animate(GameObject obj) {
		var from = Vector3.zero;
		var to = Vector3.one * 2f;
		var image = obj.GetComponent<Image>();

		TweenPlayer.Play(gameObject, new Tween(animTime).ValueTo(from, to, EaseType.easeOutSine, pos => {
			obj.transform.localScale = pos;
			image.color = new Color(1, 1, 1, (2 - pos.x) / 2f);
		}));
	}
}
