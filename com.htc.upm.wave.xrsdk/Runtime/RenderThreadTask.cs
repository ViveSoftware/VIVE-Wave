using AOT;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace Wave.XR
{
	#region syncObject
	public class Message
	{
		public bool isFree = true;
	}

	public class MessagePool
	{
		private readonly List<Message> pool = new List<Message>(2) { };
		private int index = 0;

		public MessagePool() { }

		private int Next(int value)
		{
			if (++value >= pool.Count)
				value = 0;
			return value;
		}

		public T Obtain<T>() where T : Message, new()
		{
			int c = pool.Count;
			int i = index;
			for (int j = 0; j < c; i++, j++)
			{
				if (i >= c)
					i = 0;
				if (pool[i].isFree)
				{
					//Debug.LogError("Obtain idx=" + i);
					index = i;
					return (T)pool[i];
				}
			}
			index = Next(i);
			var newItem = new T()
			{
				isFree = true
			};
			pool.Insert(index, newItem);
			//Debug.LogError("Obtain new one.  Pool.Count=" + pool.Count);
			return newItem;
		}

		public void Lock(Message msg)
		{
			msg.isFree = false;
		}

		public void Release(Message msg)
		{
			msg.isFree = true;
		}
	}

	public class PreAllocatedQueue : MessagePool
	{
		private readonly List<Message> list = new List<Message>(2) { null, null };
		private int queueBegin = 0;
		private int queueEnd = 0;

		public PreAllocatedQueue() : base() { }

		private int Next(int value)
		{
			if (++value >= list.Count)
				value = 0;
			return value;
		}

		public void Enqueue(Message msg)
		{
			Lock(msg);
			queueEnd = Next(queueEnd);

			if (queueEnd == queueBegin)
			{
				list.Insert(queueEnd, msg);
				queueBegin++;
			}
			else
			{
				list[queueEnd] = msg;
			}
		}

		public Message Dequeue()
		{
			queueBegin = Next(queueBegin);
			return list[queueBegin];
		}
	}

	// Run a lambda/delegate code in RenderThread
	public class RenderThreadTask
	{
		// In Windows, Marshal.GetFunctionPointerForDelegate() will cause application hang
		private static IntPtr GetFunctionPointerForDelegate(Delegate del)
		{
#if UNITY_EDITOR && !UNITY_2021_3
			// Older version will hang after run script in render thread.
			return IntPtr.Zero;
#else
			return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(del);
#endif
		}

		public delegate void RenderEventDelegate(int e);
		private static readonly RenderEventDelegate handle = new RenderEventDelegate(RunSyncObjectInRenderThread);
		private static readonly IntPtr handlePtr = GetFunctionPointerForDelegate(handle);

		public delegate void Receiver(PreAllocatedQueue dataQueue);

		private static List<RenderThreadTask> CommandList = new List<RenderThreadTask>();

		private readonly PreAllocatedQueue queue = new PreAllocatedQueue();
		public PreAllocatedQueue Queue { get { return queue; } }

		private readonly Receiver receiver;
		private readonly int id;

		public RenderThreadTask(Receiver render)
		{
			receiver = render;
			if (receiver == null)
				throw new ArgumentNullException("receiver should not be null");

			CommandList.Add(this);
			id = CommandList.IndexOf(this);
		}

		~RenderThreadTask()
		{
			try { CommandList.RemoveAt(id); } finally { }
		}

		void IssuePluginEvent(IntPtr callback, int eventID)
		{
#if UNITY_EDITOR && !UNITY_2021_3
			// Older version will hang after run script in render thread.
			if (Application.isEditor)
			{
				receiver(queue);
				return;
			}
#endif

#if UNITY_ANDROID
			// Older version will hang after run script in render thread.
			GL.IssuePluginEvent(callback, eventID);
			return;
#else
			receiver(queue);
			return;
#endif
		}

		void IssuePluginEvent(CommandBuffer cmdBuf, IntPtr callback, int eventID)
		{
#if UNITY_EDITOR && !UNITY_2021_3
			if (Application.isEditor)
				throw new NotImplementedException("Should not use this function in Windows");
#endif

			cmdBuf.IssuePluginEvent(callback, eventID);
			return;
		}

		// Run in GameThread
		public void IssueEvent()
		{
			// Let the render thread run the RunSyncObjectInRenderThread(id)
			IssuePluginEvent(handlePtr, id);
		}

		public void IssueInCommandBuffer(CommandBuffer cmdBuf)
		{
			// Let the render thread run the RunSyncObjectInRenderThread(id)
			IssuePluginEvent(cmdBuf, handlePtr, id);
		}

		// Called by RunSyncObjectInRenderThread()
		private void Receive()
		{
			receiver(queue);
		}

		[MonoPInvokeCallback(typeof(RenderEventDelegate))]
		private static void RunSyncObjectInRenderThread(int id)
		{
			CommandList[id].Receive();
		}
	}
#endregion
}
