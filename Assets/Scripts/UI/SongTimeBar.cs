using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongTimeBar : MonoBehaviour
{
	public static SongTimeBar Instance = null;

	public Slider slider;
	public Gradient gradient;
	public Image fill;

    private void Awake()
    {
		Instance = this;
    }

    public void SetMaxTime(float maxTime)
	{
		slider.maxValue = maxTime;
		slider.value = 0;
	}

	public void SetTime(float time)
	{
		slider.value = time;

		fill.color = gradient.Evaluate(slider.normalizedValue);
	}
}
