using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Wave.XR.Sample;

public class RenderMaskTest : MonoBehaviour
{
	float theta = 0;
	public const float cycleDuration = 3;
	private TrackingOriginModeFlags originModeFlag;  // original
	public Text text;

	void OnEnable()
	{
		originModeFlag = Utils.InputSubsystem.GetTrackingOriginMode();
		Utils.InputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
	}

	private void Start()
	{
#if false
		// TODO Settings should not available in a cross-platform sample
		bool result = false;
		Constants.ErrorCode success = SettingsHelper.GetBool(SettingsHelper.NameUseRenderMask, ref result);
		text.text = "Get status of RenderMask successfuly? " + success + "\n\tRenderMask is enabled? " + result;
#endif
	}

	// Update is called once per frame
	void Update()
    {
		theta += 2 * Mathf.PI / cycleDuration * Time.deltaTime;
		if (theta > Mathf.PI * 2)
		{
			theta -= Mathf.PI * 2;
		}

		XRSettings.occlusionMaskScale = 0.5f + Mathf.Cos(theta) * 0.5f;
	}

	private void OnDisable()
	{
		Utils.InputSubsystem.TrySetTrackingOriginMode(originModeFlag);
		XRSettings.occlusionMaskScale = 1;
	}
}
