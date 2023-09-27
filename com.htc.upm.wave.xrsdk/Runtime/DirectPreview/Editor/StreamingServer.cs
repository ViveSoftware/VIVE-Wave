// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
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
using System.Linq;
using System.IO;

#if UNITY_EDITOR && UNITY_EDITOR_WIN
namespace Wave.XR.DirectPreview.Editor
{
	public class StreamingServer
	{
		public static Process myProcess = new Process();

		//[UnityEditor.MenuItem("Wave/DirectPreview/Start Streaming Server", priority = 701)]
		static void StartStreamingServerMenu()
		{
			StartStreamingServer();
		}

		//[UnityEditor.MenuItem("Wave/DirectPreview/Stop Streaming Server", priority = 702)]
		static void StopStreamingServerMenu()
		{
			StopStreamingServer();
		}

		public static bool isStreamingServerExist()
		{
			var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/RRServer/RRserver.exe");

			UnityEngine.Debug.Log("StreamingServer pull path = " + absolutePath);

			return File.Exists(absolutePath);
		}

		// Launch rrServer
		public static void StartStreamingServer()
		{
			if (isStreamingServerExist())
			{
				try
				{
					var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/RRServer");
					var driveStr = absolutePath.Substring(0, 2);

					UnityEngine.Debug.Log("StreamingServer in " + absolutePath);
					UnityEngine.Debug.Log("driveStr " + driveStr);

					//Get the path of the Game data folder
					string taskstring = "";
					Process[] procs = Process.GetProcessesByName("RRServer");
					if (procs.Length > 0)
					{
						UnityEngine.Debug.Log("RRServer running");
						taskstring += "taskkill /F /IM RRServer.exe && ";
					}
					procs = Process.GetProcessesByName("VHConsole");
					if (procs.Length > 0)
					{
						UnityEngine.Debug.Log("VHConsole running");
						taskstring += "taskkill /F /IM VHConsole.exe && ";
					}
					procs = Process.GetProcessesByName("RRConsole");
					if (procs.Length > 0)
					{
						UnityEngine.Debug.Log("RRConsole running");
						taskstring += "taskkill /F /IM RRConsole.exe && ";
					}
					taskstring += driveStr + " && cd " + absolutePath + " && RRserver";
					UnityEngine.Debug.Log(taskstring);
					myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
					myProcess.StartInfo.Arguments = "/c " + taskstring;
					myProcess.Start();
				}
				catch (Exception e)
				{
					UnityEngine.Debug.LogError(e);
				}
			}
			else
			{
				// dpServer is not found
				UnityEngine.Debug.LogError("Streaming server is not found, please update full package from https://developer.vive.com/resources/knowledgebase/wave-sdk/");
			}

		}
		// Stop rrServer
		public static void StopStreamingServer()
		{
			if (isStreamingServerExist())
			{
				try
				{
					UnityEngine.Debug.Log("Stop Streaming Server.");
					myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
					myProcess.StartInfo.Arguments = "/c taskkill /F /IM RRServer.exe";
					myProcess.Start();
				}
				catch (Exception e)
				{
					UnityEngine.Debug.LogError(e);
				}
			}
			else
			{
				// dpServer is not found
				UnityEngine.Debug.LogError("Streaming server is not found, please update full package from https://developer.vive.com/resources/knowledgebase/wave-sdk/");
			}
		}
	}
}
#endif