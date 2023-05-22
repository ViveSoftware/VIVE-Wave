// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour {
	public int ReloadSceneEveryNSecond = 20;
	public List<string> scenes = new List<string>();
	private static LoadLevel instance = null;

	void Start()
	{
		if (instance != null)
			return;
		instance = this;

		if (ReloadSceneEveryNSecond > 0)
		{
			StartCoroutine("updateCountDown");
			//Invoke("nextScene", ReloadSceneEveryNSecond);
		}
		GameObject.DontDestroyOnLoad(gameObject);
	}

	IEnumerator updateCountDown()
	{
		var oneSecond = new WaitForSeconds(1);
		Text [] texts = FindObjectsOfType<UnityEngine.UI.Text>();
		Text text = null;
		foreach (Text t in texts) {
			if (t.name == "ReloadSceneCountDown")
			{
				text = t;
				break;
			}
		}
		do
		{
			for (int i = 0; i < ReloadSceneEveryNSecond; i++)
			{
				var left = ReloadSceneEveryNSecond - i;
				string msg = "Load next level after " + left + "s.";
				yield return oneSecond;
				left--;
				if (text != null)
					text.text = msg;
				Debug.Log(msg);
				if (left <= 0)
					nextScene();
			}
		} while (true);
	}

	void nextScene()
	{
		Scene s = SceneManager.GetActiveScene();
		int found = -1;
		for (int i = 0; i < scenes.Count; i++)
		{
			var sceneName = scenes[i];
			if (s.name.Contains(sceneName))
			{
				if (++i == scenes.Count)
					i = 0;
				found = i;
				break;
			}
		}
		if (found == -1)
		{
			// load self
			Debug.Log("LoadScene(" + s.name + ")");
			SceneManager.LoadScene(s.name);
		}
		else
		{
			// load next scene
			Debug.Log("LoadScene(" + scenes[found] + ")");
			SceneManager.LoadScene(scenes[found]);
		}
	}
}
