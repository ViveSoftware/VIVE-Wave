// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using Wave.Native;
using Wave.Essence.Render;

namespace Wave.Essence
{
	public class AdaptiveQuality : MonoBehaviour
	{
		const string TAG = "WVRAQ";
		private static bool isEnabled = false;

		[System.Serializable]
		public class AQSerializableClass
		{
			[Tooltip("Allow set quality strategy is send quality event. SendQualityEvent = false if quality strategy use default.")]
			public bool SendQualityEvent = true;
			[Tooltip("Allow set auto foveation quality strategy. AutoFoveation = false if quality strategy disable foveation.")]
			public bool AutoFoveation = false;
		}
		[Tooltip("The Rendering Performance Improve Strategy for send quality event and handle.")]
		public AQSerializableClass m_RenderingPerformanceImproveStrategy;

		void OnEnable()
		{
			Log.i(TAG, "Enable AQ");
			var flag1 = (m_RenderingPerformanceImproveStrategy.SendQualityEvent)
				? (WVR_QualityStrategy.WVR_QualityStrategy_SendQualityEvent)
				: (WVR_QualityStrategy.WVR_QualityStrategy_Default);
			var flag2 = (m_RenderingPerformanceImproveStrategy.AutoFoveation)
				? (WVR_QualityStrategy.WVR_QualityStrategy_AutoFoveation)
				: (WVR_QualityStrategy.WVR_QualityStrategy_Default);
			Interop.WVR_EnableAdaptiveQuality(true, (uint)(flag1 | flag2));
		}

	void OnDisable()
		{
			StopCoroutine("RunEnableAQ");
			Log.i(TAG, "Disable AQ");
			isEnabled = Interop.WVR_EnableAdaptiveQuality(false, (uint)WVR_QualityStrategy.WVR_QualityStrategy_Default);
			if (!isEnabled)
			{
				Log.i(TAG, "Disabled");
				GetComponent<DynamicResolution>().ResetResolutionScale();
			}
			Log.i(TAG, "SetPerformaceLevels all max");
			Interop.WVR_SetPerformanceLevels(WVR_PerfLevel.WVR_PerfLevel_Maximum, WVR_PerfLevel.WVR_PerfLevel_Maximum);
		}
	}
}
