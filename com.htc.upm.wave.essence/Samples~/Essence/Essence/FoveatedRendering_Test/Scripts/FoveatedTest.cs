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
using UnityEngine.UI;
using Wave.Native;
using Wave.Essence;


public class FoveatedTest : MonoBehaviour
{
	WaveXR_FoveatedRendering foveated;
	//private static string LOG_TAG = "FoveatedTest";
	public Text StatustextField;
	private static FoveatedTest mInstance;
	private float FOVLarge = 57;
	private float FOVMiddle = 38;
	private float FOVSmall = 19;
	public GameObject ObjectFar;
	public GameObject ObjectNear;
	private bool changeStatus = true;
	//float time;

	public void FoveationIsDisable()
	{
		if (foveated.enabled)
		{
			foveated.enabled = false;
			changeStatus = true;
		}
	}

	public void FoveationIsEnable()
	{
		if (!foveated.enabled)
		{
			foveated.SetDynamic = false;
			foveated.enabled = true;
			changeStatus = true;
		}
		else if (foveated.SetDynamic)
		{
			foveated.enabled = false;
			foveated.SetDynamic = false;
			foveated.enabled = true;
			changeStatus = true;
		}
	}

	public void FoveationIsEnableDynamic()
	{
		if (!foveated.enabled)
		{
			foveated.SetDynamic = true;
			foveated.enabled = true;
			changeStatus = true;
		}
		else if (!foveated.SetDynamic)
		{
			foveated.enabled = false;
			foveated.SetDynamic = true;
			foveated.enabled = true;
			changeStatus = true;
		}
	}

	public void LeftClearVisionFOVHigh()
	{
		if (foveated.enabled)
		{
			if (foveated.LeftClearVisionFOV != this.FOVLarge)
			{
				foveated.LeftClearVisionFOV = this.FOVLarge;
				changeStatus = true;
			}
		}
	}

	public void LeftClearVisionFOVLow()
	{
		if (foveated.enabled)
		{
			if (foveated.LeftClearVisionFOV != this.FOVSmall)
			{
				foveated.LeftClearVisionFOV = this.FOVSmall;
				changeStatus = true;
			}
		}
	}

	public void LeftClearVisionFOVMiddle()
	{
		if (foveated.enabled)
		{
			if (foveated.LeftClearVisionFOV != this.FOVMiddle)
			{
				foveated.LeftClearVisionFOV = this.FOVMiddle;
				changeStatus = true;
			}
		}
	}

	public void LeftEyePeripheralQualityHigh()
	{
		if (foveated.enabled)
		{
			if (foveated.LeftPeripheralQuality != WVR_PeripheralQuality.High)
			{
				foveated.LeftPeripheralQuality = WVR_PeripheralQuality.High;
				changeStatus = true;
			}
		}
	}

	public void LeftEyePeripheralQualityLow()
	{
		if (foveated.enabled)
		{
			if (foveated.LeftPeripheralQuality != WVR_PeripheralQuality.Low)
			{
				foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Low;
				changeStatus = true;
			}
		}
	}

	public void LeftEyePeripheralQualityMiddle()
	{
		if (foveated.enabled)
		{
			if (foveated.LeftPeripheralQuality != WVR_PeripheralQuality.Middle)
			{
				foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Middle;
				changeStatus = true;
			}
		}
	}

	public void RightClearVisionFOVHigh()
	{
		if (foveated.enabled)
		{
			if (foveated.RightClearVisionFOV != this.FOVLarge)
			{
				foveated.RightClearVisionFOV = this.FOVLarge;
				changeStatus = true;
			}
		}
	}

	public void RightClearVisionFOVLow()
	{
		if (foveated.enabled)
		{
			if (foveated.RightClearVisionFOV != this.FOVSmall)
			{
				foveated.RightClearVisionFOV = this.FOVSmall;
				changeStatus = true;
			}
		}
	}

	public void RightClearVisionFOVMiddle()
	{
		if (foveated.enabled)
		{
			if (foveated.RightClearVisionFOV != this.FOVMiddle)
			{
				foveated.RightClearVisionFOV = this.FOVMiddle;
				changeStatus = true;
			}
		}
	}

	public void RightEyePeripheralQualityHigh()
	{
		if (foveated.enabled)
		{
			if (foveated.RightPeripheralQuality != WVR_PeripheralQuality.High)
			{
				foveated.RightPeripheralQuality = WVR_PeripheralQuality.High;
				changeStatus = true;
			}
		}
	}

	public void RightEyePeripheralQualityLow()
	{
		if (foveated.enabled)
		{
			if (foveated.RightPeripheralQuality != WVR_PeripheralQuality.Low)
			{
				foveated.RightPeripheralQuality = WVR_PeripheralQuality.Low;
				changeStatus = true;
			}
		}
	}

	public void RightEyePeripheralQualityMedium()
	{
		if (foveated.enabled)
		{
			if (foveated.RightPeripheralQuality != WVR_PeripheralQuality.Middle)
			{
				foveated.RightPeripheralQuality = WVR_PeripheralQuality.Middle;
				changeStatus = true;
			}
		}
	}

	private void printFoveationInfo()
	{
		string str = string.Empty;
		if (!foveated.enabled)
		{
			str = "Foveation enable : " + foveated.enabled.ToString();
		}
		else
		{
			string[] textArray1 = new string[12];
			textArray1[0] = "Foveation enable : ";
			textArray1[1] = foveated.enabled.ToString();
			textArray1[2] = "\n LeftClearVisionFOV : ";
			textArray1[3] = foveated.LeftClearVisionFOV.ToString();
			textArray1[4] = "\n RightClearVisionFOV : ";
			textArray1[5] = foveated.RightClearVisionFOV.ToString();
			textArray1[6] = "\n LeftPeripheralQuality : ";
			textArray1[7] = foveated.LeftPeripheralQuality.ToString();
			textArray1[8] = "\n RightPeripheralQuality : ";
			textArray1[9] = foveated.RightPeripheralQuality.ToString();
			textArray1[10] = "\n Foveation dynamic : ";
			textArray1[11] = foveated.SetDynamic.ToString();
			str = string.Concat(textArray1);
			//Log.d(LOG_TAG, "foveation_type_text: " + str, false);
		}
		this.StatustextField.text = str;
	}

	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (foveated == null)
		{
			foveated = WaveXR_FoveatedRendering.Instance;
			if (!foveated)
			{
				return;
			}
		}
		if (changeStatus == true)
		{
			this.printFoveationInfo();
			changeStatus = false;
		}
	}
}
