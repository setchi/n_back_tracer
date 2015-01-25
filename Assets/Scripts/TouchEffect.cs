using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class TouchEffect : MonoBehaviour {
	public GameObject[] elements;
	float animTime = 0.4f;
	
	// Use this for initialization
	void Start () {
		StartCoroutine(Emit());
	}
	
	IEnumerator Emit() {
		foreach (var obj in elements) {
			obj.transform.SetParent(gameObject.transform);
			obj.transform.position = transform.position;
			Animate(obj);
			yield return new WaitForSeconds(0.1f);
		}
		
		yield return new WaitForSeconds(animTime);
		DestroyObject(gameObject);
	}
	
	void Animate(GameObject obj) {
		var from = Vector3.zero;
		var to = Vector3.one * 2f;

		TweenPlayer.Play(gameObject, new Tween(animTime).ValueTo(from, to, EaseType.easeOutSine, pos => {
			obj.transform.localScale = pos;
			obj.GetComponent<Image>().color = new Color(1, 1, 1, (2 - pos.x) / 2f);

		}));
	}
}
