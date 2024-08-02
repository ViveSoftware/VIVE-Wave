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
using UnityEngine;

#if UNITY_EDITOR
namespace Wave.Essence.Editor
{
	[Serializable]
	public class PreferenceAvatarAsset : ScriptableObject
	{
		public const string AssetPath = "Assets/Wave/Essence/Preferences/PreferenceAvatarAsset.asset";

		// VRM constants
		public const string kVrm0Package = "UniVRM-0.109.0_7aff.unitypackage";
		public const string kVrm0Asset = "Assets/VRM.meta";
		public const string kVrm1Package = "VRM-0.109.0_7aff.unitypackage";
		public const string kVrm1Asset = "Assets/VRM10.meta";

		// Body Tracking constants
		const string WaveEssencePath = "Assets/Wave/Essence";
		public const string kBodyTrackingAsset = WaveEssencePath + "/BodyTracking.meta";
		public const string kFacialExpressionMakerAsset = WaveEssencePath + "/FacialExpression/Maker.meta";

		public bool SupportVrm0 = false;
		public bool SupportVrm1 = false;
	}
}
#endif
