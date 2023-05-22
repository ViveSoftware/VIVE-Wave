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
using UnityEngine;
using Wave.Essence;
using Wave.Native;

public class PermissionMgr : MonoBehaviour {
	private static string LOG_TAG = "PermissionMgr";

	private PermissionManager pmInstance = null;
	private static bool isDeny = false;
	private static int retryCount = 0;
	private static int RETRY_LIMIT = 0;
	private static bool requested = false;
	private static int systemCheckFailCount = 0;

	public static void requestDoneCallback(List<PermissionManager.RequestResult> results)
	{
		Log.d(LOG_TAG, "requestDoneCallback, count = " + results.Count);
		isDeny = false;

		foreach (PermissionManager.RequestResult p in results)
		{
			Log.d(LOG_TAG, "requestDoneCallback " + p.PermissionName + ": " + (p.Granted ? "Granted" : "Denied"));
			if (!p.Granted)
			{
				isDeny = true;
			}
		}

		if (isDeny)
		{
			if (retryCount++ < RETRY_LIMIT)
			{
				Log.d(LOG_TAG, "Permission denied, retry count = " + retryCount);
				requested = false;
			} else
			{
				Log.w(LOG_TAG, "Permission denied, exceed RETRY_LIMIT and skip request");
			}
		}
	}

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
		if (Application.isEditor) return;
#endif
		Log.i(LOG_TAG, "get instance at start");
		pmInstance = PermissionManager.instance;
		requested = false;
		retryCount = 0;
	}

	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		if (Application.isEditor) return;
#endif
		if (!requested)
		{
			if (systemCheckFailCount <= 10)
			{
				if (pmInstance.isInitialized())
				{
					Log.d(LOG_TAG, "inited");
					Log.d(LOG_TAG, "showDialogOnScene() = " + pmInstance.showDialogOnScene());
					string[] tmpStr =
					{
						"android.permission.CAMERA", "android.permission.READ_EXTERNAL_STORAGE", "android.permission.WRITE_EXTERNAL_STORAGE"
					};

					Log.d(LOG_TAG, "isPermissionGranted(android.permission.CAMERA) = " + pmInstance.isPermissionGranted("android.permission.CAMERA"));
					Log.d(LOG_TAG, "isPermissionGranted(android.permission.WRITE_EXTERNAL_STORAGE) = " + pmInstance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE"));
					Log.d(LOG_TAG, "shouldGrantPermission(android.permission.READ_EXTERNAL_STORAGE) = " + pmInstance.shouldGrantPermission("android.permission.READ_EXTERNAL_STORAGE"));

					pmInstance.requestPermissions(tmpStr, requestDoneCallback);
					requested = true;
				}
				else
				{
					systemCheckFailCount++;
				}
			}
		}
	}
}
