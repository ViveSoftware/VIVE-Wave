using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample
{
	/// <summary> This class is implemented for wrapping the UnityEngine.Input and com.unity.inputsystem. </summary>
	public static class WXRInput
	{
		public static bool GetMouseButton(int button)
		{
			bool held = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			held = UnityEngine.Input.GetMouseButton(button);
#elif ENABLE_INPUT_SYSTEM
			if (button == 0)
				held = Mouse.current.leftButton.isPressed;
			if (button == 1)
				held = Mouse.current.rightButton.isPressed;
#endif
			return held;
		}
		public static bool GetMouseButtonDown(int button)
		{
			bool pressed = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			pressed = UnityEngine.Input.GetMouseButtonDown(button);
#elif ENABLE_INPUT_SYSTEM
			if (button == 0)
				pressed = Mouse.current.leftButton.wasPressedThisFrame;
			if (button == 1)
				pressed = Mouse.current.rightButton.wasPressedThisFrame;
#endif
			return pressed;
		}
		public static bool GetMouseButtonUp(int button)
		{
			bool releases = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			releases = UnityEngine.Input.GetMouseButtonUp(button);
#elif ENABLE_INPUT_SYSTEM
			if (button == 0)
				releases = Mouse.current.leftButton.wasReleasedThisFrame;
			if (button == 1)
				releases = Mouse.current.rightButton.wasReleasedThisFrame;
#endif
			return releases;
		}

#if ENABLE_INPUT_SYSTEM
		private static UnityEngine.InputSystem.Controls.KeyControl KeyboardKey(KeyCode key)
		{
			switch (key)
			{
				case KeyCode.A: return Keyboard.current.aKey;
				case KeyCode.B: return Keyboard.current.bKey;
				case KeyCode.C: return Keyboard.current.cKey;
				case KeyCode.D: return Keyboard.current.dKey;
				case KeyCode.E: return Keyboard.current.eKey;
				case KeyCode.F: return Keyboard.current.fKey;
				case KeyCode.G: return Keyboard.current.gKey;
				case KeyCode.H: return Keyboard.current.hKey;
				case KeyCode.I: return Keyboard.current.iKey;
				case KeyCode.J: return Keyboard.current.jKey;
				case KeyCode.K: return Keyboard.current.kKey;
				case KeyCode.L: return Keyboard.current.lKey;
				case KeyCode.M: return Keyboard.current.mKey;
				case KeyCode.N: return Keyboard.current.nKey;
				case KeyCode.O: return Keyboard.current.oKey;
				case KeyCode.P: return Keyboard.current.pKey;
				case KeyCode.Q: return Keyboard.current.qKey;
				case KeyCode.R: return Keyboard.current.rKey;
				case KeyCode.S: return Keyboard.current.sKey;
				case KeyCode.T: return Keyboard.current.tKey;
				case KeyCode.U: return Keyboard.current.uKey;
				case KeyCode.V: return Keyboard.current.vKey;
				case KeyCode.W: return Keyboard.current.wKey;
				case KeyCode.X: return Keyboard.current.xKey;
				case KeyCode.Y: return Keyboard.current.yKey;
				case KeyCode.Z: return Keyboard.current.zKey;
				default:
					break;
			}
			return Keyboard.current.leftCtrlKey;
		}
		private static UnityEngine.InputSystem.Controls.KeyControl KeyboardKey(string keyName)
		{
			if (keyName.Equals("a")) return Keyboard.current.aKey;
			if (keyName.Equals("b")) return Keyboard.current.bKey;
			if (keyName.Equals("c")) return Keyboard.current.cKey;
			if (keyName.Equals("d")) return Keyboard.current.dKey;
			if (keyName.Equals("e")) return Keyboard.current.eKey;
			if (keyName.Equals("f")) return Keyboard.current.fKey;
			if (keyName.Equals("g")) return Keyboard.current.gKey;
			if (keyName.Equals("h")) return Keyboard.current.hKey;
			if (keyName.Equals("i")) return Keyboard.current.iKey;
			if (keyName.Equals("j")) return Keyboard.current.jKey;
			if (keyName.Equals("k")) return Keyboard.current.kKey;
			if (keyName.Equals("l")) return Keyboard.current.lKey;
			if (keyName.Equals("m")) return Keyboard.current.mKey;
			if (keyName.Equals("n")) return Keyboard.current.nKey;
			if (keyName.Equals("o")) return Keyboard.current.oKey;
			if (keyName.Equals("p")) return Keyboard.current.pKey;
			if (keyName.Equals("q")) return Keyboard.current.qKey;
			if (keyName.Equals("r")) return Keyboard.current.rKey;
			if (keyName.Equals("s")) return Keyboard.current.sKey;
			if (keyName.Equals("t")) return Keyboard.current.tKey;
			if (keyName.Equals("u")) return Keyboard.current.uKey;
			if (keyName.Equals("v")) return Keyboard.current.vKey;
			if (keyName.Equals("w")) return Keyboard.current.wKey;
			if (keyName.Equals("x")) return Keyboard.current.xKey;
			if (keyName.Equals("y")) return Keyboard.current.yKey;
			if (keyName.Equals("z")) return Keyboard.current.zKey;

			return Keyboard.current.leftCtrlKey;
		}
#endif
		public static bool GetKey(KeyCode key)
		{
			bool held = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			held = UnityEngine.Input.GetKey(key);
#elif ENABLE_INPUT_SYSTEM
			held = KeyboardKey(key).isPressed;
#endif
			return held;
		}
		public static bool GetKeyDown(KeyCode key)
		{
			bool pressed = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			pressed = UnityEngine.Input.GetKeyDown(key);
#elif ENABLE_INPUT_SYSTEM
			pressed = KeyboardKey(key).wasPressedThisFrame;
#endif
			return pressed;
		}
		public static bool GetKeyUp(KeyCode key)
		{
			bool releases = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			releases = UnityEngine.Input.GetKeyUp(key);
#elif ENABLE_INPUT_SYSTEM
			releases = KeyboardKey(key).wasReleasedThisFrame;
#endif
			return releases;
		}

		public static bool GetKey(string keyName)
		{
			bool held = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			held = UnityEngine.Input.GetKey(keyName);
#elif ENABLE_INPUT_SYSTEM
			held = KeyboardKey(keyName).isPressed;
#endif
			return held;
		}
		public static bool GetKeyDown(string keyName)
		{
			bool pressed = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			pressed = UnityEngine.Input.GetKeyDown(keyName);
#elif ENABLE_INPUT_SYSTEM
			pressed = KeyboardKey(keyName).wasPressedThisFrame;
#endif
			return pressed;
		}
		public static bool GetKeyUp(string keyName)
		{
			bool releases = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			releases = UnityEngine.Input.GetKeyUp(keyName);
#elif ENABLE_INPUT_SYSTEM
			releases = KeyboardKey(keyName).wasReleasedThisFrame;
#endif
			return releases;
		}

#if ENABLE_INPUT_SYSTEM
		private static Dictionary<string, bool> s_JoystickButtons = new Dictionary<string, bool>()
		{
			{ "joystick button 0", false },
			{ "joystick button 1", false },
			{ "joystick button 2", false },
			{ "joystick button 3", false },
			{ "joystick button 4", false },
			{ "joystick button 5", false },
			{ "joystick button 6", false },
			{ "joystick button 7", false },
			{ "joystick button 8", false },
			{ "joystick button 9", false },
			{ "joystick button 10", false },
			{ "joystick button 11", false },
			{ "joystick button 12", false },
			{ "joystick button 13", false },
			{ "joystick button 14", false },
			{ "joystick button 15", false }
		};
		private static bool GetButtonStates(string buttonName)
		{
			bool held = false;

			if (buttonName.Equals("joystick button 0"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 1"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 2"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 3"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 4"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 5"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 6"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 7"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 8"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 9"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 14"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool value))
					held = value;
			}
			if (buttonName.Equals("joystick button 15"))
			{
				if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool value))
					held = value;
			}

			return held;
		}
#endif
		public static bool GetButton(string buttonName)
		{
			bool held = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			held = UnityEngine.Input.GetButton(buttonName);
#elif ENABLE_INPUT_SYSTEM
			if (s_JoystickButtons.TryGetValue(buttonName, out bool value))
			{
				held = GetButtonStates(buttonName);
				s_JoystickButtons[buttonName] = held;
			}
#endif
			return held;
		}
		public static bool GetButtonDown(string buttonName)
		{
			bool pressed = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			pressed = UnityEngine.Input.GetButtonDown(buttonName);
#elif ENABLE_INPUT_SYSTEM
			if (s_JoystickButtons.TryGetValue(buttonName, out bool value))
			{
				bool held = GetButtonStates(buttonName);
				if (!value && held)
					pressed = true;
				s_JoystickButtons[buttonName] = held;
			}
#endif
			return pressed;
		}
		public static bool GetButtonUp(string buttonName)
		{
			bool released = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			released = UnityEngine.Input.GetButtonUp(buttonName);
#elif ENABLE_INPUT_SYSTEM
			if (s_JoystickButtons.TryGetValue(buttonName, out bool value))
			{
				bool held = GetButtonStates(buttonName);
				if (value && !held)
					released = true;
				s_JoystickButtons[buttonName] = held;
			}
#endif
			return released;
		}
	}
}
