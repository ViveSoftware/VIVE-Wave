// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wave.Essence;
using Wave.Essence.Sample;
using Wave.Native;

public class ReloadSceneHandle : MonoBehaviour {

	private static string LOG_TAG = "ReloadSceneHandle";
	private bool bulletGeneratorState = false;
	private PermissionManager pmInstance = null;
	private WaveVR_Resource wvrRes = null;
	bool inited = false;
	private static int systemCheckFailCount = 0;

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

			 GameObject rt = GameObject.Find("ReloadText");
			 if (rt != null) {
				  Text reloadText = rt.GetComponent<Text>();
				  reloadText.text = wvrRes.getStringByLanguage("reload_scene", lang, country);
			 } else {
				  Log.w(LOG_TAG, "Could not find reload scene text game object!");
			 }

			 GameObject mt = GameObject.Find("MainText");
			 if (mt != null) {
				  Text mainText = mt.GetComponent<Text>();
				  mainText.text = wvrRes.getStringByLanguage("main_menu", lang, country);
			 } else {
				  Log.w(LOG_TAG, "Could not find main menu text game object!");
			 }

			 GameObject prt = GameObject.Find("PermReqText");
			 if (prt != null) {
				  Text permReqText = prt.GetComponent<Text>();
				  permReqText.text = wvrRes.getStringByLanguage("perm_req", lang, country);
			 } else {
				  Log.w(LOG_TAG, "Could not find perm req text game object!");
			 }

			 GameObject qt = GameObject.Find("QuitText");
			 if (qt != null) {
				  Text quitText = qt.GetComponent<Text>();
				  quitText.text = wvrRes.getStringByLanguage("exit", lang, country);
			 } else {
				  Log.w(LOG_TAG, "Could not find exit text game object!");
			 }
		}

		pmInstance = PermissionManager.instance;
		if (pmInstance != null) {
			StartCoroutine(checkPackageManagerStatus());
		}
	}

	IEnumerator checkPackageManagerStatus()
	{
		if (systemCheckFailCount < 10)
		{
			if (!pmInstance.isInitialized())
			{
				systemCheckFailCount++;
				yield return new WaitForSeconds(0.1f);
			} else
			{
				inited = true;
				yield break;
			}
		}

		inited = false;
		yield break;
	}

	public static void requestDoneCallback(List<PermissionManager.RequestResult> results)
	{
		Log.d(LOG_TAG, "requestDoneCallback, count = " + results.Count);
	}

	public void nextScene()
	{
		Scene s = SceneManager.GetActiveScene();
		SceneManager.LoadScene(s.name);
	}

	public void toggleBulletGenerator()
	{
		var roots = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (var obj in roots)
		{
			if (obj.name == "BodyByDoF")
			{
				bulletGeneratorState = !bulletGeneratorState;
				obj.GetComponentInChildren<BulletGenerator>().enabled = bulletGeneratorState;
				break;
			}
		}
	}

	public void loadMainMenu()
	{
		SceneManager.LoadScene("SeaOfCubeMain");
	}

	public void permRequest()
	{
		Log.d(LOG_TAG, "Permission Request");

		#if UNITY_EDITOR
		  if (Application.isEditor) return;
		#endif

		if (inited) {
			Log.d(LOG_TAG, "Permission Request action");
			string[] permArray = {
			   "android.permission.CAMERA", "android.permission.READ_EXTERNAL_STORAGE", "android.permission.WRITE_EXTERNAL_STORAGE"
			};
			pmInstance.requestPermissions (permArray, requestDoneCallback);
		}
	}

	public void quitGame()
	{
		Application.Quit();
	}

	private void disableClicking()
	{
		GameObject btn = GameObject.Find("BtnUtra");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = false;
		}
		btn = GameObject.Find("BtnHigh");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = false;
		}
		btn = GameObject.Find("BtnMedium");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = false;
		}
		btn = GameObject.Find("BtnLow");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = false;
		}
	}

	private void enableClicking()
	{
		GameObject btn = GameObject.Find("BtnUtra");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = true;
		}
		btn = GameObject.Find("BtnHigh");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = true;
		}
		btn = GameObject.Find("BtnMedium");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = true;
		}
		btn = GameObject.Find("BtnLow");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = true;
		}
	}

	private void hidePanel2()
	{
		GameObject obj = GameObject.Find("Panel2");
		obj.SetActive(false);
	}

	public void setQualityLevel(int level)
	{
		disableClicking();
		QualitySettings.SetQualityLevel(level);
		enableClicking();
		hidePanel2();
	}
}
