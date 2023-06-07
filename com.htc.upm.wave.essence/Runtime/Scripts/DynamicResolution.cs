// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Wave.XR.Settings;
using Wave.Native;
using Wave.Essence.Events;

namespace Wave.Essence.Render
{
	using System;
	using System.Linq;
#if UNITY_EDITOR
	using UnityEditor;

	[CustomEditor(typeof(DynamicResolution))]
	public class DynamicResolutionEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DynamicResolution myScript = target as DynamicResolution;

			WaveXRSettings waveXRSettings = WaveXRSettings.GetInstance();
			if (waveXRSettings != null)
			{
				if (waveXRSettings.adaptiveQualityMode == WaveXRSettings.AdaptiveQualityMode.CustomizationMode)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideXRSettings"));
					serializedObject.ApplyModifiedProperties();
					if (myScript.overrideXRSettings)
					{
						EditorGUILayout.HelpBox("Dynamic Resolution is feature to help adjust the Resolution Scale of the application according to usage of system resources. " +
												"\nIt also helps determining a lower bound for the Resolution Scale to maintain text readability at certain text size.", MessageType.None);
						EditorGUILayout.HelpBox("Specify the smallest size of text that you will use in your application. " + 
												"This parameter will be used while determining the lower bound for maintaining text readability.", MessageType.Info);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("textSize"));
						serializedObject.ApplyModifiedProperties();

						EditorGUILayout.HelpBox("You can define a set of Resolution Scale values which will be applied according to the events triggered by AdaptiveQuality, " +
												"adjust the default Resolution Scale by changing the deafult index value.", MessageType.Info);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("resolutionScaleList"), true);
						serializedObject.ApplyModifiedProperties();
						EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultIndex"));
						serializedObject.ApplyModifiedProperties();
					}
				}
				else if (waveXRSettings.adaptiveQualityMode == WaveXRSettings.AdaptiveQualityMode.Disabled)
				{
					EditorGUILayout.HelpBox("Adaptive Quality is disabled.\nTo use Dynamic Resolution, enable Adaptive Quality in WaveXRSettings.", MessageType.Warning);
				}
				else
				{
					EditorGUILayout.HelpBox("Adaptive Quality Mode is set to Quality/Performance Oriented Mode." +
											"\nDynamic Resolution is currently controlled by the mode preset." +
											"\nTo set scene specific settings, choose Customization Mode.", MessageType.Info);
				}
			}
		}
	}
#endif

	// The DynamicResolution will be triggered by AdaptiveQuality.  If AdaptiveQuality is not enabled, this feature will not work.
	[DisallowMultipleComponent]
	public class DynamicResolution : MonoBehaviour
	{
		[SerializeField]
		public bool overrideXRSettings = false;

		[Tooltip("The ResolutionScale help set a scaled resolution to be lower than the default.  An index will go higher or lower according to the AdaptiveQuality's event.  You can choose one of resolution scale from this list as a default resolution scale by setting the default index.")]
		[SerializeField]
		private List<float> resolutionScaleList = new List<float>();

		[Tooltip("You can choose one of resolution scale from this list as a default resolution scale by setting the default index.")]
		[SerializeField]
		private int defaultIndex = 0;
		private int index = 0;

		[Tooltip("The unit used for measuring text size here is dmm (Distance-Independent Millimeter). The method of conversion from Unity text size into dmm can be found in the documentation of the SDK.")]
		[SerializeField]
		[Range(20, 40)]
		private int textSize = 20;

		public float CurrentScale { get { return resolutionScaleList[index]; } }
		private float currentUpperBound = 1f;
		private float currentLowerBound = 0.1f;
		private bool isInitialized = false;
		private WaveXRSettings.AdaptiveQualityMode currentAdaptiveQualityMode;
		private const string LOG_TAG = "WaveEssenceDynRes";

		private const float qualityOrientedModeLowerBound = 1f;
		private const float performanceOrientedModeUpperBound = 1f;

		public static DynamicResolution instance;

		public enum AQEvent
		{
			None,
			ManualHigher,
			ManualLower,
			Higher,
			Lower,
		};

		public AQEvent CurrentAQEvent { get; private set; }

		private void Awake()
		{
			if (instance == null)
			{
				Log.d(LOG_TAG, "New DR Instance");
				instance = this;
			}
			else
			{
				if (instance != this) //Another instance of DR exists
				{
					Log.d(LOG_TAG, "Found another DR Instance");
					if (!overrideXRSettings) //Current instance uses global settings
					{
						Log.d(LOG_TAG, "Destroy this DR Instance");
						Destroy(this);
					}
					else //Current instance overrides global settings (higher priority)
					{
						Log.d(LOG_TAG, "Destroy other DR Instance and replace it");
						Destroy(instance);
						instance = this;
					}
				}
			}
		}

		void OnEnable()
		{
			WaveXRSettings waveXRSettings = WaveXRSettings.GetInstance();
			currentAdaptiveQualityMode = waveXRSettings.adaptiveQualityMode;

			Log.d(LOG_TAG, "currentAdaptiveQualityMode: " + currentAdaptiveQualityMode.ToString());
			if (currentAdaptiveQualityMode == WaveXRSettings.AdaptiveQualityMode.Disabled)
			{
				Log.e(LOG_TAG, "Destroying component as Dynamic Resolution is not enabled in WaveXRSettings.");
				instance = null;
				Destroy(this);
				return;
			}

			bool useGlobalSettings = currentAdaptiveQualityMode == WaveXRSettings.AdaptiveQualityMode.QualityOrientedMode || 
									 currentAdaptiveQualityMode == WaveXRSettings.AdaptiveQualityMode.PerformanceOrientedMode ||
									(currentAdaptiveQualityMode == WaveXRSettings.AdaptiveQualityMode.CustomizationMode && !overrideXRSettings);

			if (useGlobalSettings)
			{
				defaultIndex = waveXRSettings.DR_DefaultIndex;
				textSize = waveXRSettings.DR_TextSize;
				resolutionScaleList = waveXRSettings.DR_ResolutionScaleList;
			}

			if (resolutionScaleList.Count < 2)
			{
				Log.e(LOG_TAG, "Resolution Scale List contains less than 2 elements, Resolution Scale will not changed");
				return;
			}

			SystemEvent.Listen(HigherHandler);  // Listen to all event
			SystemEvent.Listen(WVR_EventType.WVR_EventType_RecommendedQuality_Lower, LowerHandler);
			index = defaultIndex;
			CurrentAQEvent = AQEvent.None;

			FormatResolutionScaleList();

			DefineUpperBound();
			DefineLowerBound();

			SetListUpperBound();
			SetListLowerBound();

			isInitialized = true;
			Log.d(LOG_TAG, "Finalilzed Resolution Scale List: " + string.Join(",", resolutionScaleList.ToArray()));
		}

		void OnDisable()
		{
			SystemEvent.Remove(HigherHandler);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_RecommendedQuality_Lower, LowerHandler);
			index = defaultIndex;

			SetResolutionScale(1);
		}

		private void SetResolutionScale(float rs)
		{
			XRSettings.eyeTextureResolutionScale = rs;
		}

		// Let the function can be access by script.
		public void Higher()
		{
			WVR_Event_t e = new WVR_Event_t();
			e.common.type = WVR_EventType.WVR_EventType_RecommendedQuality_Higher;
			HigherHandler(e);
			CurrentAQEvent = AQEvent.ManualHigher;
		}

		private void HigherHandler(WVR_Event_t systemEvent)
		{
			if (!isInitialized) return;
			if (systemEvent.common.type != WVR_EventType.WVR_EventType_RecommendedQuality_Higher)
				return;

			if (--index < 0)
				index = 0;

			SetResolutionScale(resolutionScaleList[index]);
			CurrentAQEvent = AQEvent.Higher;
			Log.d(LOG_TAG, "Event Higher: [" + index + "]=" + resolutionScaleList[index]);
		}

		// Let the function can be access by script.
		public void Lower()
		{
			WVR_Event_t e = new WVR_Event_t();
			LowerHandler(e);
			CurrentAQEvent = AQEvent.ManualLower;
		}

		private void LowerHandler(WVR_Event_t systemEvent)
		{
			if (!isInitialized) return;

			if (++index >= resolutionScaleList.Count)
				index = resolutionScaleList.Count - 1;

			SetResolutionScale(resolutionScaleList[index]);
			CurrentAQEvent = AQEvent.Lower;
			Log.d(LOG_TAG, "Event Lower: [" + index + "]=" + resolutionScaleList[index]);
		}

		// Set the scale back to default.
		public void ResetResolutionScale()
		{
			CurrentAQEvent = AQEvent.None;

			if (!enabled)
				return;
			index = defaultIndex;
			SetResolutionScale(resolutionScaleList[index]);
			Log.d(LOG_TAG, "Event Reset: [" + index + "]=" + resolutionScaleList[index]);
		}

		private void SetListUpperBound()
		{
			//Remove values that are higher than the defined upper bound
			while (resolutionScaleList.Count > 0 && resolutionScaleList[0] > currentUpperBound)
			{
				resolutionScaleList.RemoveAt(0);
			}
			resolutionScaleList.Insert(0, currentUpperBound);

			//Sort and reset index
			FormatResolutionScaleList();
			if (index > resolutionScaleList.Count - 1)
			{
				index = defaultIndex = resolutionScaleList.Count - 1;
			}

			SetResolutionScale(resolutionScaleList[index]);
		}

		private void SetListLowerBound()
		{
			//Remove values that are lower than the defined lower bound
			int counter = Math.Max(0, resolutionScaleList.Count - 1);
			while (counter > -1 && resolutionScaleList[counter] < currentLowerBound)
			{
				resolutionScaleList.RemoveAt(counter);
				counter--;
			}
			resolutionScaleList.Add(currentLowerBound);

			//Sort and reset index
			FormatResolutionScaleList();
			counter = Math.Max(0, counter);
			if (index > counter)
			{
				index = defaultIndex = counter;
			}

			SetResolutionScale(resolutionScaleList[index]);
		}

		private float GetResScaleFromDMM()
		{
#if UNITY_EDITOR
			return 0.1f;
#endif

			float P60D = 178.15f * (textSize * textSize) - 14419f * textSize + 356704f;

			Log.d(LOG_TAG, "Get P60D from DMM: " + P60D);
			float halfWidth = XRSettings.eyeTextureWidth / 2;
			float halfHeight = XRSettings.eyeTextureHeight / 2;
			float[] projection = GetProjection();

			float tan30 = Mathf.Tan(Mathf.Deg2Rad * 30f);

			float resolutionScale = Mathf.Sqrt(P60D / (Mathf.Pow(tan30, 2) * halfHeight * halfWidth * (1 / Mathf.Abs(projection[0]) + 1 / Mathf.Abs(projection[1])) * (1 / Mathf.Abs(projection[2]) + 1 / Mathf.Abs(projection[3]))));

			Log.d(LOG_TAG, "Eye Buffer Width: " + halfWidth + " Eye Buffer Height: " + halfHeight);
			//Log.d(LOG_TAG, "Projection: " + string.Join(", ", projection.Select(p => p.ToString()).ToArray()));
			Log.d(LOG_TAG, "Get Resolution Scale from P60D: " + resolutionScale);

			return resolutionScale;
		}

		private void DefineUpperBound()
		{
			switch (currentAdaptiveQualityMode)
			{
				case WaveXRSettings.AdaptiveQualityMode.QualityOrientedMode:
				case WaveXRSettings.AdaptiveQualityMode.CustomizationMode:
					currentUpperBound = Mathf.Min(Interop.WVR_GetResolutionMaxScale(), resolutionScaleList[0]);
					break;
				case WaveXRSettings.AdaptiveQualityMode.PerformanceOrientedMode:
					currentUpperBound = Mathf.Min(performanceOrientedModeUpperBound, resolutionScaleList[0]);
					break;
				case WaveXRSettings.AdaptiveQualityMode.Disabled:
				default:
					break;
			}
		}

		private void DefineLowerBound()
		{
			switch (currentAdaptiveQualityMode)
			{
				case WaveXRSettings.AdaptiveQualityMode.QualityOrientedMode:
					currentLowerBound = Mathf.Max(qualityOrientedModeLowerBound, resolutionScaleList[resolutionScaleList.Count - 1]);
					break;
				case WaveXRSettings.AdaptiveQualityMode.CustomizationMode:
				case WaveXRSettings.AdaptiveQualityMode.PerformanceOrientedMode:
					currentLowerBound = Mathf.Max(GetResScaleFromDMM(), resolutionScaleList[resolutionScaleList.Count - 1]);
					break;
				case WaveXRSettings.AdaptiveQualityMode.Disabled:
				default:
					break;
			}
		}

		private void FormatResolutionScaleList()
		{
			//Sort List
			FloatComparer floatComparer = new FloatComparer();
			resolutionScaleList.Sort(floatComparer);
			//Remove duplicate values
			resolutionScaleList = resolutionScaleList.Distinct().ToList();
		}

		private float[] GetProjection()
		{
			try
			{
				var displays = new List<XRDisplaySubsystem>();
				SubsystemManager.GetInstances(displays);

				float[] projLRaw = new float[4];
				if (displays.Count <= 0 || displays[0].GetRenderPassCount() <= 0 || Camera.main == null)
				{
					Log.d(LOG_TAG, "Using WVR_GetClippingPlaneBoundary for projection");
					Interop.WVR_GetClippingPlaneBoundary(WVR_Eye.WVR_Eye_Left, ref projLRaw[0], ref projLRaw[1], ref projLRaw[2], ref projLRaw[3]);
				}
				else
				{
					//Get first render parameter from display 0 (VR display)
					//Single pass: 1 pass with 2 render parameters
					//Multi pass: 2 pass with 1 render parameter each
					//First render parameter for both Stereo Rendering Paths

					displays[0].GetRenderPass(0, out XRDisplaySubsystem.XRRenderPass renderPasses);
					renderPasses.GetRenderParameter(Camera.main, 0, out XRDisplaySubsystem.XRRenderParameter renderParametersL);

					Matrix4x4 projL = renderParametersL.projection;
					Matrix4x4 inverseProjL = projL.inverse;

					projLRaw[0] = (Coordinate.MatrixMulVector(inverseProjL, new Vector4(-1, 0, 1, 1)).x);
					projLRaw[1] = (Coordinate.MatrixMulVector(inverseProjL, new Vector4(1, 0, 1, 1)).x);
					projLRaw[2] = (Coordinate.MatrixMulVector(inverseProjL, new Vector4(0, 1, 1, 1)).y);
					projLRaw[3] = (Coordinate.MatrixMulVector(inverseProjL, new Vector4(0, -1, 1, 1)).y);
				}
				Log.d(LOG_TAG, "projLRaw: " + string.Join(" ", projLRaw));
				return projLRaw;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return new float[4] { 0, 0, 0, 0 };
			}
		}

		void OnValidate()
		{
			ResetResolutionScaleListToValidState();
		}

		private void ResetResolutionScaleListToValidState()
		{
			if (resolutionScaleList.Count < 2)
			{
				resolutionScaleList.Add(0.1f);
				resolutionScaleList.Add(1);
			}

			defaultIndex = Mathf.Clamp(defaultIndex, 0, resolutionScaleList.Count - 1);
		}

		public float GetCurrentUpperBound()
		{
			return currentUpperBound;
		}

		public float GetCurrentLowerBound()
		{
			return currentLowerBound;
		}
	}

	class FloatComparer : IComparer<float>
	{
		public int Compare(float x, float y)
		{
			return y.CompareTo(x);
		}
	}
}
