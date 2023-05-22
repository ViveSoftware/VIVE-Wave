// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.UI;
using Wave.Native;
using Wave.Essence.Sample;

public class MainMenu : MonoBehaviour
{
	private static string LOG_TAG = "MainMenu";
	private WaveVR_Resource wvrRes = null;
	//WaveVR_Render.RenderThreadSynchronizer synchronizer;
	void Awake()
	{
#if UNITY_EDITOR
		if (Application.isEditor) return;
#endif
		//synchronizer = new WaveVR_Render.RenderThreadSynchronizer();
	}

	public static int is6DoFTracking()
	{
		WVR_NumDoF dof = Interop.WVR_GetDegreeOfFreedom(WVR_DeviceType.WVR_DeviceType_HMD);

		if (dof == WVR_NumDoF.WVR_NumDoF_6DoF)
			return 6;  // 6 DoF
		else if (dof == WVR_NumDoF.WVR_NumDoF_3DoF)
			return 3;  // 3 DoF
		else
			return 0;  // abnormal case
	}

	void Start()
	{
#if UNITY_EDITOR
		if (Application.isEditor) return;
#endif

		wvrRes = WaveVR_Resource.instance;

		if (wvrRes == null) {
			 Log.w(LOG_TAG, "Failed to initial WaveVR Resource instance!");
		} else {
			 string lang = wvrRes.getSystemLanguage();
			 string country = wvrRes.getSystemCountry();
			 Log.d(LOG_TAG, "system default language is " + lang);
			 Log.d(LOG_TAG, "system default country is " + country);

			 GameObject ht = GameObject.Find("SixDoFHText");
			 if (ht != null) {
				  Text htText = ht.GetComponent<Text>();
				  htText.text = wvrRes.getStringByLanguage("sixdof_head", lang, country);
			 } else {
				  Log.w(LOG_TAG, "Could not find 6dof head text game object!");
			 }

			 GameObject gt = GameObject.Find("SixDoFGText");
			 if (gt != null) {
				  Text gtText = gt.GetComponent<Text>();
				  gtText.text = wvrRes.getStringByLanguage("sixdof_ground", lang, country);
			 } else {
				  Log.w(LOG_TAG, "Could not find 6dof ground text game object!");
			 }

			 GameObject qt = GameObject.Find("QuitText");
			 if (qt != null) {
				  Text quitText = qt.GetComponent<Text>();
				  quitText.text = wvrRes.getStringByLanguage("exit", lang, country);
			 } else {
				  Log.w(LOG_TAG, "Could not find exit text game object!");
			 }
		}

		int dof = is6DoFTracking();
		Button Btn6DoFHead = GameObject.Find("Btn6DoFHead").GetComponent<Button>();
		Button Btn6DoFGround = GameObject.Find("Btn6DoFGround").GetComponent<Button>();

		switch(dof) {
			case 3:{
				if(Btn6DoFHead != null)
					Btn6DoFHead.interactable = false;
				if(Btn6DoFGround != null)
					Btn6DoFGround.interactable = false;
				}
				break;
			case 6: {
				if(Btn6DoFHead != null)
				   Btn6DoFHead.interactable = true;
				if(Btn6DoFGround != null)
				   Btn6DoFGround.interactable = true;
				}
				break;
			default:
				break;
		}
	}

	public void Choose6DoFGround()
	{
		Log.d(LOG_TAG, "6DoF Ground");
		ChooseDoF.whichHead = ChooseDoF.TrackingSpace.TS_6DOF_Ground;
		UnityEngine.SceneManagement.SceneManager.LoadScene("SeaOfCubeWithTwoHead");
	}

	public void Choose6DoFHead()
	{
		Log.d(LOG_TAG, "6DoF Head");
		ChooseDoF.whichHead = ChooseDoF.TrackingSpace.TS_6DOF_Head;
		UnityEngine.SceneManagement.SceneManager.LoadScene("SeaOfCubeWithTwoHead");
	}

	public void Choose3DoF()
	{
		Log.d(LOG_TAG, "3DoF Head");
		ChooseDoF.whichHead = ChooseDoF.TrackingSpace.TS_3DOF;
		UnityEngine.SceneManagement.SceneManager.LoadScene("SeaOfCubeWithTwoHead");
	}

	public void ChooseQuit()
	{
		Log.d(LOG_TAG, "Quit Game");
		Application.Quit();
	}
}
