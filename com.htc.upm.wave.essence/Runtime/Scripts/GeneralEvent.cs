// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Collections.Generic;
using Wave.Native;

namespace Wave.Essence.Events
{
	public class GeneralEvent
	{
		const string LOG_TAG = "Wave.Essence.Events.GeneralEvent";

		public static readonly string INTERACTION_MODE_MANAGER_READY = "INTERACTION_MODE_MANAGER_READY";

		public delegate void Handler(params object[] args);

		public static void Listen(string message, Handler action)
		{
			List<Handler> handlerList = null;
			listeners.TryGetValue(message, out handlerList);
			if (handlerList == null)
			{
				handlerList = new List<Handler>();
				listeners[message] = handlerList;
			}
			else if (handlerList.Contains(action))
			{
				Log.w(LOG_TAG,
					Log.CSB
					.AppendLine("Skip a duplicated listener from here:")
					.Append(new System.Diagnostics.StackTrace(false).ToString())
					.ToString());
				return;
			}

			handlerList.Add(action);
		}

		public static void Remove(string message, Handler action)
		{
			List<Handler> handlerList = null;
			listeners.TryGetValue(message, out handlerList);
			if (handlerList == null)
				return;
			if (!handlerList.Contains(action))
				return;

			handlerList.Remove(action);
		}

		public static void Send(string message, params object[] args)
		{
			List<Handler> handlerList = null;
			listeners.TryGetValue(message, out handlerList);
			if (handlerList != null)
			{
				int N = handlerList.Count;
				for (int i = N - 1; i >= 0; i--)
				{
					Handler single = handlerList[i];
					try
					{
						single(args);
					}
					catch (Exception e)
					{
						Log.e(LOG_TAG, e.ToString(), true);
						handlerList.Remove(single);
						Log.e(LOG_TAG, "A listener is removed due to exception.", true);
					}
				}
			}
		}

		private static Dictionary<string, List<Handler>> listeners = new Dictionary<string, List<Handler>>();
	}
}
