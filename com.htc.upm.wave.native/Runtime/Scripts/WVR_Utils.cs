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
using UnityEngine.XR;
using System.Collections.Generic;
using System;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.Native
{
	public static class EnumExtensions
	{
		public static string Name(this WVR_DeviceType e)
		{
			switch (e)
			{
				case WVR_DeviceType.WVR_DeviceType_Controller_Left: return "Controller Left";
				case WVR_DeviceType.WVR_DeviceType_Controller_Right: return "Controller Right";
				case WVR_DeviceType.WVR_DeviceType_HMD: return "HMD";
				case WVR_DeviceType.WVR_DeviceType_NaturalHand_Left: return "Hand Left";
				case WVR_DeviceType.WVR_DeviceType_NaturalHand_Right: return "Hand Right";
				default: return "Invalid";
			}
		}

		public static string Name(this WVR_InputId e)
		{
			switch (e)
			{
				case WVR_InputId.WVR_InputId_Alias1_System: return "Syste";
				case WVR_InputId.WVR_InputId_Alias1_Menu: return "Menu";
				case WVR_InputId.WVR_InputId_Alias1_Grip: return "Grip";
				case WVR_InputId.WVR_InputId_Alias1_DPad_Left: return "DPad_Left";
				case WVR_InputId.WVR_InputId_Alias1_DPad_Up: return "DPad_Up";
				case WVR_InputId.WVR_InputId_Alias1_DPad_Right: return "DPad_Right";
				case WVR_InputId.WVR_InputId_Alias1_DPad_Down: return "DPad_Down";
				case WVR_InputId.WVR_InputId_Alias1_Volume_Up: return "Volume_Up";
				case WVR_InputId.WVR_InputId_Alias1_Volume_Down: return "Volume_Down";
				case WVR_InputId.WVR_InputId_Alias1_Bumper: return "Digital_Trigger";
				case WVR_InputId.WVR_InputId_Alias1_Back: return "Back";
				case WVR_InputId.WVR_InputId_Alias1_Enter: return "Enter";
				case WVR_InputId.WVR_InputId_Alias1_Touchpad: return "Touchpad";
				case WVR_InputId.WVR_InputId_Alias1_Trigger: return "Trigger";
				case WVR_InputId.WVR_InputId_Alias1_Thumbstick: return "Thumbstick";
				default: return e.ToString();
			}
		}
	} // class EnumExtensions


	public static class Coordinate
	{
		public static Vector3 GetVectorFromGL(this Matrix4x4 matrix)
		{
			var x = matrix.m03;
			var y = matrix.m13;
			var z = matrix.m23;

			return new Vector3(x, y, z);
		}
		public static Vector3 GetVectorFromGL(WVR_Vector3f_t glVector)
		{
			return new Vector3(glVector.v0, glVector.v1, -glVector.v2);
		}
		public static void GetVectorFromGL(WVR_Vector3f_t gl_vec, out Vector3 unity_vec)
		{
			unity_vec.x = gl_vec.v0;
			unity_vec.y = gl_vec.v1;
			unity_vec.z = -gl_vec.v2;
		}
		public static void Vector3NormalizeZ(ref Vector3 vec)
		{
			if (vec.z == 0) { return; }
			float normal = vec.z < 0 ? -vec.z : vec.z;
			vec.x /= normal;
			vec.y /= normal;
			vec.z /= normal;
		}

		public static Quaternion GetQuaternionFromGL(Matrix4x4 matrix)
		{
			float tr = matrix.m00 + matrix.m11 + matrix.m22;
			float qw, qx, qy, qz;
			if (tr > 0)
			{
				float S = Mathf.Sqrt(tr + 1.0f) * 2; // S=4*qw
				qw = 0.25f * S;
				qx = (matrix.m21 - matrix.m12) / S;
				qy = (matrix.m02 - matrix.m20) / S;
				qz = (matrix.m10 - matrix.m01) / S;
			}
			else if ((matrix.m00 > matrix.m11) & (matrix.m00 > matrix.m22))
			{
				float S = Mathf.Sqrt(1.0f + matrix.m00 - matrix.m11 - matrix.m22) * 2; // S=4*qx
				qw = (matrix.m21 - matrix.m12) / S;
				qx = 0.25f * S;
				qy = (matrix.m01 + matrix.m10) / S;
				qz = (matrix.m02 + matrix.m20) / S;
			}
			else if (matrix.m11 > matrix.m22)
			{
				float S = Mathf.Sqrt(1.0f + matrix.m11 - matrix.m00 - matrix.m22) * 2; // S=4*qy
				qw = (matrix.m02 - matrix.m20) / S;
				qx = (matrix.m01 + matrix.m10) / S;
				qy = 0.25f * S;
				qz = (matrix.m12 + matrix.m21) / S;
			}
			else
			{
				float S = Mathf.Sqrt(1.0f + matrix.m22 - matrix.m00 - matrix.m11) * 2; // S=4*qz
				qw = (matrix.m10 - matrix.m01) / S;
				qx = (matrix.m02 + matrix.m20) / S;
				qy = (matrix.m12 + matrix.m21) / S;
				qz = 0.25f * S;
			}
#if UNITY_2018_1_OR_NEWER
			return new Quaternion(qx, qy, qz, qw).normalized;
#else
		Vector4 un = new Vector4(qx, qy, qz, qw);
		un.Normalize();
		return new Quaternion(un.x, un.y, un.z, un.w);
#endif
		}
		public static Quaternion GetQuaternionFromGL(WVR_Quatf_t glQuat)
		{
			return new Quaternion(glQuat.x, glQuat.y, -glQuat.z, -glQuat.w);
		}
		public static void GetQuaternionFromGL(WVR_Quatf_t glQuat, out Quaternion unity_quat)
		{
			unity_quat.x = glQuat.x;
			unity_quat.y = glQuat.y;
			unity_quat.z = -glQuat.z;
			unity_quat.w = -glQuat.w;
		}

		public static Vector3 GetScale(this Matrix4x4 matrix)
		{
			Vector3 scale;
			scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
			scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
			scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
			return scale;
		}

		public static Vector4 MatrixMulVector(Matrix4x4 m, Vector4 v)
		{
			Vector4 row0 = m.GetRow(0);
			Vector4 row1 = m.GetRow(1);
			Vector4 row2 = m.GetRow(2);
			Vector4 row3 = m.GetRow(3);

			float v0 = row0.x * v.x + row0.y * v.y + row0.z * v.z + row0.w * v.w;
			float v1 = row1.x * v.x + row1.y * v.y + row1.z * v.z + row1.w * v.w;
			float v2 = row2.x * v.x + row2.y * v.y + row2.z * v.z + row2.w * v.w;
			float v3 = row3.x * v.x + row3.y * v.y + row3.z * v.z + row3.w * v.w;

			return new Vector4(v0, v1, v2, v3);
		}
	} // class Coordinate

	// get new position and rotation from new pose
	[System.Serializable]
    public struct RigidTransform
    {
        public Vector3 pos;
        public Quaternion rot;

        public static RigidTransform identity
        {
            get { return new RigidTransform(Vector3.zero, Quaternion.identity); }
        }

        public RigidTransform(Vector3 pos, Quaternion rot)
        {
            this.pos = pos;
            this.rot = rot;
        }

        public RigidTransform(Transform t)
        {
            this.pos = t.position;
            this.rot = t.rotation;
        }

        public RigidTransform(WVR_Matrix4f_t pose)
        {
            var m = toMatrix44(pose);
            this.pos = Coordinate.GetVectorFromGL(m);
            this.rot = Coordinate.GetQuaternionFromGL(m);
        }

        public static Matrix4x4 toMatrix44(WVR_Matrix4f_t pose, bool glToUnity = true)
        {
            var m = Matrix4x4.identity;
            int sign = glToUnity ? -1 : 1;

            m[0, 0] = pose.m0;
            m[0, 1] = pose.m1;
            m[0, 2] = pose.m2 * sign;
            m[0, 3] = pose.m3;

            m[1, 0] = pose.m4;
            m[1, 1] = pose.m5;
            m[1, 2] = pose.m6 * sign;
            m[1, 3] = pose.m7;

            m[2, 0] = pose.m8 * sign;
            m[2, 1] = pose.m9 * sign;
            m[2, 2] = pose.m10;
            m[2, 3] = pose.m11 * sign;

            m[3, 0] = pose.m12;
            m[3, 1] = pose.m13;
            m[3, 2] = pose.m14;
            m[3, 3] = pose.m15;

            return m;
        }

        public static WVR_Matrix4f_t ToWVRMatrix(Matrix4x4 m, bool unityToGL = true)
        {
            WVR_Matrix4f_t pose;
            int sign = unityToGL ? -1 : 1;

            pose.m0 = m[0, 0];
            pose.m1 = m[0, 1];
            pose.m2 = m[0, 2] * sign;
            pose.m3 = m[0, 3];

            pose.m4 = m[1, 0];
            pose.m5 = m[1, 1];
            pose.m6 = m[1, 2] * sign;
            pose.m7 = m[1, 3];

            pose.m8 = m[2, 0] * sign;
            pose.m9 = m[2, 1] * sign;
            pose.m10 = m[2, 2];
            pose.m11 = m[2, 3] * sign;

            pose.m12 = m[3, 0];
            pose.m13 = m[3, 1];
            pose.m14 = m[3, 2];
            pose.m15 = m[3, 3];

            return pose;
        }

        public static Vector3 ToUnityPos(Vector3 glPos)
        {
            glPos.z *= -1;
            return glPos;
        }

        public void update(WVR_Matrix4f_t pose)
        {
            var m = toMatrix44(pose);
            this.pos = Coordinate.GetVectorFromGL(m);
            this.rot = Coordinate.GetQuaternionFromGL(m);
        }

        public void update(Vector3 position, Quaternion orientation)
        {
            this.pos = position;
            this.rot = orientation;
        }

        public void update(WVR_Pose_t pose)
        {
            Coordinate.GetVectorFromGL(pose.position, out pos);
            Coordinate.GetQuaternionFromGL(pose.rotation, out rot);
        }

        public override bool Equals(object o)
        {
            if (o is RigidTransform)
            {
                RigidTransform t = (RigidTransform)o;
                return pos == t.pos && rot == t.rot;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode() ^ rot.GetHashCode();
        }

        public static bool operator ==(RigidTransform a, RigidTransform b)
        {
            return a.pos == b.pos && a.rot == b.rot;
        }

        public static bool operator !=(RigidTransform a, RigidTransform b)
        {
            return a.pos != b.pos || a.rot != b.rot;
        }

        public static RigidTransform operator *(RigidTransform a, RigidTransform b)
        {
            return new RigidTransform
            {
                rot = a.rot * b.rot,
                pos = a.pos + a.rot * b.pos
            };
        }

        public void Inverse()
        {
            rot = Quaternion.Inverse(rot);
            pos = -(rot * pos);
        }

        public RigidTransform GetInverse()
        {
            var t = new RigidTransform(pos, rot);
            t.Inverse();
            return t;
        }

        public Vector3 TransformPoint(Vector3 point)
        {
            return pos + (rot * point);
        }

        public static Vector3 operator *(RigidTransform t, Vector3 v)
        {
            return t.TransformPoint(v);
        }

	} // struct RigidTransform

	/// <summary> This class is implemented for wrapping the UnityEngine.Input and com.unity.inputsystem. </summary>
	public static class WXRInput
	{
		public static bool GetMouseButton(int button)
		{
			bool held = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			held = Input.GetMouseButton(button);
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
			pressed = Input.GetMouseButtonDown(button);
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
			releases = Input.GetMouseButtonUp(button);
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

				case KeyCode.RightArrow: return Keyboard.current.rightArrowKey;
				case KeyCode.LeftArrow: return Keyboard.current.leftArrowKey;
				case KeyCode.UpArrow: return Keyboard.current.upArrowKey;
				case KeyCode.DownArrow: return Keyboard.current.downArrowKey;

				case KeyCode.Space: return Keyboard.current.spaceKey;

				case KeyCode.LeftAlt: return Keyboard.current.leftAltKey;
				case KeyCode.LeftShift: return Keyboard.current.leftShiftKey;

				case KeyCode.RightAlt: return Keyboard.current.rightAltKey;
				case KeyCode.RightShift: return Keyboard.current.rightShiftKey;
				case KeyCode.RightControl: return Keyboard.current.rightCtrlKey;
				default:
					break;
			}
			return Keyboard.current.leftCtrlKey;
		}
		private static UnityEngine.InputSystem.Controls.KeyControl KeyboardKey(string keyName)
		{
			if (keyName.Equals("a") || keyName.Equals("A")) return Keyboard.current.aKey;
			if (keyName.Equals("b") || keyName.Equals("B")) return Keyboard.current.bKey;
			if (keyName.Equals("c") || keyName.Equals("C")) return Keyboard.current.cKey;
			if (keyName.Equals("d") || keyName.Equals("D")) return Keyboard.current.dKey;
			if (keyName.Equals("e") || keyName.Equals("E")) return Keyboard.current.eKey;
			if (keyName.Equals("f") || keyName.Equals("F")) return Keyboard.current.fKey;
			if (keyName.Equals("g") || keyName.Equals("G")) return Keyboard.current.gKey;
			if (keyName.Equals("h") || keyName.Equals("H")) return Keyboard.current.hKey;
			if (keyName.Equals("i") || keyName.Equals("I")) return Keyboard.current.iKey;
			if (keyName.Equals("j") || keyName.Equals("J")) return Keyboard.current.jKey;
			if (keyName.Equals("k") || keyName.Equals("K")) return Keyboard.current.kKey;
			if (keyName.Equals("l") || keyName.Equals("L")) return Keyboard.current.lKey;
			if (keyName.Equals("m") || keyName.Equals("M")) return Keyboard.current.mKey;
			if (keyName.Equals("n") || keyName.Equals("N")) return Keyboard.current.nKey;
			if (keyName.Equals("o") || keyName.Equals("O")) return Keyboard.current.oKey;
			if (keyName.Equals("p") || keyName.Equals("P")) return Keyboard.current.pKey;
			if (keyName.Equals("q") || keyName.Equals("Q")) return Keyboard.current.qKey;
			if (keyName.Equals("r") || keyName.Equals("R")) return Keyboard.current.rKey;
			if (keyName.Equals("s") || keyName.Equals("S")) return Keyboard.current.sKey;
			if (keyName.Equals("t") || keyName.Equals("T")) return Keyboard.current.tKey;
			if (keyName.Equals("u") || keyName.Equals("U")) return Keyboard.current.uKey;
			if (keyName.Equals("v") || keyName.Equals("V")) return Keyboard.current.vKey;
			if (keyName.Equals("w") || keyName.Equals("W")) return Keyboard.current.wKey;
			if (keyName.Equals("x") || keyName.Equals("X")) return Keyboard.current.xKey;
			if (keyName.Equals("y") || keyName.Equals("Y")) return Keyboard.current.yKey;
			if (keyName.Equals("z") || keyName.Equals("Z")) return Keyboard.current.zKey;

			return Keyboard.current.leftCtrlKey;
		}
#endif
		public static bool GetKey(KeyCode key)
		{
			bool held = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			held = Input.GetKey(key);
#elif ENABLE_INPUT_SYSTEM
			held = KeyboardKey(key).isPressed;
#endif
			return held;
		}
		public static bool GetKeyDown(KeyCode key)
		{
			bool pressed = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			pressed = Input.GetKeyDown(key);
#elif ENABLE_INPUT_SYSTEM
			pressed = KeyboardKey(key).wasPressedThisFrame;
#endif
			return pressed;
		}
		public static bool GetKeyUp(KeyCode key)
		{
			bool releases = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			releases = Input.GetKeyUp(key);
#elif ENABLE_INPUT_SYSTEM
			releases = KeyboardKey(key).wasReleasedThisFrame;
#endif
			return releases;
		}

		public static bool GetKey(string keyName)
		{
			bool held = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			held = Input.GetKey(keyName);
#elif ENABLE_INPUT_SYSTEM
			held = KeyboardKey(keyName).isPressed;
#endif
			return held;
		}
		public static bool GetKeyDown(string keyName)
		{
			bool pressed = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			pressed = Input.GetKeyDown(keyName);
#elif ENABLE_INPUT_SYSTEM
			pressed = KeyboardKey(keyName).wasPressedThisFrame;
#endif
			return pressed;
		}
		public static bool GetKeyUp(string keyName)
		{
			bool releases = false;
#if ENABLE_LEGACY_INPUT_MANAGER
			releases = Input.GetKeyUp(keyName);
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
			held = Input.GetButton(buttonName);
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
			pressed = Input.GetButtonDown(buttonName);
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
			released = Input.GetButtonUp(buttonName);
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

		public static float GetAxis(string axisName)
		{
			float axis = 0;
#if ENABLE_LEGACY_INPUT_MANAGER
		axis = Input.GetAxis(axisName);
#endif
			return axis;
		}

		static string[] s_JoystickNames = new string[] { };
		static List<string> joysticks = new List<string>();
		public static string[] GetJoystickNames()
		{
#if ENABLE_LEGACY_INPUT_MANAGER
			s_JoystickNames = Input.GetJoystickNames();
#elif ENABLE_INPUT_SYSTEM
			joysticks.Clear();

			// Find all gamepads and joysticks.
			var devices = InputSystem.devices;
			for (var i = 0; i < devices.Count; i++)
			{
				var device = devices[i];
				if (device is Joystick)
				{
					Debug.Log("Found joystick " + device);
					joysticks.Add(device.name);
				}
			}

			s_JoystickNames = joysticks.ToArray();
#endif
			return s_JoystickNames;
		}
	}

	[Obsolete("Please directly compare the struct by == or != instead of using this helper class.")]
	public static class WVRStructCompare
	{
		/// <summary>
		/// A helper function for comparing two <see cref="WVR_Uuid"/>.
		/// </summary>
		/// <param name="uuid1">A <see cref="WVR_Uuid"/> of which will be in the comparison.</param>
		/// <param name="uuid2">A <see cref="WVR_Uuid"/> of which will be in the comparison.</param>
		/// <returns>
		/// true if the Uuids are the identical, false if they are not.
		/// </returns>
		[Obsolete("Please directly compare the uuids by == or != instead of using this function.")]
		public static bool IsUUIDEqual(WVR_Uuid uuid1, WVR_Uuid uuid2)
		{
			return uuid1.Equals(uuid2);
		}

		[Obsolete("Please directly compare the pose by == or != instead of using this function.")]
		public static bool WVRPoseEqual(WVR_Pose_t pose1, WVR_Pose_t pose2)
		{
			return pose1 == pose2;
		}

		[Obsolete("Please directly compare the pose by == or != instead of using this function.")]
		public class MarkerIdComparer : IEqualityComparer<WVR_Uuid> //For using WVR_Uuid as key in Dictionary
		{
			public bool Equals(WVR_Uuid uuid1, WVR_Uuid uuid2)
			{
				return uuid1 == uuid2;
			}

			public int GetHashCode(WVR_Uuid uuid)
			{
				unchecked
				{
					int hash = 17;
					foreach (byte byteData in uuid.data)
					{
						hash = hash * 31 + byteData.GetHashCode();
					}

					return hash;
				}
			}
		}
	}
}
