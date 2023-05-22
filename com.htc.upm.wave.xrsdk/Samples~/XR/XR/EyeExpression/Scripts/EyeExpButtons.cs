// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wave.XR.Sample.EyeExp
{
	public class EyeExpButtons : MonoBehaviour
	{
		const string LOG_TAG = "Wave.XR.Sample.EyeExp.EyeExpButtons";
		public void ExitGame()
		{
			Debug.Log(LOG_TAG + " ExitGame()");
			Application.Quit();
		}

		public void BackToUpLayer()
		{
			Debug.Log(LOG_TAG + "BackToUpLayer()");
			SceneManager.LoadScene(0);
		}
	}
}
