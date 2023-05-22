using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wave.Essence;
using Wave.Native;
using Wave.XR.Settings;

public class FSETest_Manager : MonoBehaviour
{
	public Text FSEEnabledText, FSELevelText;
	public Canvas FSECanvas;

	private bool isFSEEnabledInRenderInit = false;
	private float currentFSELevel = 0f;
	private WaveXRSettings settingsInstance = null;

	private float analogDetectionThreshold = 0.7f;
	private float furthestDistance = 7f;
	private float nearestDistance = 1f;
	private float maxFSELevel = 1f;
	private float minFSELevel = 0f;
	private float FSELevelStep = 0.1f;

	// Start is called before the first frame update
	void Start()
    {
        if (settingsInstance == null)
		{
			settingsInstance = WaveXRSettings.GetInstance();
		}

		isFSEEnabledInRenderInit = settingsInstance.enableFSE;

		if (isFSEEnabledInRenderInit)
		{
			currentFSELevel = settingsInstance.FSE_Level;
			Interop.WVR_SetFrameSharpnessEnhancementLevel(currentFSELevel);
		}

		if (FSEEnabledText != null)
		{
			FSEEnabledText.text = isFSEEnabledInRenderInit.ToString();
		}

		if (FSELevelText != null)
		{
			FSELevelText.text = currentFSELevel.ToString();
		}

	}
	private void Update()
	{
		PanelTranslation();
	}

	public void IncreaseFSELevel()
	{
		if (!isFSEEnabledInRenderInit) return;

		currentFSELevel = Mathf.Clamp(currentFSELevel + FSELevelStep, minFSELevel, maxFSELevel);
		Interop.WVR_SetFrameSharpnessEnhancementLevel(currentFSELevel);
		if (FSELevelText != null)
		{
			FSELevelText.text = currentFSELevel.ToString();
		}
	}

	public void DecreaseFSELevel()
	{
		if (!isFSEEnabledInRenderInit) return;

		currentFSELevel = Mathf.Clamp(currentFSELevel - FSELevelStep, minFSELevel, maxFSELevel);
		Interop.WVR_SetFrameSharpnessEnhancementLevel(currentFSELevel);
		if (FSELevelText != null)
		{
			FSELevelText.text = currentFSELevel.ToString();
		}
	}

	public void SetFSELevel(float level)
	{
		if (!isFSEEnabledInRenderInit) return;

		currentFSELevel = Mathf.Clamp(level, minFSELevel, maxFSELevel);
		Interop.WVR_SetFrameSharpnessEnhancementLevel(currentFSELevel);
		if (FSELevelText != null)
		{
			FSELevelText.text = currentFSELevel.ToString();
		}
	}

	void PanelTranslation()
	{
		float L_TS_Y_State = WXRDevice.ButtonAxis(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Thumbstick).y;
		float R_TS_Y_State = WXRDevice.ButtonAxis(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Thumbstick).y;
		float L_TP_Y_State = WXRDevice.ButtonAxis(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Touchpad).y;
		float R_TP_Y_State = WXRDevice.ButtonAxis(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Touchpad).y;

		//Log.d(LOG_TAG, "L_TS_Y_State: " + L_TS_Y_State + " R_TS_Y_State: " + R_TS_Y_State + " L_TP_Y_State: " + L_TP_Y_State + " R_TP_Y_State: " + R_TP_Y_State);

		if (L_TS_Y_State > 0.1f ||
			R_TS_Y_State > 0.1f ||
			L_TP_Y_State > 0.1f ||
			R_TP_Y_State > 0.1f)
		{
			//Log.d(LOG_TAG, "Button Axis: Y Positive");
			if (FSECanvas.transform.position.z < furthestDistance)
			{
				//Log.d(LOG_TAG, "Layer translation: Further");
				Vector3 targetLayerPosition = new Vector3(FSECanvas.transform.position.x, FSECanvas.transform.position.y, Mathf.Min(FSECanvas.transform.position.z + 0.025f, furthestDistance));
				FSECanvas.transform.position = targetLayerPosition;
			}
		}
		else if (L_TS_Y_State < -analogDetectionThreshold ||
				 R_TS_Y_State < -analogDetectionThreshold ||
				 L_TP_Y_State < -analogDetectionThreshold ||
				 R_TP_Y_State < -analogDetectionThreshold)
		{
			//Log.d(LOG_TAG, "Button Axis: Y Negative");
			if (FSECanvas.transform.position.z > nearestDistance)
			{
				//Log.d(LOG_TAG, "Layer translation: Nearer");
				Vector3 targetLayerPosition = new Vector3(FSECanvas.transform.position.x, FSECanvas.transform.position.y, Mathf.Max(FSECanvas.transform.position.z - 0.025f, nearestDistance));
				FSECanvas.transform.position = targetLayerPosition;
			}
		}
	}
}
