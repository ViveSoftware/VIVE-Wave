using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Wave.XR.Sample.Input
{
	public enum InputUsage {
		IsTracked = 1 << 0,
		Velocity = 1 << 1,
		AngularVelocity = 1 << 2,
		Position = 1 << 3,
		Rotation = 1 << 4,
		PrimaryButton = 1 << 5,
		SecondaryButton = 1 << 6,
		TriggerButton = 1 << 7,
		GripButton = 1 << 8,
		Primary2DAxis = 1 << 9,
		Primary2DButton = 1 << 10,
		MenuButton = 1 << 11,
		HandData = 1 << 12,
		All = InputUsage.IsTracked | InputUsage.Velocity | InputUsage.AngularVelocity | InputUsage.Position | InputUsage.Rotation | InputUsage.PrimaryButton | InputUsage.SecondaryButton | InputUsage.TriggerButton | InputUsage.GripButton | InputUsage.Primary2DAxis | InputUsage.Primary2DButton | InputUsage.MenuButton | InputUsage.HandData
	}

	public class XRInputDevice
	{
		public InputDevice dev;
		public int frameCount = 0;  // The data is update at which frame
		public InputUsage usages = InputUsage.All;
	}

	public class XRTrackingDevice : XRInputDevice
	{
		public readonly Button isTracked = new Button();
		public Vector3 velocity;  // tracking space
		public Vector3 angularVelocity;  // device space???
		public Vector3 position;  // tracking space
		public Quaternion rotation;  // tracking space
	}

	public class Controller : XRTrackingDevice
	{
		public bool isLeft;
		public readonly Button btnPri = new Button();
		public readonly Button btnSec = new Button();
		public readonly Button btnTrigger = new Button();
		public readonly Button btnGrip = new Button();
		public readonly Axis2D axisJoy = new Axis2D();
		public readonly Button btnJoy = new Button();
		public readonly Button btnMenu = new Button();

		public Controller(bool isLeft) { this.isLeft = isLeft; }
		public Controller(bool isLeft, InputUsage usages) { this.isLeft = isLeft; this.usages = usages; }
	}

	public class Hand : XRTrackingDevice
	{
		public bool isLeft;
		public readonly Button pinch = new Button();
		public UnityEngine.XR.Hand handData;

		public Hand(bool isLeft) { this.isLeft = isLeft; }
		public Hand(bool isLeft, InputUsage usages) { this.isLeft = isLeft; this.usages = usages; }
	}

	public class Tracker : XRTrackingDevice
	{
		public TrackerID trackerId;

		public Tracker(TrackerID id, InputUsage usages) { trackerId = id; this.usages = usages; }

		public enum TrackerID
		{
			Tracker0,
			Tracker1,
			Tracker2,
			Tracker3,
			Tracker4,
			Tracker5,
			Tracker6,
			Tracker7,
		}
	}

	public class HMD : XRTrackingDevice
	{
		public HMD(InputUsage usages) { this.usages = usages; }
		public HMD() { }
	}

	public class Button
	{
		float timeDown = 0f;

		public bool IsPressed { get; private set; } = false;
		public bool IsChanged { get; private set; } = false;
		public bool IsDown { get; private set; } = false;
		public bool IsUp { get; private set; } = false;

		// Do once in a frame
		public void Set(bool pressed)
		{
			IsChanged = IsPressed != pressed;
			IsPressed = pressed;
			IsDown = IsChanged && pressed;
			IsUp = IsChanged && !pressed;

			if (IsDown)
			{
				timeDown = Time.unscaledTime;
			}
		}

		public bool IsLongPressed(float threshold = 0.5f)
		{
			return IsPressed && (Time.unscaledTime - timeDown) > threshold;
		}
	}

	public class Axis1D
	{
		public float Value { get; private set; }

			// Do once in a frame
		public void Set(float value)
		{
			this.Value = value;
		}
	}

	public class Axis2D
	{
		public Button Left { get; private set; } = new Button();
		public Button Right { get; private set; } = new Button();
		public Button Neutral { get; private set; } = new Button();
		public Button Up { get; private set; } = new Button();
		public Button Down { get; private set; } = new Button();
		public bool IsChanged
		{
			get
			{
				return Left.IsChanged || Right.IsChanged || Neutral.IsChanged || Up.IsChanged || Down.IsChanged;
			}
		}

		//public bool IsDirChanged { get; private set; } = false;
		//public bool HasCombine { get; private set; } = false;
		public float Threshold { get; private set; } = 0.5f;
		public float HysteresisValue { get; private set; } = 0.2f;

		public Vector2 Value { get; private set; }

		//public JoyDir Dir { get; private set; } = JoyDir.N;
		//private JoyDir oldDir = JoyDir.N;

		//public enum JoyDir {
		//    L, R, U, D, N
		//}

		// Do once in a frame
		public void Set(Vector2 value)
		{
			Value = value;

			var t1 = Threshold;
			var t2 = Threshold - HysteresisValue;
			Left.Set( (Value.x < -t1) || ((Value.x < -t2) && Left.IsPressed));
			Right.Set((Value.x >  t1) || ((Value.x >  t2) && Right.IsPressed));
			Up.Set(   (Value.y >  t1) || ((Value.y >  t2) && Up.IsPressed));
			Down.Set( (Value.y < -t1) || ((Value.y < -t2) && Down.IsPressed));

			//HasCombine = (IsLeft && IsUp) || (IsLeft && IsDown) || (IsRight && IsUp) || (IsRight && IsDown);

			Neutral.Set(!(Left.IsPressed || Right.IsPressed || Up.IsPressed || Down.IsPressed));
		}
	}

	public class InputDeviceTools
	{
		public static bool GetInputDevice(XRInputDevice dev, List<InputDevice> inputDevices, InputDeviceCharacteristics chars)
		{
			if (dev.dev.isValid) return true;
			// Not check again in the same frame
			if (dev.frameCount == Time.frameCount) return false;
			dev.frameCount = Time.frameCount;

			if (inputDevices == null) inputDevices = new List<InputDevice>();
			if (!dev.dev.isValid)
			{
				InputDevices.GetDevicesWithCharacteristics(chars, inputDevices);
            
				if (inputDevices.Count > 0)
					dev.dev = inputDevices[0];
			}
			inputDevices.Clear();
			return dev.dev.isValid;
		}

		public static bool GetController(Controller dev, List<InputDevice> inputDevices)
		{
			InputDeviceCharacteristics chars;
			if (dev.isLeft)
				chars = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
			else
				chars = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
			return GetInputDevice(dev, inputDevices, chars);
		}

		public static bool GetHand(Hand dev, List<InputDevice> inputDevices)
		{
			InputDeviceCharacteristics chars;
			if (dev.isLeft)
				chars = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.HandTracking;
			else
				chars = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.HandTracking;
			return GetInputDevice(dev, inputDevices, chars);
		}

		public static bool GetTracker(Tracker dev, List<InputDevice> inputDevices)
		{
			if (dev.dev.isValid) return true;
			// Not check again in the same frame
			if (dev.frameCount == Time.frameCount) return false;
			dev.frameCount = Time.frameCount;
			InputDeviceCharacteristics chars = InputDeviceCharacteristics.TrackedDevice;

			if (inputDevices == null) inputDevices = new List<InputDevice>();
			InputDevices.GetDevicesWithCharacteristics(chars, inputDevices);

			//InputDeviceTracker.TrackerId trackerId = (InputDeviceTracker.TrackerId)dev.trackerId;
			for (int i = 0; i < inputDevices.Count; i++)
			{
				try
				{
					if (inputDevices[i].serialNumber.Contains("Tracker"))
					{
						dev.dev = inputDevices[i];
						Debug.Log($"Found Tracker {inputDevices[i].serialNumber}");
						break;
					}
				}
				catch (System.Exception) { }
			}
			inputDevices.Clear();
			return dev.dev.isValid;
		}

		public static bool GetHMD(HMD dev, List<InputDevice> inputDevices)
		{
			InputDeviceCharacteristics chars;
			chars = InputDeviceCharacteristics.HeadMounted;
			return GetInputDevice(dev, inputDevices, chars);
		}

		public static bool HasUsage(InputUsage usages, InputUsage target)
		{
			return (usages & target) == target;
		}

		public static void UpdateXRTrackingDevice(XRTrackingDevice dev, InputUsage usages = InputUsage.All)
		{
			if (HasUsage(usages, InputUsage.IsTracked))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked);
				dev.isTracked.Set(isTracked);
			}
			if (HasUsage(usages, InputUsage.Velocity))
				dev.dev.TryGetFeatureValue(CommonUsages.deviceVelocity, out dev.velocity);
			if (HasUsage(usages, InputUsage.AngularVelocity))
				dev.dev.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out dev.angularVelocity);
			if (HasUsage(usages, InputUsage.Position))
				dev.dev.TryGetFeatureValue(CommonUsages.devicePosition, out dev.position);
			if (HasUsage(usages, InputUsage.Rotation))
				dev.dev.TryGetFeatureValue(CommonUsages.deviceRotation, out dev.rotation);

			dev.frameCount = Time.frameCount;
		}


		public static void UpdateHMD(HMD dev, InputUsage usages)
		{
			if (!dev.dev.isValid)
				return;

			if (HasUsage(usages, InputUsage.Position))
				dev.dev.TryGetFeatureValue(CommonUsages.centerEyePosition, out dev.position);
			if (HasUsage(usages, InputUsage.Rotation))
				dev.dev.TryGetFeatureValue(CommonUsages.centerEyeRotation, out dev.rotation);

			dev.frameCount = Time.frameCount;
		}

		public static void UpdateHMD(HMD dev)
		{
			UpdateHMD(dev, dev.usages);
		}

		public static void UpdateController(Controller dev, InputUsage usages)
		{
			if (!dev.dev.isValid)
				return;
			UpdateXRTrackingDevice(dev, usages);

			if (HasUsage(usages, InputUsage.PrimaryButton))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.primaryButton, out bool btn);
				dev.btnPri.Set(btn);
			}
			if (HasUsage(usages, InputUsage.SecondaryButton))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.secondaryButton, out bool btn);
				dev.btnSec.Set(btn);
			}
			if (HasUsage(usages, InputUsage.TriggerButton))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.triggerButton, out bool btn);
				dev.btnTrigger.Set(btn);
			}
			if (HasUsage(usages, InputUsage.GripButton))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.gripButton, out bool btn);
				dev.btnGrip.Set(btn);
			}
			if (HasUsage(usages, InputUsage.Primary2DAxis))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis);
				dev.axisJoy.Set(axis);
			}
			if (HasUsage(usages, InputUsage.Primary2DButton))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool btn);
				dev.btnJoy.Set(btn);
			}
			if (HasUsage(usages, InputUsage.MenuButton))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.menuButton, out bool btn);
				dev.btnMenu.Set(btn);
			}


			dev.frameCount = Time.frameCount;
		}

		public static void UpdateController(Controller dev)
		{
			UpdateController(dev, dev.usages);
		}

		public static void UpdateHand(Hand dev, InputUsage usages)
		{
			if (!dev.dev.isValid)
				return;
			UpdateXRTrackingDevice(dev, usages);

			if (HasUsage(usages, InputUsage.HandData))
			{
				dev.dev.TryGetFeatureValue(CommonUsages.handData, out dev.handData);
			}

			dev.frameCount = Time.frameCount;
		}

		public static void UpdateHand(Hand dev)
		{
			UpdateHand(dev, dev.usages);
		}
	}
}
