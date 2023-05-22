// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;

namespace Wave.Essence
{
	public static class ActionEventHub
	{
		private static readonly List<IActionEvent> s_ActionEvents = new List<IActionEvent>();

		public static void AddAction(IActionEvent action)
		{
			if (s_ActionEvents.Contains(action))
				return;

			s_ActionEvents.Add(action);
		}
		public static List<IActionEvent> GetActions()
		{
			return s_ActionEvents;
		}

		public static void RemoveAction(IActionEvent action)
		{
			if (!s_ActionEvents.Contains(action)) { return; }
			s_ActionEvents.Remove(action);
		}
	}

	public static class ActionEvent
	{
		public static bool Processing(XR_Device device)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.Device)
				{
					if (value == (uint)device)
					{
						if (actions[i].processing)
							return true;
					}
				}
			}
			return false;
		}
		public static bool Processing(XR_Hand hand)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.Hand)
				{
					if (value == (uint)hand)
					{
						if (actions[i].processing)
							return true;
					}
				}
			}
			return false;
		}
		public static bool Processing(XR_HandDevice handDevice)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.HandDevice)
				{
					if (value == (uint)handDevice)
					{
						if (actions[i].processing)
							return true;
					}
				}
			}
			return false;
		}
		public static bool Selected(XR_Device device)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.Device)
				{
					if (value == (uint)device)
					{
						if (actions[i].selected)
							return true;
					}
				}
			}
			return false;
		}
		public static bool Selected(XR_Hand hand)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.Hand)
				{
					if (value == (uint)hand)
					{
						if (actions[i].selected)
							return true;
					}
				}
			}
			return false;
		}
		public static bool Selected(XR_HandDevice handDevice)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.HandDevice)
				{
					if (value == (uint)handDevice)
					{
						if (actions[i].selected)
							return true;
					}
				}
			}
			return false;
		}
		public static bool Griped(XR_Device device)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.Device)
				{
					if (value == (uint)device)
					{
						if (actions[i].griped)
							return true;
					}
				}
			}
			return false;
		}
		public static bool Griped(XR_Hand hand)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.Hand)
				{
					if (value == (uint)hand)
					{
						if (actions[i].griped)
							return true;
					}
				}
			}
			return false;
		}
		public static bool Griped(XR_HandDevice handDevice)
		{
			List<IActionEvent> actions = ActionEventHub.GetActions();
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i].GetActionTaker(out uint value) == IActionEvent.ActionTaker.HandDevice)
				{
					if (value == (uint)handDevice)
					{
						if (actions[i].griped)
							return true;
					}
				}
			}
			return false;
		}
	}
}