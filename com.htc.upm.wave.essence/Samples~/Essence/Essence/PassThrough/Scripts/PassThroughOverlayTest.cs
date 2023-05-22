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

namespace Wave.Essence.Samples.PassThrough
{
	public class PassThroughOverlayTest : MonoBehaviour
	{
		private static string LOG_TAG = "Wave.Essence.Samples.PassThrough.PassThroughOverlayTest";

		private bool passThroughOverlayFlag = false;
		private bool showPassThroughOverlay = false;
		bool delaySubmit = false;
		bool showIndicator = false;
		float alpha = 1.0f;
		float alpha2 = 1.0f;
		int steps = 0;
		// Start is called before the first frame update
		void Start()
		{
			Log.i(LOG_TAG, "PassThroughOverlay start: " + passThroughOverlayFlag);
			showPassThroughOverlay = Interop.WVR_ShowPassthroughOverlay(passThroughOverlayFlag);
			Interop.WVR_ShowProjectedPassthrough(false);
			Log.i(LOG_TAG, "ShowPassThroughOverlay start: " + showPassThroughOverlay);
		}

		// Update is called once per frame
		void Update()
		{
			if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A))
			{
				bool visible = !Interop.WVR_IsPassthroughOverlayVisible();
				if (visible)
				{
					if (steps == 0)
					{
						delaySubmit = false;
						showIndicator = false;
					}
					else if (steps == 1)
					{
						delaySubmit = true;
						showIndicator = false;
					}
					else if (steps == 2)
					{
						delaySubmit = false;
						showIndicator = true;
					}
					else if (steps == 3)
					{
						delaySubmit = true;
						showIndicator = true;
					}
					Interop.WVR_ShowPassthroughOverlay(visible, delaySubmit, showIndicator);
					Log.i(LOG_TAG, "WVR_ShowPassthroughOverlay: visible:" + visible + " ,delaySubmit: " + delaySubmit + " ,showIndicator: " + showIndicator);
					alpha = 1.0f;
					Interop.WVR_SetPassthroughOverlayAlpha(alpha);
			    }
				else
				{
					Interop.WVR_ShowPassthroughOverlay(visible);
					steps++;
					if (steps >= 4)
					{
						steps = 0;
					}
				}
			}
			else if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_X))
			{
				bool visible = !Interop.WVR_IsPassthroughOverlayVisible();
				if (visible)
				{
					alpha2 = 1.0f;
					WVR_Pose_t pose = new WVR_Pose_t();
					pose.position.v0 = 0.0f;
					pose.position.v1 = 0.0f;
					pose.position.v2 = -2.0f;
					pose.rotation.w = 1.0f;
					pose.rotation.x = 0.0f;
					pose.rotation.y = 0.0f;
					pose.rotation.z = 0.0f;
					Interop.WVR_SetProjectedPassthroughPose(ref pose);

					float size = 0.25f;
					float[] vertex = { -size, -size, 0.0f,
							  size, -size, 0.0f,
							  size, size, 0.0f,
							  -size, size, 0.0f };
					uint[] indices = { 0, 1, 2, 0, 2, 3 };
					Interop.WVR_SetProjectedPassthroughMesh(vertex, (uint)vertex.Length, indices, (uint)indices.Length);
					Interop.WVR_SetProjectedPassthroughAlpha(alpha2);
					Interop.WVR_ShowProjectedPassthrough(visible);
					Log.i(LOG_TAG, "WVR_ShowProjectedPassthrough: " + alpha2);
				}
				else
				{
					Interop.WVR_ShowProjectedPassthrough(visible);
				}
			}
			else if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Y))
			{
				alpha2 -= 0.1f;
				if (alpha2 < 0.0f)
				{
					alpha2 = 1.0f;
				}
				Interop.WVR_SetProjectedPassthroughAlpha(alpha2);
				Log.i(LOG_TAG, "WVR_SetProjectedPassthroughAlpha: " + alpha2);
			}
			else if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_B))
			{
				alpha -= 0.1f;
				if (alpha < 0.0f)
				{
					alpha = 1.0f;
				}
				Interop.WVR_SetPassthroughOverlayAlpha(alpha);
				Log.i(LOG_TAG, "SetPassthroughOverlayAlpha: " + alpha);
			}
		}

	private void OnApplicationPause()
		{
			showPassThroughOverlay = Interop.WVR_ShowPassthroughOverlay(false);
			Interop.WVR_ShowProjectedPassthrough(false);
			Log.i(LOG_TAG, "ShowPassThroughOverlay Pause: " + showPassThroughOverlay);
		}

		private void OnApplicationQuit()
		{
			showPassThroughOverlay = Interop.WVR_ShowPassthroughOverlay(false);
			Interop.WVR_ShowProjectedPassthrough(false);
			Log.i(LOG_TAG, "ShowPassThroughOverlay Quit: " + showPassThroughOverlay);
		}
	}
}
