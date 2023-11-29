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
using UnityEngine;

using Wave.Native;
using Wave.XR.Function;
#if UNITY_EDITOR
using Wave.Essence.Editor;
#endif

namespace Wave.Essence.Events
{
	public class SystemEvent
    {
        public delegate void Handler(WVR_Event_t wvrEvent);

        public static void Listen(Handler action, bool removedWhenException = false)
        {
            WaveVR_SystemEvent.CheckInstance();

            // Only one listener can be registered.
            if (allEventListeners.Contains(action) || allEventListenersSticky.Contains(action))
            {
                Log.w("Event",
                    Log.CSB
                    .AppendLine("Skip a duplicated listener from here:")
                    .Append(new System.Diagnostics.StackTrace(false).ToString())
                    .ToString());
                return;
            }
            if (removedWhenException)
                allEventListeners.Add(action);
            else
                allEventListenersSticky.Add(action);
        }

        public static void Listen(WVR_EventType eventType, Handler action, bool removedWhenException = false)
        {
            WaveVR_SystemEvent.CheckInstance();
            List<Handler> handlerList = null;
            List<Handler> handlerListSticky = null;

            listeners.TryGetValue(eventType, out handlerList);
            listenersSticky.TryGetValue(eventType, out handlerListSticky);

            bool exist = handlerList == null ? false : handlerList.Contains(action);
            bool existSticky = handlerListSticky == null ? false : handlerListSticky.Contains(action);

            if (exist || existSticky)
            {
                Log.w("Event",
                    Log.CSB
                    .AppendLine("Skip a duplicated listener from here:")
                    .Append(new System.Diagnostics.StackTrace(false).ToString())
                    .ToString());
                return;
            }

            var targetList = removedWhenException ? listeners : listenersSticky;
            var targetHandlerList = removedWhenException ? handlerList : handlerListSticky;
            if (targetHandlerList == null)
            {
                targetHandlerList = new List<Handler>();
                targetList[eventType] = targetHandlerList;
            }

            targetHandlerList.Add(action);
        }

        public static void Remove(Handler action)
        {
            if (allEventListeners.Contains(action))
                allEventListeners.Remove(action);
            else if (allEventListenersSticky.Contains(action))
                allEventListenersSticky.Remove(action);
        }

        public static void Remove(WVR_EventType eventType, Handler action)
        {
            List<Handler> handlerList = null;
            List<Handler> handlerListSticky = null;

            listeners.TryGetValue(eventType, out handlerList);
            listenersSticky.TryGetValue(eventType, out handlerListSticky);

            bool exist = handlerList == null ? false : handlerList.Contains(action);
            bool existSticky = handlerListSticky == null ? false : handlerListSticky.Contains(action);

            if (!exist && !existSticky)
                return;

            if (exist)
                handlerList.Remove(action);
            if (existSticky)
                handlerListSticky.Remove(action);
        }

        public static void Send(WVR_Event_t wvrEvent)
        {
            List<Handler> handlerList = null;
            listeners.TryGetValue(wvrEvent.common.type, out handlerList);
            int N = 0;
            if (handlerList != null)
            {
                N = handlerList.Count;
                for (int i = N - 1; i >= 0; i--)
                {
                    Handler single = handlerList[i];
                    try
                    {
                        single(wvrEvent);
                    }
                    catch (Exception e)
                    {
                        Log.e("Event", e.ToString(), true);
                        handlerList.Remove(single);
                        Log.e("Event", "A listener is removed due to exception.", true);
                    }
                }
            }

            handlerList = null;
            listenersSticky.TryGetValue(wvrEvent.common.type, out handlerList);
            if (handlerList != null)
            {
                N = handlerList.Count;
                for (int i = N - 1; i >= 0; i--)
                {
                    Handler single = handlerList[i];
                    try
                    {
                        single(wvrEvent);
                    }
                    catch (Exception e)
                    {
                        Log.e("Event", e.ToString(), true);
                    }
                }
            }

            N = allEventListenersSticky.Count;
            for (int i = N - 1; i >= 0; i--)
            {
                var listener = allEventListenersSticky[i];
                try
                {
                    listener(wvrEvent);
                }
                catch (Exception e)
                {
                    Log.e("Event", e.ToString(), true);
                }
            }

            N = allEventListeners.Count;
            for (int i = N - 1; i >= 0; i--)
            {
                var listener = allEventListeners[i];
                try
                {
                    listener(wvrEvent);
                }
                catch (Exception e)
                {
                    Log.e("Event", e.ToString(), true);
                    allEventListeners.Remove(listener);
                    Log.e("Event", "A listener is removed due to exception.", true);
                }
            }
        }

        private static Dictionary<WVR_EventType, List<Handler>> listeners = new Dictionary<WVR_EventType, List<Handler>>();
        private static Dictionary<WVR_EventType, List<Handler>> listenersSticky = new Dictionary<WVR_EventType, List<Handler>>();
        private static List<Handler> allEventListeners = new List<Handler>();
        private static List<Handler> allEventListenersSticky = new List<Handler>();
        // Start is called before the first frame update
    }

    public class WaveVR_SystemEvent : MonoBehaviour
    {
        public static WaveVR_SystemEvent instance = null;
        public static void CheckInstance()
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("Wave.Essence.SystemEvent");
                instance = obj.AddComponent<WaveVR_SystemEvent>();
#if UNITY_EDITOR
                WaveEditor.AssertInstance();
#endif
            }
        }

        delegate int PollSystemFrameEventDelegate(int handle, ref WVR_Event_t wvrEvent);
        static PollSystemFrameEventDelegate PollSystemFrameEvent = null;

        private void OnEnable()
        {
            if (PollSystemFrameEvent == null)
                PollSystemFrameEvent = FunctionsHelper.GetFuncPtr<PollSystemFrameEventDelegate>("PollFrameEvent");
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void Update()
        {
            // Only one can poll and send the event.
            if (instance != this) return;

            if (PollSystemFrameEvent == null)
            {
                Log.e("WVRSystemEvent", "PollSystemFrameEvent == null");
                return;
            }

            int handle = 0;
            WVR_Event_t wvrEvent = new WVR_Event_t();
            do
            {
                handle = PollSystemFrameEvent(handle, ref wvrEvent);
                if (handle == 0)
                    break;
                //Log.d("WVRSystemEvent", "Got Event" + wvrEvent.common.type.ToString());
                SystemEvent.Send(wvrEvent);
            } while (handle != 0);
        }
    }
}
