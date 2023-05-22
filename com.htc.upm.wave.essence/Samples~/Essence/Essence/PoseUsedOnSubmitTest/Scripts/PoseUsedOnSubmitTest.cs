using UnityEngine;
using Wave.Native;

public class PoseUsedOnSubmitTest : MonoBehaviour
{
	bool hasPoseUsedOnSubmit = false;
	WVR_PoseState_t[] poseState = new WVR_PoseState_t[1] { new WVR_PoseState_t() };

	// Invoked when button is clicked
	public void OnTogglePoseClicked()
	{
		if (Wave.Essence.RenderFunctions.SetPoseUsedOnSubmit == null)
			return;

		hasPoseUsedOnSubmit = !hasPoseUsedOnSubmit;

		if (hasPoseUsedOnSubmit)
		{
			Interop.WVR_GetPoseState(WVR_DeviceType.WVR_DeviceType_HMD, WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround, 0, ref poseState[0]);
			Wave.Essence.RenderFunctions.SetPoseUsedOnSubmit(poseState);
		}
		else
		{
			Wave.Essence.RenderFunctions.SetPoseUsedOnSubmit(null);
		}
	}
}
