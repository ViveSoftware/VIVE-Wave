using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DynamicFR))]
public class DynamicFRAdjust : MonoBehaviour
{
	DynamicFR dynamicFR;

	public Slider sliderAV;
	public Slider sliderVF;
	public Slider sliderVB;
	public Slider sliderVZ;
	public Slider sliderFR;
	public Slider sliderFW;  // FOV Wide
	public Slider sliderFN;  // FOV Narrow
	public Slider sliderRS;
	public Slider sliderVS;
	public Toggle toggleEN;

	public Toggle toggleLightMain;
	public Toggle toggleLightLeft;
	public Toggle toggleLightRight;
	public Toggle toggleLightDirectional;
	public Toggle toggleBulletGenerators;

	public Light LightMain1;
	public Light LightMain2;

	public Light LightLeft1;
	public Light LightLeft2;

	public Light LightRight1;
	public Light LightRight2;

	public Light LightDirectional;

	public GameObject bulletGenerators;

	List<Slider> sliders = new List<Slider>();

	bool initialized = false;

	private void OnEnable()
	{
		dynamicFR = GetComponent<DynamicFR>();

		sliders.Add(sliderAV);
		sliders.Add(sliderVF);
		sliders.Add(sliderVB);
		sliders.Add(sliderVZ);
		sliders.Add(sliderFR);
		sliders.Add(sliderFW);
		sliders.Add(sliderFN);
		sliders.Add(sliderRS);
		sliders.Add(sliderVS);
	}

	void Start()
    {
		sliderAV.maxValue = 1;
		sliderAV.minValue = 0;
		sliderVF.maxValue = 1;
		sliderVF.minValue = 0;
		sliderVB.maxValue = 1;
		sliderVB.minValue = 0;
		sliderVZ.maxValue = 1;
		sliderVZ.minValue = 0;

		sliderFR.maxValue = 3;
		sliderFR.minValue = 0;
		sliderFW.maxValue = 179;
		sliderFW.minValue = 1;
		sliderFN.maxValue = 179;
		sliderFN.minValue = 1;

		sliderRS.maxValue = 3;
		sliderRS.minValue = 0;
		sliderVS.maxValue = 3;
		sliderVS.minValue = 0;

		sliderAV.SetValueWithoutNotify(dynamicFR.AVWeight);
		sliderVF.SetValueWithoutNotify(dynamicFR.VFWeight);
		sliderVB.SetValueWithoutNotify(dynamicFR.VBWeight);
		sliderVZ.SetValueWithoutNotify(dynamicFR.VZWeight);
		sliderFR.SetValueWithoutNotify(dynamicFR.FRWeight);
		sliderFN.SetValueWithoutNotify(dynamicFR.FOV_Narrow);
		sliderFW.SetValueWithoutNotify(dynamicFR.FOV_Wide);

		sliderRS.SetValueWithoutNotify(dynamicFR.RSWeight);
		sliderVS.SetValueWithoutNotify(dynamicFR.VSWeight);

		toggleEN.isOn = dynamicFR.isActiveAndEnabled;

		toggleLightMain.isOn = (LightMain1.isActiveAndEnabled && LightMain1.isActiveAndEnabled);
		toggleLightLeft.isOn = (LightLeft1.isActiveAndEnabled && LightLeft1.isActiveAndEnabled);
		toggleLightRight.isOn = (LightRight1.isActiveAndEnabled && LightRight1.isActiveAndEnabled);
		toggleLightDirectional.isOn = LightDirectional.isActiveAndEnabled;

		toggleBulletGenerators.isOn = bulletGenerators.activeInHierarchy;


		foreach (var slider in sliders)
		{
			UpdateText(slider);
		}
		// Before this is done, the slider will set itself earilier. 
		initialized = true;
	}

	public void FOVValidate()
	{
		float w = Mathf.RoundToInt(dynamicFR.FOV_Wide);
		float n = Mathf.RoundToInt(dynamicFR.FOV_Narrow);

		if (w <= n)
		{
			w = n + 1;
			w = Mathf.Clamp(w, 1, 179);
			if (w == n)
				n -= 1;
			
			dynamicFR.FOV_Wide = w;
			sliderFW.SetValueWithoutNotify(w);
			dynamicFR.FOV_Narrow = n;
			sliderFN.SetValueWithoutNotify(n);
		}
	}

	public void OnToggleValueChanged(Toggle toggle)
	{
		//if (Input.GetButtonUp("Button6") || Input.GetButtonUp("Button7"))
		if (toggle == toggleEN)
			dynamicFR.enabled = toggleEN.isOn;

		if (toggle == toggleLightMain)
		{
			LightMain1.enabled = toggle.isOn;
			LightMain2.enabled = toggle.isOn;
		}
		if (toggle == toggleLightLeft)
		{
			LightLeft1.enabled = toggle.isOn;
			LightLeft2.enabled = toggle.isOn;
		}
		if (toggle == toggleLightRight)
		{
			LightRight1.enabled = toggle.isOn;
			LightRight2.enabled = toggle.isOn;
		}
		if (toggle == toggleLightDirectional)
		{
			LightDirectional.enabled = toggle.isOn;
		}
		if (toggle == toggleBulletGenerators)
		{
			bulletGenerators.SetActive(toggle.isOn);
		}
	}

	void UpdateText(Slider slider)
	{
		Transform textTransform = slider.transform.parent.Find("WeightLabel");
		if (textTransform != null)
		{
			Text text = textTransform.GetComponent<Text>();
			text.text = slider.transform.parent.name + "=\n" + slider.value;
		}
	}

	public void OnValueChanged(Slider slider)
	{
		//Debug.Log("Slider value = " + slider.value);

		// Avoid do validate before initialization.
		if (!initialized)
			return;

		if (slider == sliderAV)
			dynamicFR.AVWeight = slider.value;
		else if (slider == sliderVF)
			dynamicFR.VFWeight = slider.value;
		else if (slider == sliderVB)
			dynamicFR.VBWeight = slider.value;
		else if (slider == sliderVZ)
			dynamicFR.VZWeight = slider.value;
		else if (slider == sliderFR)
			dynamicFR.FRWeight = slider.value;
		else if (slider == sliderFW) { 
			dynamicFR.FOV_Wide = Mathf.RoundToInt(slider.value);
			slider.SetValueWithoutNotify(dynamicFR.FOV_Wide);
			FOVValidate();
		} else if (slider == sliderFN) {
			dynamicFR.FOV_Narrow = Mathf.RoundToInt(slider.value);
			slider.SetValueWithoutNotify(dynamicFR.FOV_Narrow);
			FOVValidate();
		} else if (slider == sliderRS)
			dynamicFR.RSWeight = slider.value;
		else if (slider == sliderVS)
			dynamicFR.VSWeight = slider.value;

		UpdateText(slider);
	}
}
