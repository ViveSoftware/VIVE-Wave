// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using AssetBundleTool;
using UnityEngine;
using Wave.Essence.Render;

namespace AQDR
{
	public class DRContactHandler : MonoBehaviour, IColliderHandler
	{
		//static readonly string TAG = "AQDRDRCH";
		[SerializeField]
		GameObject drHigher = null;
		[SerializeField]
		GameObject drLower = null;

		[SerializeField]
		DynamicResolution dr = null;

		void OnEnable()
		{
			ColliderDetect.RegisterHandler(this);
		}

		void OnDisable()
		{
			ColliderDetect.UnregisterHandler(this);
		}

		float debounceTimeHigher = 0;
		float debounceTimeLower = 0;

		public void ProcessContactEnter(Collision collision, ContactPoint cp)
		{
			if (cp.thisCollider.gameObject == drHigher)
			{
				if (Time.unscaledTime - debounceTimeHigher > 0.5f)
				{
					debounceTimeHigher = Time.unscaledTime;
					dr.Higher();
				}
			}
			else if (cp.thisCollider.gameObject == drLower)
			{
				if (Time.unscaledTime - debounceTimeLower > 0.5f)
				{
					debounceTimeLower = Time.unscaledTime;
					dr.Lower();
				}
			}
		}
		public void ProcessContactExit(Collision collision) { }
	}
}
