using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Wave.Essence;
using Wave.Native;

public class IMETest_Manager : MonoBehaviour
{
	public bool isShowingKeyboard = false;
	public bool isIMEManagerInitialized = false;
	public InputField onInputCompletedwithObjectResultText, onInputClickedwithObjectResultText;
	private string onInputCompletedwithObjectResultTextContent, onInputClickedwithObjectResultTextContent;
	private IMEManager imeManagerInstance = null;
	private IMEManager.IMEParameter currentIMEParameter = null;
	private IMEManager.IMEParameter currentIMENumericParameter = null;
	private static string LOG_TAG = "IMETest";
	private StringBuilder onInputClickedSB = new StringBuilder();
	private const string onInputCompletedPlaceHolder = "onInputCompleted";
	private const string onInputClickedPlaceHolder = "onInputClicked";
	// Start is called before the first frame update
	void Start()
	{
		imeManagerInstance = IMEManager.instance;
		isIMEManagerInitialized = imeManagerInstance.isInitialized();
		isShowingKeyboard = false;
		initParameter();
	}

	private void initParameter()
	{
		int MODE_FLAG_FIX_MOTION = 0x02;
		//int MODE_FLAG_AUTO_FIT_CAMERA = 0x04;
		int id = 0;
		int type = MODE_FLAG_FIX_MOTION;
		int mode = 2;

		string exist = "";
		int cursor = 0;
		int selectStart = 0;
		int selectEnd = 0;
		double[] pos = new double[] { 0, 0, -1.05 };
		double[] rot = null;
		int width = 800;
		int height = 800;
		int shadow = 100;
		string locale = "en_US";
		string localeForNumeric = "numeric";
		string title = "IMETest";
		int extraInt = 0;
		string extraString = "";
		int buttonId = (1 << (int)WVR_InputId.WVR_InputId_Alias1_Thumbstick) 
			| (1 << (int)WVR_InputId.WVR_InputId_Alias1_Touchpad) 
			| (1 << (int)WVR_InputId.WVR_InputId_Alias1_Trigger)
			| (1 << (int)WVR_InputId.WVR_InputId_Alias1_Bumper);
		currentIMEParameter = new IMEManager.IMEParameter(id, type, mode, exist, cursor, selectStart, selectEnd, pos,
							 rot, width, height, shadow, locale, title, extraInt, extraString, buttonId);

		currentIMENumericParameter = new IMEManager.IMEParameter(id, type, mode, exist, cursor, selectStart, selectEnd, pos,
							 rot, width, height, shadow, localeForNumeric, title, extraInt, extraString, buttonId);
		//currentIMENumericParameter = new IMEManager.IMEParameter(localeForNumeric,buttonId);
	}

	public void ShowKeyboard()
	{
		if (!isShowingKeyboard && isIMEManagerInitialized)
		{
			Log.i(LOG_TAG, "ShowKeyboard: done");
			imeManagerInstance.showKeyboard(currentIMEParameter, onInputCompletedwithObjectCallback);
			isShowingKeyboard = true;
		}
	}

	public void ShowKeyboardWithEnablePanel()
	{
		if (!isShowingKeyboard && isIMEManagerInitialized)
		{
			Log.i(LOG_TAG, "ShowKeyboard enable panel: done");
			imeManagerInstance.showKeyboard(currentIMEParameter, true, onInputCompletedwithObjectCallback, onInputClickedwithObjectCallback);
			isShowingKeyboard = true;
		}
	}

	public void ShowKeyboardWithDisablePanel()
	{
		if (!isShowingKeyboard && isIMEManagerInitialized)
		{
			Log.i(LOG_TAG, "ShowKeyboard disable panel: done");
			imeManagerInstance.showKeyboard(currentIMEParameter, false, onInputCompletedwithObjectCallback, onInputClickedwithObjectCallback);
			isShowingKeyboard = true;
		}
	}

	public void ShowNumericKeyKeypad()
	{
		if (!isShowingKeyboard && isIMEManagerInitialized)
		{
			Log.i(LOG_TAG, "ShowNumericKeyKeypad: Placeholder");
			imeManagerInstance.showKeyboard(currentIMENumericParameter, false, onInputCompletedwithObjectCallback, onInputClickedwithObjectCallback);
			isShowingKeyboard = true;
		}
	}

	public void ClearTextFields()
	{
		if (onInputCompletedwithObjectResultText != null)
		{
			onInputCompletedwithObjectResultTextContent = "";
			onInputCompletedwithObjectResultText.text = onInputCompletedwithObjectResultTextContent;
		}

		if (onInputClickedwithObjectResultText != null)
		{
			onInputClickedSB.Clear();
			onInputClickedwithObjectResultTextContent = "";
			onInputClickedwithObjectResultText.text = onInputClickedwithObjectResultTextContent;
		}
	}

	void LateUpdate()
	{
		if (onInputCompletedwithObjectResultTextContent != null && onInputCompletedwithObjectResultTextContent.Length > 0)
		{
			onInputCompletedwithObjectResultText.text = onInputCompletedwithObjectResultTextContent;
			onInputCompletedwithObjectResultTextContent = null;
		}


		if (onInputClickedwithObjectResultTextContent != null)
		{
			onInputClickedSB.Append(onInputClickedwithObjectResultTextContent);
			onInputClickedwithObjectResultText.text = onInputClickedSB.ToString();
			onInputClickedwithObjectResultTextContent = null;
		}
	}

	public void onInputCompletedwithObjectCallback(IMEManager.InputResult results)
	{
		Log.d(LOG_TAG, "onInputCompletedwithObjectCallback(" + results.Id + ") :" + results.InputContent);
		onInputCompletedwithObjectResultTextContent = results.InputContent;
		isShowingKeyboard = false;
	}

	public void onInputClickedwithObjectCallback(IMEManager.InputResult results)
	{
		Log.d(LOG_TAG, "onInputClickedwithObjectCallback(" + results.Id + ") :" + results.InputContent);
		onInputClickedwithObjectResultTextContent = results.InputContent;
		if (results.KeyCode == IMEManager.InputResult.Key.BACKSPACE)
		{
			onInputClickedSB.Clear();
			if (onInputClickedwithObjectResultText.text.Length > 1)
			{
				onInputClickedwithObjectResultTextContent = onInputClickedwithObjectResultText.text.Substring(0, onInputClickedwithObjectResultText.text.Length - 1);
			}
			else if (onInputClickedwithObjectResultText.text.Length == 1)
			{
				onInputClickedSB.Clear();
				onInputClickedwithObjectResultTextContent = "";
			}

		}
		if (results.KeyCode == IMEManager.InputResult.Key.ENTER)
		{
			Log.d(LOG_TAG, "on clicked enter key");
			isShowingKeyboard = false;
			imeManagerInstance.hideKeyboard();
		}
		if (results.KeyCode == IMEManager.InputResult.Key.CLOSE)
		{
			Log.d(LOG_TAG, "on clicked close key");
			isShowingKeyboard = false;
		}

	}
}
