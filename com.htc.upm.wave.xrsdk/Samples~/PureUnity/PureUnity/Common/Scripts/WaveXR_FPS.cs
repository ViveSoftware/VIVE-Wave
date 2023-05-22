// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.XR.Management;

namespace Wave.XR.Sample
{
	[RequireComponent(typeof(Text))]
	public class WaveXR_FPS : MonoBehaviour
	{
		private Text textField;
		private float fps = 75;
		#if !UNITY_EDITOR && UNITY_ANDROID
		private float targetfps = 75;
		XRDisplaySubsystem displaySubsystem = null;
		#endif
		private float accTime = 0;

		void Awake()
		{
			textField = GetComponent<Text>();
			accTime = 0;
		}

		void LateUpdate()
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			accTime += unscaledDeltaTime;

			// Avoid crash when timeScale is 0.
			if (unscaledDeltaTime == 0)
			{
				textField.text = "0fps";
				return;
			}

			string text = "";

			float interp = unscaledDeltaTime / (0.5f + unscaledDeltaTime);
			float currentFPS = 1.0f / unscaledDeltaTime;
			fps = Mathf.Lerp(fps, currentFPS, interp);
			// Let Show FPS can up to limit
//#if !UNITY_EDITOR && UNITY_ANDROID
//			List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();

//			SubsystemManager.GetInstances(displays);
//			foreach (var display in displays)
//				if (display.running)
//					displaySubsystem = display;
//			if (displaySubsystem != null)
//				displaySubsystem.TryGetDisplayRefreshRate(out targetfps);

//			var showFps = (fps > targetfps) ? targetfps : fps;
//#else
			var showFps = fps;
//#endif
			// Avoid update Canvas too frequently.
			if (accTime < 0.20f)
				return;
			accTime = 0;

			text += Mathf.RoundToInt(showFps) + "fps";
			textField.text = text;
		}

		void Start()
		{
#if !UNITY_EDITOR && UNITY_ANDROID
		var localfps = Application.targetFrameRate;

		if (localfps > 0)
		{
			targetfps = localfps;
		}
#endif
		}
	}

}
