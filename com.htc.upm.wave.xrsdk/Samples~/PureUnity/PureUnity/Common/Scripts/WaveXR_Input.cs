using UnityEngine;
using UnityEngine.EventSystems;

namespace Wave.XR.Sample
{
	public class WaveXR_Input : BaseInput
	{
		public override bool mousePresent
		{
			get { return true; }
		}

		protected override void Awake()
		{
			StandaloneInputModule standaloneInputModule = FindObjectOfType<StandaloneInputModule>();
			if (standaloneInputModule) standaloneInputModule.inputOverride = this;
		}

		public override Vector2 mouseScrollDelta
		{
			get { return Vector2.zero; }
		}

		public override Vector2 mousePosition
		{
			get { return new Vector2(Camera.main.scaledPixelWidth * 0.5f, Camera.main.scaledPixelHeight * 0.5f); }
		}

		public override bool GetMouseButton(int button)
		{
			if (button != 0)
				return WXRInput.GetMouseButton(button);

			return WXRInput.GetMouseButton(button) || WXRInput.GetButton("Button0") || WXRInput.GetButton("Button2") || WXRInput.GetButton("Button8") || WXRInput.GetButton("Button9");
		}

		public override bool GetMouseButtonDown(int button)
		{
			if (button != 0)
				return WXRInput.GetMouseButtonDown(button);

			return WXRInput.GetMouseButtonDown(button) || WXRInput.GetButtonDown("Button0") || WXRInput.GetButtonDown("Button2") || WXRInput.GetButtonDown("Button8") || WXRInput.GetButtonDown("Button9");
		}

		public override bool GetMouseButtonUp(int button)
		{
			if (button != 0)
				return WXRInput.GetMouseButtonDown(button);

			return WXRInput.GetMouseButtonUp(button) || WXRInput.GetButtonUp("Button0") || WXRInput.GetButtonUp("Button2") || WXRInput.GetButtonUp("Button8") || WXRInput.GetButtonUp("Button9");
		}

		public override bool GetButtonDown(string buttonName)
		{
			//if (buttonName == "Submit")
			//	return WXRInput.GetButtonDown("Button0") || WXRInput.GetButtonDown("Button2") || WXRInput.GetButtonDown("Button8") || WXRInput.GetButtonDown("Button9");
			//if (buttonName == "Cancel")
			//	return WXRInput.GetButtonDown("Button1") || WXRInput.GetButtonDown("Button3") || WXRInput.GetButtonDown("Button14") || WXRInput.GetButtonDown("Button15");
			return false;
		}

		public override bool touchSupported
		{
			get { return false; }
		}

		public override float GetAxisRaw(string axisName)
		{
			return 0;
		}
	}
}
