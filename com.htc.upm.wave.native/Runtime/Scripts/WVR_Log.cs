// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Text;
#if UNITY_ANDROID && !UNITY_EDITOR
using System.Runtime.InteropServices;
#elif UNITY_STANDALONE
using System.Runtime.InteropServices;
using System.IO;
#endif
using UnityEngine;
using Wave.XR;
using Wave.XR.Settings;

namespace Wave.Native
{
	public class Log
	{
		private static WaveXRSettings m_WaveXRSettings = null;
		private enum DebugFlag : uint
		{
			WARNING = 1 << Constants.DebugLogFlag.Debug1,
			INFO = 1 << Constants.DebugLogFlag.Debug2,
			DEBUG = 1 << Constants.DebugLogFlag.Debug3,
			VERBOSE = 1 << Constants.DebugLogFlag.Debug4,
		}
		private static bool LogEnabled(DebugFlag flag)
		{
			if (m_WaveXRSettings == null) { m_WaveXRSettings = WaveXRSettings.GetInstance(); }
			if (m_WaveXRSettings == null) { return true; }
			if ((m_WaveXRSettings.debugLogFlagForUnity & (uint)flag) == 0) { return false; }
			return true;
		}
		[Obsolete("This variable is deprecated. Please use Project Settings > WaveXRSettings > Log Flag for Unity instead.")]
		public static bool EnableDebugLog = true;
		private const int LOG_VERBOSE = 2;
		private const int LOG_DEBUG = 3;
		private const int LOG_INFO = 4;
		private const int LOG_WARN = 5;
		private const int LOG_ERROR = 6;

		// A default StringBuilder
		// Please don't use Insert().  Insert() will let StringBuilder create new buffer when Clear().
		// Please use SB only in game thread.  It's not thread safe.
		public readonly static StringBuilder SB = new StringBuilder();
		public static StringBuilder CSB { get { return SB.Clear(); } }

#if UNITY_ANDROID && !UNITY_EDITOR
		[DllImportAttribute("log", EntryPoint = "__android_log_print", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		internal static extern int __log_print(int prio, string tag, string fmt, System.IntPtr ptr);

#elif UNITY_STANDALONE
		[DllImportAttribute("wave_api", EntryPoint = "WVR_LOGV", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void WVR_LOGV(string log);
		[DllImportAttribute("wave_api", EntryPoint = "WVR_LOGD", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void WVR_LOGD(string log);
		[DllImportAttribute("wave_api", EntryPoint = "WVR_LOGI", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void WVR_LOGI(string log);
		[DllImportAttribute("wave_api", EntryPoint = "WVR_LOGW", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void WVR_LOGW(string log);
		[DllImportAttribute("wave_api", EntryPoint = "WVR_LOGE", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void WVR_LOGE(string log);

		private static int __log_print(int prio, string tag, string fmt, System.IntPtr ptr)
		{
			if (prio == LOG_VERBOSE)
				WVR_LOGV(tag + " " + fmt);
			else if (prio == LOG_DEBUG)
				WVR_LOGD(tag + " " + fmt);
			else if (prio == LOG_INFO)
				WVR_LOGI(tag + " " + fmt);
			else if (prio == LOG_WARN)
				WVR_LOGW(tag + " " + fmt);
			else
				WVR_LOGE(tag + " " + fmt);
			return 0;
		}
#else
        private static int __log_print(int prio, string tag, string fmt, System.IntPtr ptr)
		{
			return 0;
		}
#endif

		public static void v(string tag, string message, bool logInEditor = false)
		{
			if (!LogEnabled(DebugFlag.VERBOSE)) { return; }
			__log_print(LOG_VERBOSE, tag, message, System.IntPtr.Zero);
#if UNITY_EDITOR
			if (logInEditor)
				Debug.Log(tag + " " + message);
#endif
		}

		public static void d(string tag, string message, bool logInEditor = false)
		{
			if (!LogEnabled(DebugFlag.DEBUG)) { return; }
			__log_print(LOG_DEBUG, tag, message, System.IntPtr.Zero);
#if UNITY_EDITOR
			if (logInEditor)
				Debug.Log(tag + " " + message);
#endif
		}
		public static void i(string tag, string message, bool logInEditor = false)
		{
			if (!LogEnabled(DebugFlag.INFO)) { return; }
			__log_print(LOG_INFO, tag, message, System.IntPtr.Zero);
#if UNITY_EDITOR
			if (logInEditor)
				Debug.Log(tag + " " + message);
#endif
		}
		public static void w(string tag, string message, bool logInEditor = false)
		{
			if (!LogEnabled(DebugFlag.WARNING)) { return; }
			__log_print(LOG_WARN, tag, message, System.IntPtr.Zero);
#if UNITY_EDITOR
			if (logInEditor)
				Debug.LogWarning(tag + " " + message);
#endif
		}
		public static void e(string tag, string message, bool logInEditor = false)
		{
			__log_print(LOG_ERROR, tag, message, System.IntPtr.Zero);
#if UNITY_EDITOR
			if (logInEditor)
				Debug.LogError(tag + " " + message);
#endif
		}

		// StringBuilders
		public static void v(string tag, StringBuilder sb, bool logInEditor = false)
		{
			if (!LogEnabled(DebugFlag.VERBOSE)) { return; }
			v(tag, sb.ToString(), logInEditor);
		}
		public static void d(string tag, StringBuilder sb, bool logInEditor = false)
		{
			if (!LogEnabled(DebugFlag.DEBUG)) { return; }
			d(tag, sb.ToString(), logInEditor);
		}
		public static void i(string tag, StringBuilder sb, bool logInEditor = false)
		{
			if (!LogEnabled(DebugFlag.INFO)) { return; }
			i(tag, sb.ToString(), logInEditor);
		}
		public static void w(string tag, StringBuilder sb, bool logInEditor = false)
		{
			if (!LogEnabled(DebugFlag.WARNING)) { return; }
			w(tag, sb.ToString(), logInEditor);
		}
		public static void e(string tag, StringBuilder sb, bool logInEditor = false)
		{
			e(tag, sb.ToString(), logInEditor);
		}


		public static EnterAndExit ee(string message)
		{
			return new EnterAndExit("Unity", message, "+", "-");
		}

		public static EnterAndExit ee(string tag, string message)
		{
			return new EnterAndExit(tag, message, "+", "-");
		}

		public static EnterAndExit ee(string tag, string postfixEnter, string postfixExit)
		{
			return new EnterAndExit(tag, "", postfixEnter, postfixExit);
		}

		public static EnterAndExit ee(string tag, string message, string postfixEnter, string postfixExit)
		{
			return new EnterAndExit(tag, message, postfixEnter, postfixExit);
		}

		/**
		 * The *using* syntax will help calling the dispose of its argument.
		 * 
		 * Usage example:
		 * void func() {
		 *   using(var ee = Log.ee("WVR", "func is ", "enter", "exit")
		 *   {
		 *	  // Do your work here
		 *   }
		 * }
		 * 
		 * Log:
		 *	WVR D func is enter
		 *	... other logs
		 *	WVR D func is exit
		**/
		public class EnterAndExit : IDisposable
		{
			string tag, message, enter, exit;
			bool logInEditor = false;
			public EnterAndExit(string tag, string message, string postfixEnter, string postfixExit, bool logInEditor = false)
			{
				this.tag = tag;
				this.message = message;
				this.exit = postfixExit;
				this.logInEditor = logInEditor;
				Log.d(tag, message + postfixEnter, logInEditor);
			}

			public void Dispose()
			{
				Log.d(tag, message + exit, logInEditor);
			}
		}

		public class PeriodLog
		{
			public delegate string StringProcessDelegate();

			public float interval = 3;   // default is 3 seconds
			private float lastTime = 0;
			public bool Print { get; private set; }

			public PeriodLog()
			{
				lastTime = Time.realtimeSinceStartup;
			}

			public void check()
			{
				var time = Time.realtimeSinceStartup;
				Print = false;
				if (time > (lastTime + interval))
				{
					lastTime = time;
					Print = true;
				}
			}

			// Only debug log need print periodically.  Other type please just print it out.
			public void d(string tag, string message, bool logInEditor = false)
			{
				if (Print) Log.d(tag, message, logInEditor);
			}

			// This is better the string version.  Only Print will trigger the ToString().  Save more GC.Alloc().
			public void d(string tag, StringBuilder sb, bool logInEditor = false)
			{
				if (Print) Log.d(tag, sb.ToString(), logInEditor);
			}

			// If not print, the delegate will not be processed.  Save performance waste of string concat.
			[Obsolete("The delegate still use GC.Alloc to remember your variable.")]
			public void d(string tag, StringProcessDelegate strDelegate, bool logInEditor = false)
			{
				if (Print) Log.d(tag, strDelegate(), logInEditor);
			}
		}

		public static PeriodLog gpl = new PeriodLog();
	}
	public static class StringBuilderExtensions
	{
		public static StringBuilder AppendMatrix(this StringBuilder sb, string name, Matrix4x4 m)
		{
			return sb.AppendFormat("{0,-16}=", name).AppendLine()
				.AppendFormat(" / {0:F6} {1:F6} {2:F6} {3:F6} \\", m.m00, m.m01, m.m02, m.m03).AppendLine()
				.AppendFormat(" | {0:F6} {1:F6} {2:F6} {3:F6} |", m.m10, m.m11, m.m12, m.m13).AppendLine()
				.AppendFormat(" | {0:F6} {1:F6} {2:F6} {3:F6} |", m.m20, m.m21, m.m22, m.m23).AppendLine()
				.AppendFormat(" \\ {0:F6} {1:F6} {2:F6} {3:F6} /", m.m30, m.m31, m.m32, m.m33);
		}

		public static StringBuilder AppendVector3(this StringBuilder sb, string name, Vector3 v)
		{
			return sb.AppendFormat("{0,-16}=({1:F6}, {2:F6}, {3:F6})", name, v.x, v.y, v.z);
		}

		public static StringBuilder AppendVector4(this StringBuilder sb, string name, Vector4 v)
		{
			return sb.AppendFormat("{0,-16}=({1:F6}, {2:F6}, {3:F6}, {4:F6})", name, v.x, v.y, v.z, v.w);
		}

		public static StringBuilder AppendQuaternion(this StringBuilder sb, string name, Quaternion q)
		{
			return sb.AppendFormat("{0,-16}=({1:F6}, {2:F6}, {3:F6}, {4:F6})", name, q.x, q.y, q.z, q.w);
		}
	}
}
