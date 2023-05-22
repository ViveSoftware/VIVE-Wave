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
using System.Collections.Generic;
using UnityEngine;
using Wave.Native;

namespace AQDR
{
	public class QualityContactHandler : MonoBehaviour, IColliderHandler
	{
		static readonly string TAG = "AQDRQCH";
		[SerializeField]
		List<GameObject> qualityGOList = new List<GameObject>();

		public AssetBundleLoader_LoadAll sceneLoader = null;

		public int QualityLevelStartIndex = 0;
		public int QualityLevelEndIndex = 0;
		public int nextQualityLevel = 0;

		public GameObject indicator;


		/**
		 * In Wave XRSDK, you can only change quality level between the same 
		 * MSAA level.  If changing to different MSAA level, the application
		 * will crash.
		 * 
		 * Since Unity 2019.4, the MSAA need be enabled by texture created 
		 * with flag, "AutoResolve", in XRSDK.  However it will cause the MSAA
		 * level can't be changed by QualitySettings.  This may be an Unity's
		 * issue.
		**/
		public void ChangeQuality()
		{
			Log.d(TAG, "SetQualityLevel(" + nextQualityLevel + ")");
			QualitySettings.SetQualityLevel(nextQualityLevel, true);

			// Currently we don't need this.
			//Wave.Essence.RenderFunctions.NotifyQualityLevelChange();
		}

		void OnEnable()
		{
			if (!sceneLoader)
				Log.w(TAG, "Need sceneLoader to know if we are loading.");
			ColliderDetect.RegisterHandler(this);
		}

		void OnDisable()
		{
			ColliderDetect.UnregisterHandler(this);
		}

		private void Start()
		{
			nextQualityLevel = QualitySettings.GetQualityLevel();

			string[] names = QualitySettings.names;

			int qlCount = names.Length;
			int goCount = qualityGOList.Count;
			if (goCount > qlCount)
			{
				QualityLevelStartIndex = 0;
				QualityLevelEndIndex = qlCount - 1;
				for (int i = 0; i < goCount; i++)
				{
					if (i >= qlCount)
						qualityGOList[i].SetActive(false);// GetComponent<MeshRenderer>().enabled = false;
				}
			}
			else
			{
				// qlCount > goCount
				QualityLevelStartIndex = qlCount - goCount;
				QualityLevelEndIndex = goCount - 1;
			}

			var sb = Log.CSB;
			sb.Append("QualityLevel names: ");
			for (int i = 0; i < qlCount; i++)
			{
				if (i > 0)
					sb.Append("  ");
				sb.Append(i).Append(" ").Append(names[i]);
			}
			Log.d(TAG, sb.ToString());

			ParticleControl(false);
		}

		public void IndicatorControl(ContactPoint cp)
		{
			if (indicator == null)
				return;

			indicator.transform.position = cp.point;
			indicator.transform.rotation = Quaternion.FromToRotation(Vector3.forward, cp.normal);
		}

		public void ParticleControl(bool enable)
		{
			ParticleSystem particle = null;
			if (indicator != null)
				particle = indicator.GetComponentInChildren<ParticleSystem>();

			if (!particle)
				return;

			if (enable)
			{
				particle.Play();
			}
			else
			{
				particle.Stop();
			}
		}

		bool hasEntered = false;

		public void ProcessContactEnter(Collision collision, ContactPoint cp)
		{
			if (sceneLoader && sceneLoader.IsLoadingScenes)
			{
				Log.d(TAG, "Wait for all scenes loaded.");
				return;
			}

			// cp.other.gameobject will always be the Trigger
			int N = qualityGOList.Count;
			for (int i = 0; i < N; i++)
			{
				if (cp.thisCollider.gameObject == qualityGOList[i])
				{
					IndicatorControl(cp);
					ParticleControl(true);
					hasEntered = true;

					var next = i + QualityLevelStartIndex;
					if (next == nextQualityLevel)
						return;
					nextQualityLevel = next;

					Invoke("ChangeQuality", 1);  // Change QL after 1 second
					break;
				}
			}
		}

		public void ProcessContactExit(Collision collision)
		{
			// collision.gameobject will always be the Trigger
			if (hasEntered)
			{
				ParticleControl(false);
				hasEntered = false;
			}
		}
	}
}
