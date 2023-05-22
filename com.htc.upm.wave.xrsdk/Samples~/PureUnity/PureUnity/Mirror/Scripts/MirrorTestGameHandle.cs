using UnityEngine;

namespace Wave.Generic.Sample
{

	public class MirrorTestGameHandle : MonoBehaviour
	{
		public Transform trackingOrigin;
		public Transform position0;
		public Transform position1;
		public Transform position2;

		void Start()
		{
        
		}

		void Update()
		{
        
		}

		public void OnPosition0Clicked()
		{
			SetOriginPose(position0);
		}

		public void OnPosition1Clicked()
		{
			SetOriginPose(position1);
		}

		public void OnPosition2Clicked()
		{
			SetOriginPose(position2);
		}

		void SetOriginPose(Transform pose)
		{
			trackingOrigin.position = pose.position;
			trackingOrigin.rotation = pose.rotation;
		}

		public void OnQuitClicked()
		{
			var msm = MasterSceneManager.Instance;
			if (msm != null)
			msm.LoadPrevious();
		}
	}
}
