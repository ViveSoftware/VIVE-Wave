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
using UnityEngine.UI;
using Wave.Native;
#if UNITY_EDITOR
using Wave.Essence.Editor;
#endif

namespace Wave.Essence.Samples.ButtonTest
{
	public class ButtonTest : MonoBehaviour
	{
		private const string LOG_TAG = "ButtonTest";
		private void DEBUG(string msg) { Log.d(LOG_TAG, this.DeviceType + " " + msg, true); }

		public XR_Device DeviceType = XR_Device.Dominant;

		public GameObject Button_Touch = null;
		private Text touch_text = null;
		public GameObject Button_Axis = null;
		private Text axis_text = null;
		private Vector2 v2axis = Vector2.zero;
		public GameObject Button_Press = null;
		private Text press_text = null;
		public GameObject DPad_Button_Touch = null;
		private Text dpad_touch_text = null;
		public GameObject DPad_Button_Axis = null;
		private Text dpad_axis_text = null;
		public GameObject DPad_Button_Press = null;
		private Text dpad_press_text = null;

		void Awake()
		{
			if (this.Button_Touch != null)
			{
				touch_text = this.Button_Touch.GetComponent<Text>();
				DEBUG("Start() Get Text of " + this.Button_Touch.name);
			}
			if (this.Button_Axis != null)
			{
				axis_text = this.Button_Axis.GetComponent<Text>();
				DEBUG("Start() Get Text of " + this.Button_Axis.name);
			}
			if (this.Button_Press != null)
			{
				press_text = this.Button_Press.GetComponent<Text>();
				DEBUG("Start() Get Text of " + this.Button_Press.name);
			}
			if (this.DPad_Button_Touch != null)
			{
				dpad_touch_text = this.DPad_Button_Touch.GetComponent<Text>();
				DEBUG("Start() Get Text of " + this.DPad_Button_Touch);
			}
			if (this.DPad_Button_Axis != null)
			{
				dpad_axis_text = this.DPad_Button_Axis.GetComponent<Text>();
				DEBUG("Start() Get Text of " + this.DPad_Button_Axis);
			}
			if (this.DPad_Button_Press != null)
			{
				dpad_press_text = this.DPad_Button_Press.GetComponent<Text>();
				DEBUG("Start() Get Text of " + this.DPad_Button_Press);
			}
		}

		void Update()
		{
			UpdateTouchText();
			UpdateAxisText();
			UpdatePressText();
			updateDPadTouchText();
			//updateDPadAxisText();
			updateDPadPressText();
		}


		private bool binaryValue = false;
		private bool preAXTouch = false, preBYTouch = false, preTouchpadTouch = false, preThumbstickTouch = false;
		private void UpdateTouchText()
		{
			if (touch_text == null)
				return;

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonTouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_A))
				{
					touch_text.text = "WVR_InputId_Alias1_A";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_A is touched.");
				}
				if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_A))
				{
					touch_text.text = "";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_A is untouched.");
				}
				if (WXRDevice.ButtonTouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_X))
				{
					touch_text.text = "WVR_InputId_Alias1_X";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_X is touched.");
				}
				if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_X))
				{
					touch_text.text = "";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_X is untouched.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.primaryTouch);
				if (preAXTouch != binaryValue)
				{
					preAXTouch = binaryValue;
					if (binaryValue)
					{
						touch_text.text = "primaryTouch";
						DEBUG("UpdateTouchText() primaryTouch is touched.");
					}
					else
					{
						touch_text.text = "";
						DEBUG("UpdateTouchText() primaryTouch is untouched.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonTouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_B))
				{
					touch_text.text = "WVR_InputId_Alias1_B";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_B is touched.");
				}
				if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_B))
				{
					touch_text.text = "";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_B is untouched.");
				}
				if (WXRDevice.ButtonTouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Y))
				{
					touch_text.text = "WVR_InputId_Alias1_Y";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_Y is touched.");
				}
				if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Y))
				{
					touch_text.text = "";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_Y is untouched.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.secondaryTouch);
				if (preBYTouch != binaryValue)
				{
					preBYTouch = binaryValue;
					if (binaryValue)
					{
						touch_text.text = "secondaryTouch";
						DEBUG("UpdateTouchText() secondaryTouch is touched.");
					}
					else
					{
						touch_text.text = "";
						DEBUG("UpdateTouchText() secondaryTouch is untouched.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonTouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Touchpad))
				{
					touch_text.text = "WVR_InputId_Alias1_Touchpad";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_Touchpad is touched.");
				}
				if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Touchpad))
				{
					touch_text.text = "";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_Touchpad is untouched.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.primary2DAxisTouch);
				if (preTouchpadTouch != binaryValue)
				{
					preTouchpadTouch = binaryValue;
					if (binaryValue)
					{
						touch_text.text = "primary2DAxisTouch";
						DEBUG("UpdateTouchText() primary2DAxisTouch is touched.");
					}
					else
					{
						touch_text.text = "";
						DEBUG("UpdateTouchText() primary2DAxisTouch is untouched.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonTouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Thumbstick))
				{
					touch_text.text = "WVR_InputId_Alias1_Thumbstick";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_Thumbstick is touched.");
				}
				if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Thumbstick))
				{
					touch_text.text = "";
					DEBUG("UpdateTouchText() WVR_InputId_Alias1_Thumbstick is untouched.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.secondary2DAxisTouch);
				if (preThumbstickTouch != binaryValue)
				{
					preThumbstickTouch = binaryValue;
					if (binaryValue)
					{
						touch_text.text = "secondary2DAxisTouch";
						DEBUG("UpdateTouchText() secondary2DAxisTouch is touched.");
					}
					else
					{
						touch_text.text = "";
						DEBUG("UpdateTouchText() secondary2DAxisTouch is untouched.");
					}
				}
			}

			if (WXRDevice.ButtonTouch((WVR_DeviceType)DeviceType, WVR_InputId.WVR_InputId_Alias1_Bumper))
			{
				touch_text.text = "WVR_InputId_Alias1_Bumper";
				DEBUG("UpdateTouchText() WVR_InputId_Alias1_Bumper is touched.");
			}
			if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Bumper))
			{
				touch_text.text = "";
				DEBUG("UpdateTouchText() WVR_InputId_Alias1_Bumper is untouched.");
			}
			if (WXRDevice.ButtonTouch((WVR_DeviceType)DeviceType, WVR_InputId.WVR_InputId_Alias1_Trigger))
			{
				touch_text.text = "WVR_InputId_Alias1_Trigger";
				DEBUG("UpdateTouchText() WVR_InputId_Alias1_Trigger is touched.");
			}
			if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Trigger))
			{
				touch_text.text = "";
				DEBUG("UpdateTouchText() WVR_InputId_Alias1_Trigger is untouched.");
			}

			if (WXRDevice.ButtonTouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Grip))
			{
				touch_text.text = "WVR_InputId_Alias1_Grip";
				DEBUG("UpdateTouchText() WVR_InputId_Alias1_Grip is touched.");
			}
			if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Grip))
			{
				touch_text.text = "";
				DEBUG("UpdateTouchText() WVR_InputId_Alias1_Grip is untouched.");
			}

			if (WXRDevice.ButtonTouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Parking))
			{
				touch_text.text = "WVR_InputId_Alias1_Parking";
				DEBUG("UpdateTouchText() WVR_InputId_Alias1_Parking is touched.");
			}
			if (WXRDevice.ButtonUntouch((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Parking))
			{
				touch_text.text = "";
				DEBUG("UpdateTouchText() WVR_InputId_Alias1_Parking is untouched.");
			}
		}

		private Vector2 axis2DValue = Vector2.zero;
		private float axisValue = 0;
		private void UpdateAxisText()
		{
			if (axis_text == null)
				return;

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonTouching((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Touchpad) ||
					WXRDevice.ButtonHold((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Touchpad))
				{
					axis2DValue = WXRDevice.ButtonAxis((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Touchpad);
					axis_text.text = axis2DValue.x.ToString() + ", " + axis2DValue.y.ToString();
				}
				else
				if (WXRDevice.ButtonTouching((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Thumbstick) ||
					WXRDevice.ButtonHold((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Thumbstick))
				{
					axis2DValue = WXRDevice.ButtonAxis((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Thumbstick);
					axis_text.text = axis2DValue.x.ToString() + ", " + axis2DValue.y.ToString();
				}
				else
				if (WXRDevice.ButtonTouching((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Trigger) ||
					WXRDevice.ButtonHold((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Trigger))
				{
					axis2DValue = WXRDevice.ButtonAxis((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Trigger);
					axis_text.text = axis2DValue.x.ToString();
				}
				else
				if (WXRDevice.ButtonTouching((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Grip) ||
					WXRDevice.ButtonHold((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Grip))
				{
					axis2DValue = WXRDevice.ButtonAxis((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Grip);
					axis_text.text = axis2DValue.x.ToString();
				}
				else
				{
					axis_text.text = "";
				}
			}
			else
#endif
			{
				axis2DValue = WXRDevice.KeyAxis2D(this.DeviceType, XR_Axis2DButton.primary2DAxis);
				if (!axis2DValue.Equals(Vector2.zero))
					axis_text.text = axis2DValue.x.ToString() + ", " + axis2DValue.y.ToString();

				axis2DValue = WXRDevice.KeyAxis2D(this.DeviceType, XR_Axis2DButton.secondary2DAxis);
				if (!axis2DValue.Equals(Vector2.zero))
					axis_text.text = axis2DValue.x.ToString() + ", " + axis2DValue.y.ToString();

				axisValue = WXRDevice.KeyAxis1D(this.DeviceType, XR_Axis1DButton.trigger);
				if (axisValue != 0)
					axis_text.text = axisValue.ToString();

				axisValue = WXRDevice.KeyAxis1D(this.DeviceType, XR_Axis1DButton.grip);
				if (axisValue != 0)
					axis_text.text = axisValue.ToString();
			}
		}

		private bool preMenuPress = false, preTouchpadPress = false, preTriggerPress = false, preGripPress = false, preThumbstickPress = false;
		private bool preAXPress = false, preBYPress = false;
		private void UpdatePressText()
		{
			if (press_text == null)
				return;

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Menu))
				{
					press_text.text = "WVR_InputId_Alias1_Menu";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Menu is pressed.");
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Menu))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Menu is unpressed.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.menuButton);
				if (preMenuPress != binaryValue)
				{
					preMenuPress = binaryValue;
					if (binaryValue)
					{
						press_text.text = "menuButton";
						DEBUG("UpdatePressText() menuButton is pressed.");
					}
					else
					{
						press_text.text = "";
						DEBUG("UpdatePressText() menuButton is unpressed.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Touchpad))
				{
					press_text.text = "WVR_InputId_Alias1_Touchpad";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Touchpad is pressed.");
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Touchpad))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Touchpad is unpressed.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.primary2DAxisClick);
				if (preTouchpadPress != binaryValue)
				{
					preTouchpadPress = binaryValue;
					if (binaryValue)
					{
						press_text.text = "primary2DAxisClick";
						DEBUG("UpdatePressText() primary2DAxisClick is pressed.");
					}
					else
					{
						press_text.text = "";
						DEBUG("UpdatePressText() primary2DAxisClick is unpressed.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Trigger))
				{
					press_text.text = "WVR_InputId_Alias1_Trigger";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Trigger is pressed.");
					TriggerVibration(this.DeviceType, 0.9f, 0.5f);
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Trigger))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Trigger is unpressed.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.triggerButton);
				if (preTriggerPress != binaryValue)
				{
					preTriggerPress = binaryValue;
					if (binaryValue)
					{
						press_text.text = "triggerButton";
						TriggerVibration(this.DeviceType, 0.9f, 0.5f);
						DEBUG("UpdatePressText() triggerButton is pressed.");
					}
					else
					{
						press_text.text = "";
						DEBUG("UpdatePressText() triggerButton is unpressed.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Grip))
				{
					press_text.text = "WVR_InputId_Alias1_Grip";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Grip is pressed.");
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Grip))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Grip is unpressed.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.gripButton);
				if (preGripPress != binaryValue)
				{
					preGripPress = binaryValue;
					if (binaryValue)
					{
						press_text.text = "gripButton";
						DEBUG("UpdatePressText() gripButton is pressed.");
					}
					else
					{
						press_text.text = "";
						DEBUG("UpdatePressText() gripButton is unpressed.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_A))
				{
					press_text.text = "WVR_InputId_Alias1_A";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_A is pressed.");
					TriggerVibration(this.DeviceType, 0.1f, 0.5f);
				}
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_X))
				{
					press_text.text = "WVR_InputId_Alias1_X";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_X is pressed.");
					TriggerVibration(this.DeviceType, 0.1f, 0.5f);
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_A))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_A is unpressed.");
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_X))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_X is unpressed.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.primaryButton);
				if (preAXPress != binaryValue)
				{
					preAXPress = binaryValue;
					if (binaryValue)
					{
						press_text.text = "primaryButton";
						TriggerVibration(this.DeviceType, 0.1f, 0.5f);
						DEBUG("UpdatePressText() primaryButton is pressed.");
					}
					else
					{
						press_text.text = "";
						DEBUG("UpdatePressText() primaryButton is unpressed.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_B))
				{
					press_text.text = "WVR_InputId_Alias1_B";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_B is pressed.");
					TriggerVibration(this.DeviceType, 0.5f, 0.5f);
				}
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Y))
				{
					press_text.text = "WVR_InputId_Alias1_Y";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Y is pressed.");
					TriggerVibration(this.DeviceType, 0.5f, 0.5f);
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_B))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_B is unpressed.");
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Y))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Y is unpressed.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.secondaryButton);
				if (preBYPress != binaryValue)
				{
					preBYPress = binaryValue;
					if (binaryValue)
					{
						press_text.text = "secondaryButton";
						TriggerVibration(this.DeviceType, 0.5f, 0.5f);
						DEBUG("UpdatePressText() secondaryButton is pressed.");
					}
					else
					{
						press_text.text = "";
						DEBUG("UpdatePressText() secondaryButton is unpressed.");
					}
				}
			}

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Thumbstick))
				{
					press_text.text = "WVR_InputId_Alias1_Thumbstick";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Thumbstick is pressed.");
				}
				if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Thumbstick))
				{
					press_text.text = "";
					DEBUG("UpdatePressText() WVR_InputId_Alias1_Thumbstick is unpressed.");
				}
			}
			else
#endif
			{
				binaryValue = WXRDevice.KeyDown(this.DeviceType, XR_BinaryButton.secondary2DAxisClick);
				if (preThumbstickPress != binaryValue)
				{
					preThumbstickPress = binaryValue;
					if (binaryValue)
					{
						press_text.text = "secondary2DAxisClick";
						DEBUG("UpdatePressText() secondary2DAxisClick is pressed.");
					}
					else
					{
						press_text.text = "";
						DEBUG("UpdatePressText() secondary2DAxisClick is unpressed.");
					}
				}
			}

			if (WXRDevice.ButtonPress((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Enter))
			{
				press_text.text = this.DeviceType + " Enter";
				DEBUG("UpdatePressText() Enter is pressed.");
			}
			if (WXRDevice.ButtonRelease((WVR_DeviceType)this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Enter))
			{
				press_text.text = "";
				DEBUG("UpdatePressText() Enter is unpressed.");
			}
		}

		private void updateDPadTouchText()
		{
			if (dpad_touch_text == null)
				return;
		}

		private void updateDPadPressText()
		{
			if (dpad_press_text == null)
				return;
		}

		/// <summary>
		/// Trigger vibration on a device.
		/// </summary>
		/// <param name="device">The vibration device.</param>
		/// <param name="amplitude">[0, 1] The intensity of vibration.</param>
		/// <param name="duration">The vibration duration in seconds. Default 1 second.</param>
		public void TriggerVibration(XR_Device device, float amplitude, float duration = 1)
		{
			DEBUG("TriggerVibration() " + device + ", amplitude: " + amplitude + ", duration: " + duration);
			WXRDevice.SendHapticImpulse(device, amplitude, duration);
#if UNITY_EDITOR
			if (device == XR_Device.Right)
				Interop.WVR_TriggerVibration(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Touchpad, (uint)(duration * 1000000), 1, WVR_Intensity.WVR_Intensity_Strong);
			if (device == XR_Device.Left)
				Interop.WVR_TriggerVibration(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Touchpad, (uint)(duration * 1000000), 1, WVR_Intensity.WVR_Intensity_Strong);
#endif
		}
	}
}
