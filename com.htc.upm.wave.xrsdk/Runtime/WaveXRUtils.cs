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

namespace Wave.XR
{
	public static class Utils
	{
		private static XRDisplaySubsystem displaySubsystem = null;
		public static XRDisplaySubsystem DisplaySubsystem
		{
			get
			{
				if (displaySubsystem == null)
					displaySubsystem = GetDisplaySubsystem();
				return displaySubsystem;
			}
		}

		private static XRInputSubsystem inputSubsystem = null;
		public static XRInputSubsystem InputSubsystem
		{
			get
			{
				if (inputSubsystem == null)
					inputSubsystem = GetInputSubsystem();
				return inputSubsystem;
			}
		}

		static XRDisplaySubsystem GetDisplaySubsystem()
		{
			var subsystems = new List<XRDisplaySubsystem>();
			SubsystemManager.GetInstances(subsystems);
			if (subsystems != null && subsystems.Count > 0)
			{
				for (int i = 0; i < subsystems.Count; i++)
				{
					if (subsystems[i].SubsystemDescriptor.id == Constants.k_DisplaySubsystemId)
					{
						return subsystems[i];
					}
				}
			}
			return null;
		}

		static XRInputSubsystem GetInputSubsystem()
		{
			var subsystems = new List<XRInputSubsystem>();
			SubsystemManager.GetInstances(subsystems);
			if (subsystems != null && subsystems.Count > 0)
			{
				for (int i = 0; i < subsystems.Count; i++)
				{
					if (subsystems[i].SubsystemDescriptor.id == Constants.k_InputSubsystemId)
					{
						return subsystems[i];
					}
				}
			}
			return null;
		}

		static void UpdateStatic()
		{
			displaySubsystem = GetDisplaySubsystem();
			inputSubsystem = GetInputSubsystem();
		}

		private static float _copysign(float sizeval, float signval)
		{
			return Mathf.Sign(signval) == 1 ? Mathf.Abs(sizeval) : -Mathf.Abs(sizeval);
		}
	} // class Utils
}