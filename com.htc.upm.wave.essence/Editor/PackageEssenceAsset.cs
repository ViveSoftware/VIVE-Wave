using System;
using UnityEngine;

#if UNITY_EDITOR
namespace Wave.Essence.Editor
{
	[Serializable]
	public class PackageEssenceAsset : ScriptableObject
	{
		public bool importedControllerModelPackage = false;
		public bool importedInputModulePackage = false;
		public bool importedHandModelPackage = false;
		public bool importedInteractionModePackage = false;
		public bool importedTrackerModelPackage = false;
		public bool importedBodyTrackingPackage = false;
	}
}
#endif
