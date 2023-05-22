using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Pure Unity code
namespace Wave.XR.Sample
{
	public class ApplyQuality : MonoBehaviour
	{
		public int desiredQuality = 0;

		public static class FunctionsHelper
		{
			[DllImport("wvrunityxr", EntryPoint = "GetFuncPtr")]
			internal static extern int GetFuncPtr(string name, ref System.IntPtr value);

			public static T GetFuncPtr<T>(string name)
			{
				IntPtr ptr = new IntPtr();
				try
				{
					int ec = GetFuncPtr(name, ref ptr);
					Debug.Log("GetFuncPtr<" + typeof(T) + ">(" + name + ", [out]" + ptr + ")=" + ec);
					if (ec != 0)
					{
						return default;
					}
					return Marshal.GetDelegateForFunctionPointer<T>(ptr);
				}
				catch (DllNotFoundException)
				{
					// Not exist
					return default;
				}
			}
		}

		public class RenderFunctions
		{
			public delegate void NotifyQualityLevelChangeDelegate();
			private static NotifyQualityLevelChangeDelegate notifyQualityLevelChange = null;
			public static NotifyQualityLevelChangeDelegate NotifyQualityLevelChange
			{
				get
				{
					if (notifyQualityLevelChange == null)
						notifyQualityLevelChange = FunctionsHelper.GetFuncPtr<NotifyQualityLevelChangeDelegate>("NotifyQualityLevelChange");
					return notifyQualityLevelChange;
				}
			}
		}

		void OnEnable()
		{
			if (desiredQuality == QualitySettings.GetQualityLevel())
				return;
			QualitySettings.SetQualityLevel(desiredQuality, true);
			RenderFunctions.NotifyQualityLevelChange?.Invoke();
		}
	}
}
