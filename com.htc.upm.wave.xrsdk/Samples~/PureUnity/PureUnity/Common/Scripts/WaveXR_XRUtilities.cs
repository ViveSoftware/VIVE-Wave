using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Wave.XR.Sample
{
	public static class Utils
	{
		public const string WaveDisplaySubsystemId = "WVR Display Provider";
		public const string WaveInputSubsystemId = "WVR Input Provider";
		public const string WaveMeshSubsystemId = "WVR Mesh Provider";

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
			foreach (var subsystem in subsystems)
			{
				if (subsystem.SubsystemDescriptor.id == WaveDisplaySubsystemId)
				{
					return subsystem;
				}
			}
			return null;
		}

		static XRInputSubsystem GetInputSubsystem()
		{
			var subsystems = new List<XRInputSubsystem>();
			SubsystemManager.GetInstances(subsystems);
			foreach (var subsystem in subsystems)
			{
				if (subsystem.SubsystemDescriptor.id == WaveInputSubsystemId)
				{
					return subsystem;
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
	} // Utils
}
