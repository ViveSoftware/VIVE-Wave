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

namespace Wave.Essence.Extra
{
	public class OEMConfig
	{
		private static bool isSetCallback = false;
		public delegate void OEMConfigChangedDelegate();
		public static event OEMConfigChangedDelegate onOEMConfigChanged = null;

		public static JSON_ControllerDesc jcd = null;
		private static bool hasControllerDesc = false;

        public static JSON_HandModelDesc jhd = null;
        private static bool hasHandDesc = false;

        public static JSON_BatteryPolicy batteryPolicy = null;
		private static bool hasBatteryPolicy = false;

		public static JSON_BeamPolicy beamPolicy = null;
		private static bool hasBeamPolicy = false;

		public static void OEMConfig_Changed()
		{
			Log.i("OEMConfig", "onConfigChanged callback");
			if (onOEMConfigChanged == null)
			{
				Log.i("OEMConfig", "onConfigChanged delegate is null");
				return;
			}
			updateOEMConfig();

			onOEMConfigChanged.Invoke();
		}

		public static void initOEMConfig()
		{
			if (!isSetCallback)
			{
				Log.i("OEMConfig", "initOEMConfig");
				Interop.WVR_SetOEMConfigChangedCallback(OEMConfig_Changed);
				updateOEMConfig();

				isSetCallback = true;
			}
		}

		private static void updateOEMConfig()
		{
			Log.i("OEMConfig", "update and parse config");
			string json_string;
			hasControllerDesc = false;
			hasBatteryPolicy = false;
			hasBeamPolicy = false;
            hasHandDesc = false;

			json_string = Interop.WVR_GetOEMConfigByKey("controller_property");

			if (!json_string.Equals(""))
			{
				jcd = JsonUtility.FromJson<JSON_ControllerDesc>(json_string);
				hasControllerDesc = true;
			}

			json_string = Interop.WVR_GetOEMConfigByKey("battery_indicator");

			if (!json_string.Equals(""))
			{
				batteryPolicy = JsonUtility.FromJson<JSON_BatteryPolicy>(json_string);
				hasBatteryPolicy = true;
			}

			json_string = Interop.WVR_GetOEMConfigByKey("controller_singleBeam");

			if (!json_string.Equals(""))
			{
				beamPolicy = JsonUtility.FromJson<JSON_BeamPolicy>(json_string);
				hasBeamPolicy = true;
			}

            json_string = Interop.WVR_GetOEMConfigByKey("hand_styles");

            if (!json_string.Equals(""))
            {
                jhd = JsonUtility.FromJson<JSON_HandModelDesc>(json_string);
                hasHandDesc = true;
            }
        }

		public static JSON_BatteryPolicy getBatteryPolicy()
		{
			initOEMConfig();
			return (hasBatteryPolicy ? batteryPolicy : null);
		}

		public static JSON_BeamPolicy getSingleBeamEnablePolicy()
		{
			initOEMConfig();
			return (hasBeamPolicy ? beamPolicy : null);
		}

		public static JSON_BeamDesc getBeamDesc()
		{
			initOEMConfig();
			return (hasControllerDesc ? jcd.beam : null);
		}

		public static JSON_PointerDesc getControllerPointerDesc()
		{
			initOEMConfig();
			return (hasControllerDesc ? jcd.pointer : null);
		}

		public static JSON_ModelDesc getControllerModelDesc()
		{
			initOEMConfig();
			return (hasControllerDesc ? jcd.model : null);
		}

        public static JSON_HandModelDesc getHandModelDesc()
        {
            initOEMConfig();
            return (hasHandDesc ? jhd : null);
        }
    }
}
