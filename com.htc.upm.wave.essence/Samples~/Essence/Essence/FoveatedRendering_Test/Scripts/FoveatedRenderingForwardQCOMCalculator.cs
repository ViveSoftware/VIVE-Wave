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

namespace wvr.sample.foveated.qcom
{
	public class FoveatedRenderingForwardQCOMCalculator : MonoBehaviour
	{
		public enum Profile
		{
			Min = 4,
			Middle = 7,
			Max = 10
		}

		public void drawCircle(LineRenderer lineRenderer, float radius, int segment)
		{
			int count = lineRenderer.positionCount;
			lineRenderer.positionCount += segment + 1;
			int idx = count;
			int N = segment + 1;
			float step = 2 * Mathf.PI / segment;
			for (int i = 0; i < N; i++)
			{
				float theta = step * i;
				float x = radius * Mathf.Cos(theta);
				float y = radius * Mathf.Sin(theta);

				Vector3 pos = new Vector3(x, y, 0);
				lineRenderer.SetPosition(idx++, pos);
			}
		}

		public Camera cam = null;

		[Range(1, 179)]
		public float displayFOV = 93.01f;  // Focus Vive

		public Profile gain = Profile.Max;
		public float area = 1;
		public float[] FOV;

		public bool clickMeToUpdate = true;

		void Start()
		{
			clickMeToUpdate = true;
			FOV = new float[5];
		}

		void Update()
		{
			if (!clickMeToUpdate)
				return;

			if (cam != null)
				displayFOV = cam.fieldOfView;

			float tanDisplay = Mathf.Tan(Mathf.Deg2Rad * displayFOV / 2);

			float circleDistance = 99.7f;
			var circleWidth = 0.01f * circleDistance;
			var color = Color.red;

			var lineRenderer = GetComponentInChildren<LineRenderer>();
			if (lineRenderer != null)
			{
				var o = lineRenderer.gameObject;
				lineRenderer = null;
				GameObject.Destroy(o);
			}
			string name = "LineRenderer";
			var obj = new GameObject(name);
			obj.transform.SetParent(this.transform, false);
			obj.transform.localPosition = new Vector3(0, 0, circleDistance);

			lineRenderer = obj.AddComponent<LineRenderer>();
			lineRenderer.positionCount = 0;
			lineRenderer.material = new Material(Shader.Find("Unlit/Transparent"));
			lineRenderer.startColor = color;
			lineRenderer.endColor = color;
			lineRenderer.startWidth = circleWidth;
			lineRenderer.endWidth = circleWidth;
			lineRenderer.loop = true;
			lineRenderer.useWorldSpace = false;
			lineRenderer.receiveShadows = false;
			lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;


			// Forward
			for (int i = 0; i < FOV.Length; i++)
			{
				float ndc;
				uint scale = (uint)Mathf.Pow(2, i);  // 1, 2, 4, 8, 16
				Debug.Log(scale);
				QCOMFoveatedScaleFormula(scale, (float)gain, area, out ndc);
				FOV[i] = Mathf.Atan(ndc * tanDisplay) * 2 * Mathf.Rad2Deg;

				var circleRadius = circleDistance * ndc * tanDisplay;
				drawCircle(lineRenderer, circleRadius, 40);
			}

			clickMeToUpdate = false;
		}

		static void QCOMFoveatedScaleFormula(uint scale, float g, float w, out float ndc)
		{
			// QCOM formula x^2 * g^2 - w = s
			// => ndc^2 = (s + w) / gsqr

			float gsqr = g * g;
			ndc = Mathf.Sqrt((scale + w) / gsqr);
		}
	}
}
