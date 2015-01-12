using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderUIController : MonoBehaviour {
	public Text numberUI;
	public Slider sliderUI;
	public string prefix;

	public Image fill;

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
