using System;

namespace Wave.XR
{
    /// <summary>
    /// Static constants
    /// </summary>
    public static class Constants
    {
        public const string k_DisplaySubsystemId = "WVR Display Provider";
        public const string k_InputSubsystemId = "WVR Input Provider";
        public const string k_MeshSubsystemId = "WVR Mesh Provider";

		public const string SDKPackageName = "com.htc.upm.wave.xrsdk";
		public const string NativePackageName = "com.htc.upm.wave.native";
		public const string EssencePackageName = "com.htc.upm.wave.essence";

		/// <summary>
		/// Key we use to store and retrieve custom configuration settings from EditorBuildSettings
		/// </summary>
		public const string k_SettingsKey = SDKPackageName + ".xrsettings";

		// This error code need map to native's ErrorCode.
		public enum ErrorCode
        {
            NoError = 0,
            UnknownError,
            TooLate,
            TooEarly,
            NotExist,
            TypeIsWrong,
            NotAllowed,
            NullReference,
            OutOfRange,
            TooShort,
        };

        // This flag define need map to native's.  Used as a bit shifter.  For example, uint flag = (1 << Basic1 || 1 << Render2)
        public enum DebugLogFlag
        {
            Default = 1 << Basic1 | 1 << Lifecycle1 | 1 << Render1 | 1 << Input1,
            Basic1 = 0,
            Basic2,
            Basic3,
            Basic4,
            Debug1,
            Debug2,
            Debug3,
            Debug4,
            Lifecycle1,
            Lifecycle2,
            Lifecycle3,
            Lifecycle4,
            Render1,
            Render2,
            Render3,
            Render4,
            Input1,
            Input2,
            Input3,
            Input4,
            // max is up to 32
            BasicMask = 0xF << Basic1,
            DebugMask = 0xF << Debug1,
            LifecycleMask = 0xF << Lifecycle1,
            RenderMask = 0xF << Render1,
            InputMask = 0xF << Input1,
            All = 0x7FFFFFFF
        };
    }
}
