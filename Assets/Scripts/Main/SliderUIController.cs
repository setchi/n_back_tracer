using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderUIController : MonoBehaviour {
	[SerializeField] Text numberUI;
	[SerializeField] Slider sliderUI;
	[SerializeField] string prefix;
	[SerializeField] Image fill;

	[HideInInspector]
	public int max = 1;

	public float value {
		set {
			fill.color = Color.red - (Color.red - Color.blue) * value;
			sliderUI.value = value;
		}
		get { return sliderUI.value; }
	}
}
