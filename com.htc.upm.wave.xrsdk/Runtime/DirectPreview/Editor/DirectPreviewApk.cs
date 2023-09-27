// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;

#if UNITY_EDITOR && UNITY_EDITOR_WIN
namespace Wave.XR.DirectPreview.Editor
{
	public class DirectPreviewAPK
	{
		private static void GeneralSettings()
		{
			PlayerSettings.Android.bundleVersionCode = 1;
			PlayerSettings.bundleVersion = "2.0.0";
			PlayerSettings.companyName = "HTC Corp.";
			PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
			PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
		}

		//[UnityEditor.MenuItem("Wave/DirectPreview/Install Device APK", priority = 801)]
		public static void InstallSimulator()
		{
			//UninstallSimulatorInner();
			InstallSimulatorInner();
			//CreateDirectPreviewFolderInner();
			//PushConfigInner();
		}

		//[UnityEditor.MenuItem("Wave/DirectPreview/Start Device APK", priority = 802)]
        public static void StartSimulator()
		{
			StopSimulatorInner();
			KillSimulatorInner();
			StartSimulatorInner();
		}

		//[UnityEditor.MenuItem("Wave/DirectPreview/Stop Device APK", priority = 803)]
        public static void StopSimulator()
		{
			StopSimulatorInner2();
			//KillSimulatorInner();
		}

		public static void UninstallSimulatorInner()
		{
			try
			{
				Process myProcess = new Process();
				myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				myProcess.StartInfo.Arguments = "/c adb uninstall com.htc.vr.directpreview.agent.unity";
				//myProcess.EnableRaisingEvents = true;
				myProcess.Start();
				myProcess.WaitForExit();
				int ExitCode = myProcess.ExitCode;
				if (ExitCode == 0)
				{
					UnityEngine.Debug.Log("Uninstall Direct Preview device APK succeeded.");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Uninstall Direct Preview device APK failed.");
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}

        public static void InstallSimulatorInner()
		{
			try
			{
				var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/RRClient/Vive_rrClient.apk");
				UnityEngine.Debug.Log("Install Direct Preview device APK pull path = " + absolutePath);

				Process myProcess = new Process();
				myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				myProcess.StartInfo.Arguments = "/c adb install -r -g -d \"" + absolutePath + "\"";
				myProcess.Start();
				myProcess.WaitForExit();
				int ExitCode = myProcess.ExitCode;
				if (ExitCode == 0)
				{
					UnityEngine.Debug.Log("Install Direct Preview device APK succeeded.");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Install Direct Preview device APK failed.");
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}

		public static void StartSimulatorInner()
		{
			try
			{
				Process myProcess = new Process();
				myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				myProcess.StartInfo.Arguments = "/c adb shell am start -n com.htc.vr.directpreview.agent.unity/com.vive.rrclient.RRClient";
				myProcess.Start();
				myProcess.WaitForExit();
				int ExitCode = myProcess.ExitCode;
				if (ExitCode == 0)
				{
					UnityEngine.Debug.Log("Start Direct Preview device APK succeeded.");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Start Direct Preview device APK failed.");
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}

		public static void KillSimulatorInner()
		{
			try
			{
				Process myProcess = new Process();
				myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				myProcess.StartInfo.Arguments = "/c adb shell am kill com.htc.vr.directpreview.agent.unity";
				myProcess.Start();
				myProcess.WaitForExit();
				int ExitCode = myProcess.ExitCode;
				if (ExitCode == 0)
				{
					UnityEngine.Debug.Log("Kill Direct Preview device APK process succeeded.");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Kill Direct Preview device APK process failed.");
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}

		public static void StopSimulatorInner2()
		{
			try
			{
				Process myProcess = new Process();
				myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				myProcess.StartInfo.Arguments = "/c adb shell am broadcast -a com.htc.vr.directpreview.agent.SHUTDOWN";
				myProcess.Start();
				myProcess.WaitForExit();
				int ExitCode = myProcess.ExitCode;
				if (ExitCode == 0)
				{
					UnityEngine.Debug.Log("Stop Direct Preview device APK succeeded.(2)");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Stop Direct Preview device APK failed.(2)");
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}

		public static void StopSimulatorInner()
		{
			try
			{
				Process myProcess = new Process();
				myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				myProcess.StartInfo.Arguments = "/c adb shell am force-stop com.htc.vr.directpreview.agent.unity";
				myProcess.Start();
				myProcess.WaitForExit();
				int ExitCode = myProcess.ExitCode;
				if (ExitCode == 0)
				{
					UnityEngine.Debug.Log("Stop Direct Preview device APK succeeded.");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Stop Direct Preview device APK failed.");
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}

		public static void CreateDirectPreviewFolderInner()
		{
			try
			{
				Process myProcess = new Process();

				myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				myProcess.StartInfo.Arguments = "/c adb shell mkdir /sdcard/DirectPreview/";
				//myProcess.EnableRaisingEvents = true;
				myProcess.Start();
				myProcess.WaitForExit();
				int ExitCode = myProcess.ExitCode;
				if (ExitCode == 0)
				{
					UnityEngine.Debug.Log("Create Direct Preview folder succeeded.");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Create Direct Preview folder failed, /sdcard/Direct/Preview folder might be exist!");
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.Log(e);
			}
		}

		private static string configPath()
		{
			var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/DirectPreviewConfig.json");
			UnityEngine.Debug.Log("configPath = " + absolutePath);

			return absolutePath;
		}

		public static void PushConfigInner()
		{
			string fileName = "";
			fileName = configPath();

			writeConfig();

			try
			{
				Process myProcess = new Process();

				myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				myProcess.StartInfo.Arguments = "/c adb push \"" + fileName + "\"" + " /sdcard/DirectPreview/config.json";
				//myProcess.EnableRaisingEvents = true;
				myProcess.Start();
				myProcess.WaitForExit();
				int ExitCode = myProcess.ExitCode;
				if (ExitCode == 0)
				{
					UnityEngine.Debug.Log("Push config succeeded.");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Push config failed");
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.Log(e);
			}

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
		}

		private static string LocalIPAddress()
		{
			IPHostEntry host;
			string localIP = "";
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = ip.ToString();
					break;
				}
			}
			return localIP;
		}

		private static string getRenderTarget()
		{
			string ret = "";

			var rd = EditorPrefs.GetInt("DPTargetDevice");

			if (rd == 1)
			{
				ret = "FOCUS";
			}
			else
			{
				ret = "COSMOS";
			}

			if (ret.Equals("FOCUS"))
			{
				UnityEngine.Debug.Log("Render target is FOCUS");
			} else
			{
				UnityEngine.Debug.Log("Render target is Other");
			}

			return ret;
		}

		private static void writeConfig()
		{
			string fileName = "";
			fileName = configPath();

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			var sr = File.CreateText(fileName);

			sr.WriteLine("{");
			sr.WriteLine(" \"IP\" : \"" + LocalIPAddress() + "\",");
			sr.WriteLine(" \"Port\" : 6555,");
			sr.WriteLine(" \"HMD\" : \"" + getRenderTarget() + "\",");
			sr.WriteLine(" ");
			sr.WriteLine(" \"RenderWidth\" : 1440,");
			sr.WriteLine(" \"RenderHeight\" : 1600,");
			sr.WriteLine(" \"RenderSizeScale\" : 1.0,");
			sr.WriteLine(" \"RenderOverfillScale\" : 1.3,");
			sr.WriteLine(" ");
			sr.WriteLine(" \"UseAutoPrecdictTime\" : true,");
			sr.WriteLine(" \"CtlPredictRate\" : 6,");
			sr.WriteLine(" \"HmdPredictRatio\" : 0.615,");
			sr.WriteLine(" \"CtlPredictRatio\" : 0.615,");
			sr.WriteLine(" \"HmdPredict\" : 41,");
			sr.WriteLine(" \"ControllerPredict\" : 40,");
			sr.WriteLine(" \"MaxHmdPredictTimeInMs\" : 35,");
			sr.WriteLine(" \"MaxCtlPredictTimeInMs\" : 20,");
			sr.WriteLine(" ");
			sr.WriteLine(" \"RoomHeight\" : 1.6");
			sr.WriteLine("}");
			sr.Close();
		}
	}
}
#endif