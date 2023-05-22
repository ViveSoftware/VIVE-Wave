using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Wave.Native;

public class FramerateTest : MonoBehaviour
{
	public GameObject template;
	public LayoutGroup layoutGroup;

	uint[] availableFrameRates;
	Dictionary<uint, Text> frameRateTextMap = new Dictionary<uint, Text>();

	void Update()
	{
		if (availableFrameRates == null)
		{
			Interop.WVR_GetAvailableFrameRates(out availableFrameRates);
			if (availableFrameRates == null)
				return;

			uint currentFR = 0;
			Interop.WVR_GetFrameRate(ref currentFR);

			foreach (uint fr in availableFrameRates)
			{
				var obj = Instantiate(template, layoutGroup.transform);

				var btn = obj.GetComponent<Button>();
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(() => OnClick(fr.ToString()));

				var text = obj.GetComponentInChildren<Text>();
				text.supportRichText = true;
				if (currentFR == fr)
					text.text = "<b>" + fr.ToString() + "</b>";
				else
					text.text = fr.ToString();
				frameRateTextMap.Add(fr, text);

				obj.SetActive(true);
			}
			StartCoroutine("CheckFrameRate");
		}
	}

	IEnumerator CheckFrameRate()
	{
		var wfs = new WaitForSeconds(0.2f);
		WVR_Result ret;
		uint fr = 0;
		while (true)
		{
			yield return wfs;
			if (availableFrameRates == null || frameRateTextMap.Count == 0)
				yield break;

			Profiler.BeginSample("WVR_GetFrameRate");
			ret = Interop.WVR_GetFrameRate(ref fr);
			Profiler.EndSample();
			Debug.Log("GetFrameRate(" + fr + ")=" + ret);
			if (ret == WVR_Result.WVR_Success)
			{
				foreach (var pair in frameRateTextMap)
				{
					if (pair.Key == fr)
						pair.Value.text = "<b>" + fr.ToString() + "</b>";
					else
						pair.Value.text = pair.Key.ToString();
				}
			}
		}
	}

	public void OnClick(string frStr)
	{
		Profiler.BeginSample("WVR_GetFrameRate");
		Debug.Log("SetFrameRate(" + frStr + ")=" + Interop.WVR_SetFrameRate(uint.Parse(frStr)));
		Profiler.EndSample();
	}
}
