// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

#pragma warning disable 0219
#pragma warning disable 0414

using Wave.Native;

namespace Wave.Essence
{
	public class IMEManagerWrapper
	{
		private static string LOG_TAG = "IMEManagerWrapper";
		private static int VERSION_ID = 3;

		private static IMEManager mIMEManager = null;
		private static IMEManagerWrapper mInstance = null;
		private IMEManager.IMEParameter mParameter = null;
		private static InputDoneCallback mDoneCallback = null;
		private static InputClickedCallback mClickedCallback = null;

		//private static int CONTROLLER_BUTTON_MIN = 0;
		//private static int CONTROLLER_BUTTON_MENU = 1;
		//private static int CONTROLLER_BUTTON_GRIP = 2;
		//private static int CONTROLLER_BUTTON_VOLUME_UP = 4;
		//private static int CONTROLLER_BUTTON_VOLUME_DOWN = 8;
		//private static int CONTROLLER_BUTTON_TOUCH_PAD = 16;
		//private static int CONTROLLER_BUTTON_TRIGGER = 32;
		//private static int CONTROLLER_BUTTON_DIGITALTRIGGER = 64;
		//private static int CONTROLLER_BUTTON_ENTER = 128;
		private static int CONTROLLER_BUTTON_DEFAULT = 240;
		//private static int CONTROLLER_BUTTON_MAX = 255;

		public enum Locale
		{
			en_US = 0,
			zh_CN = 1,
		};

		public enum Action
		{
			Done = 0,
			Enter = 1,
			Search = 2,
			Go = 3,
			Send = 4,
			Next = 5,
			Submit = 6,
		};

		private IMEManagerWrapper()
		{
			InitParameter();
		}

		public static IMEManagerWrapper GetInstance()
		{
			if (mInstance == null || mIMEManager == null)
			{
				mInstance = new IMEManagerWrapper();
				mIMEManager = IMEManager.instance;
				mIMEManager.isInitialized();
			}

			Log.d(LOG_TAG, "VERSION_ID=" + VERSION_ID);

			return mInstance;
		}

		public void SetText(string text)
		{
			mParameter.exist = text;
		}

		public void SetTitle(string title)
		{
			mParameter.title = title;
		}

		public void SetLocale(Locale locale)
		{
			Log.d(LOG_TAG, "SetLocale, locale = " + locale);
			if (locale == Locale.en_US)
			{
				mParameter.locale = "en_US";
			}
			else if (locale == Locale.zh_CN)
			{
				mParameter.locale = "zh_CN";
			}
			else
			{
				mParameter.locale = "";
			}
		}

		public void SetDoneCallback(InputDoneCallback callback)
		{
			mDoneCallback = callback;
		}

		public void SetClickedCallback(InputClickedCallback callback)
		{
			mClickedCallback = callback;
		}

		public void Show()
		{
			mIMEManager.showKeyboard(mParameter, inputDoneCallback);
		}
        public void Show(bool isEnableEditorPanel)
        {
            mIMEManager.showKeyboard(mParameter, isEnableEditorPanel, inputDoneCallback, inputClickedCallback);
        }

        public void Hide()
		{
			mIMEManager.hideKeyboard();
		}

		public void SetAction(Action action)
		{
			mParameter.extraString = "action=" + (int)action;
		}

		public void InitParameter()
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
			double[] pos = new double[] { 0, 0, -1 };
			double[] rot = new double[] { 1.0, 0.0, 0.0, 0.0 };
			int width = 800;
			int height = 800;
			int shadow = 100;
			string locale = "";
			string title = "";
			int extraInt = 0;
			string extraString = "";
			int buttonId = CONTROLLER_BUTTON_DEFAULT;
			mParameter = new IMEManager.IMEParameter(id, type, mode, exist, cursor, selectStart, selectEnd, pos,
				rot, width, height, shadow, locale, title, extraInt, extraString, buttonId);
		}

		public delegate void InputDoneCallback(InputResult results);

		public delegate void InputClickedCallback(InputResult results);

		public class InputResult
		{
			private string mContent;
			private IMEManager.InputResult.Key mKeyCode;

			public InputResult(string content, IMEManager.InputResult.Key key)
			{
				mContent = content;
				mKeyCode = key;
			}

			public InputResult(string content)
			{
				mContent = content;				
			}

			public string GetContent()
			{
				return mContent;
			}

			public IMEManager.InputResult.Key GetKeyCode()
			{
				return mKeyCode;
			}
		}

		private void inputDoneCallback(IMEManager.InputResult results)
		{
			if (mDoneCallback != null)
			{
				InputResult inputResult = new InputResult(results.InputContent);
				mDoneCallback(inputResult);
			}
		}

		private void inputClickedCallback(IMEManager.InputResult results)
		{
			if (mClickedCallback != null)
			{
				InputResult inputResult = new InputResult(results.InputContent, results.KeyCode);
				mClickedCallback(inputResult);
			}
		}
	}
}
