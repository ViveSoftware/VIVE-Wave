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

[RequireComponent(typeof(Text))]
public class DoFDetector : MonoBehaviour {
	private Text textField;

	void Awake()
	{
		textField = GetComponent<Text>();
	}

	// Use this for initialization
	void Start() {
		#if UNITY_EDITOR
		if (Application.isEditor) {
			return;
		}
		#endif

		int dof = MainMenu.is6DoFTracking();
		if (dof > 0)
			textField.text = dof.ToString() + "DoF";
		else
			textField.text = "";
	}
}
