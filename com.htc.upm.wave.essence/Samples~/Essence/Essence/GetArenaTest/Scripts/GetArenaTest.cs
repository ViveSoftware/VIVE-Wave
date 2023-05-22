using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wave.Native;
using Wave.Essence.Events;

namespace Wave.Essence.Samples.GetArenaTest
{
	public class GetArenaTest : MonoBehaviour
	{
		private static string LOG_TAG = "GetArenaTest";

		private WVR_Arena_t arena;
		private Text textArenaChanged = null;
		private void OnEnable()
		{
			SystemEvent.Listen(WVR_EventType.WVR_EventType_ArenaChanged, ArenaChanged);
			textArenaChanged = GameObject.Find("ArenaChanedText").GetComponent<Text>();
			if (textArenaChanged == null)
				Log.i(LOG_TAG, "ArenaChanedText can't be found.");

		}
		// Start is called before the first frame update
		void Start()
		{
        
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void GetArena()
		{
			arena = Interop.WVR_GetArena();
			if (textArenaChanged != null)
			{
				textArenaChanged.text = "Arena Shape : " + arena.shape.ToString() + "\n" +
										"Width : " + (float)arena.area.rectangle.width + "\n" +"Length : " + (float)arena.area.rectangle.length + "\n" +"Diameter : " + (float)arena.area.round.diameter;
				Log.i(LOG_TAG, "Arena Shape : " + arena.shape.ToString() + " Width : " + (float)arena.area.rectangle.width + " Length : " + (float)arena.area.rectangle.length + " Diameter : " + (float)arena.area.round.diameter);
			}
		}

		void ArenaChanged(WVR_Event_t systemEvent)
		{
			if (textArenaChanged != null)
			{
				Log.i(LOG_TAG, "Arena Changed!");
				textArenaChanged.text = "Arena Changed!";
			}
		}

		private void OnDisable()
		{
			SystemEvent.Remove(WVR_EventType.WVR_EventType_ArenaChanged, ArenaChanged);
		}
	}
}

