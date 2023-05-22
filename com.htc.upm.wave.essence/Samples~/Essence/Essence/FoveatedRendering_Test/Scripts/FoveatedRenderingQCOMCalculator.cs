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
	public class FoveatedRenderingQCOMCalculator : MonoBehaviour
	{
		public float displayFOV;

		[Range(1, 179)]
		public float clearVisionFOV;

		[Range(2, 179)]
		public float lowBoundFOV;

		[Range(1, 32, order = 1)]
		public uint minScale;

		public bool clickMeToUpdate = true;

		void Start()
		{
			clickMeToUpdate = true;
			var camera = GetComponent<Camera>();
			displayFOV = camera.fieldOfView;
		}

		void Update()
		{
			if (!clickMeToUpdate)
				return;

			var camera = GetComponent<Camera>();

			displayFOV = camera.fieldOfView;
			float g, w;
			var enable = calculateFoveated(displayFOV, clearVisionFOV, lowBoundFOV, minScale, out g, out w);
			Debug.Log("QCOM gain = " + g + ", area = " + w + ", enable = " + enable);

			clickMeToUpdate = false;
		}

		static bool calculateFoveated(float displayFOV, float clearVisionFOV, float lowBoundFOV, uint minScale, out float g, out float w)
		{
			bool enable = true;
			if (minScale <= 1)
				enable = false;
			if (clearVisionFOV > lowBoundFOV || clearVisionFOV > 179 || clearVisionFOV < 1 || lowBoundFOV > 179 || lowBoundFOV < 2 || displayFOV < 1 || displayFOV > 179)
			{
				enable = false;
			}

			var clearVisionNDC = fov2NDC(displayFOV, clearVisionFOV);
			var lowBoundNDC = fov2NDC(displayFOV, lowBoundFOV);

			Debug.Log("NDC clearVision = " + clearVisionNDC + ", lowBound = " + lowBoundNDC);

			invertQCOMFoveatedScaleFormula(clearVisionNDC, lowBoundNDC, minScale, out g, out w);
			return enable;
		}

		static float fov2NDC(float displayFOV, float inputFOV)
		{
			var halfDisplayFOV = displayFOV / 2;
			var halfInputFOV = inputFOV / 2;
			var tanDisplay = Mathf.Tan(halfDisplayFOV * Mathf.Deg2Rad);
			var tan = Mathf.Tan(halfInputFOV * Mathf.Deg2Rad);

			return tan / tanDisplay;
		}

		static void invertQCOMFoveatedScaleFormula(float clearVisionNDC, float lowBoundFOVNDC, uint scale, out float g, out float w)
		{
			// Set the minScale to native SDK API

			// QCOM formula x^2 * g^2 - w = s
			// clearVision =>  w = xClear^2 * gsqr - 1	 ....(1)
			// lowBound	=>  w = xLow^2 * gsqr - s	   ....(2)
			// (2) - (1)   =>  0 = (xLow^2 - xClear^2) * gsqr -1 + s
			//			 =>  gsqr = (1 - s) / (xLow^2 - xClear^2)
			//			 =>  g = sqrt((1 - s) / (xLow^2 - xClear^2))   ... (Answer 1)
			//			 =>  w = xClear^2 * gsqr - 1				   ... (Answer 2)

			var xClearSqr = clearVisionNDC * clearVisionNDC;
			var xLowSqr = lowBoundFOVNDC * lowBoundFOVNDC;
			var diff = xLowSqr - xClearSqr;

			// clearVision should smaller than lowBound
			if (diff < Mathf.Epsilon)
				diff = Mathf.Epsilon;
			var gsqr = (scale - 1) / diff;
			g = Mathf.Sqrt(gsqr);
			w = xClearSqr * gsqr - 1;
		}
	}
}