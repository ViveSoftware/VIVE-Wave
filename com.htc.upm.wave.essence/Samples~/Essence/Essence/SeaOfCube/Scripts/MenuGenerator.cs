// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGenerator : MonoBehaviour {
	public List<string> names;
	float horizontalBorder = 0.05f;
	float gap = 0.02f;

	// Use this for initialization
	void Start () {
		int count = names.Count;
		if (count <= 0)
			return;
		float height = (1.0f - gap * (count + 1)) / count;
		float accumulateHeight = 0;
		for (int i = 0; i < count; i++)
		{
			var go = new GameObject(names[i], typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
			go.transform.SetParent(this.transform, false);
			var rect = (RectTransform)go.transform;
			rect.pivot = new Vector2(0.5f, 0.5f);
			accumulateHeight += gap;
			rect.anchorMax = new Vector2(1 - horizontalBorder, 1 - accumulateHeight);
			accumulateHeight += height;
			rect.anchorMin = new Vector2(horizontalBorder, 1 - accumulateHeight);
			rect.offsetMin = new Vector2(0, 0);
			rect.offsetMax = new Vector2(0, 0);

			var txtGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
			txtGo.transform.SetParent(go.transform, false);
			var text = txtGo.GetComponent<Text>();
			text.text = names[i];
			text.resizeTextForBestFit = true;
			text.fontSize = 70;
			text.color = Color.black;

			rect = (RectTransform)txtGo.transform;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(1, 1);
			rect.anchorMin = new Vector2(0, 0);
			rect.offsetMin = new Vector2(0, 0);
			rect.offsetMax = new Vector2(0, 0);
		}
	}
}
