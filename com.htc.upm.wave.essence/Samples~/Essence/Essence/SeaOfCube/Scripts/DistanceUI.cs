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

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class DistanceUI : MonoBehaviour {
	public static DistanceUI Instance;
	public string text { get; set; }
	private UnityEngine.UI.Text ui;

	void Start () {
		Instance = this;
		ui = GetComponent<UnityEngine.UI.Text>();
	}
	
	void OnGUI () {
		ui.text = text;
	}
}
