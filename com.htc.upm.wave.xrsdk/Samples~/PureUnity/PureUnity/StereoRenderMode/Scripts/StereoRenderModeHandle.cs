using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class StereoRenderModeHandle : MonoBehaviour
{
	XRDisplaySubsystem display;
	bool randomTest = false;

	public Text status = null;

	uint hasChange = 1;
	StringBuilder sb = new StringBuilder();

	private float fps = 75;
	private float accTime = 0;

	void Awake()
	{
		accTime = 0;
	}

	void LateUpdate()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		accTime += unscaledDeltaTime;

		// Avoid crash when timeScale is 0.
		if (unscaledDeltaTime == 0)
		{
			return;
		}

		float interp = unscaledDeltaTime / (0.5f + unscaledDeltaTime);
		float currentFPS = 1.0f / unscaledDeltaTime;
		fps = Mathf.Lerp(fps, currentFPS, interp);

		// Avoid update Canvas too frequently.
		if (accTime < 0.20f)
			return;

		accTime = 0;
		hasChange = 1;
	}

	private void OnEnable()
	{
		//var display = Wave.XR.Sample.Utils.DisplaySubsystem;
		//if (display == null)
		//{
		//	Debug.LogError("XR Display subsystem is not exist");
		//	return;
		//}
	}

	public void RandomTest()
	{
		if (display == null) return;
		int random = Random.Range(0, 97);
		switch (random % 7)
		{
			case 0:
				// SinglePass
				display.singlePassRenderingDisabled = false;
				break;
			case 1:
				// MultiPass
				display.singlePassRenderingDisabled = true;
				break;
			case 2:
				XRSettings.eyeTextureResolutionScale = 1;
				break;
			case 3:
				XRSettings.eyeTextureResolutionScale = 0.75f;
				break;
			case 4:
				XRSettings.eyeTextureResolutionScale = 0.5f;
				break;
			case 5:
			case 6:
				// Do nothing
				break;
		}
		hasChange = 3;
	}

	private void Update()
	{
		if (display == null)
			display = Wave.XR.Sample.Utils.DisplaySubsystem;

		if (display == null)
		{
			Debug.LogError("XR Display subsystem is not exist");
			return;
		}

		if (randomTest)
			RandomTest();

		if (status != null && hasChange > 0)
		{
			sb.Clear();
			sb.Append("Status");

			int rpc = display.GetRenderPassCount();
			sb.AppendLine().Append("RenderPassCount=").Append(rpc);
			for (int i = 0; i < rpc; i++)
			{
				display.GetRenderPass(i, out XRDisplaySubsystem.XRRenderPass pass);
				sb.AppendLine().Append("RP").Append(i).Append(".RenderParamCount=").Append(pass.GetRenderParameterCount());
			}
			sb.AppendLine().Append("RenderMode=").Append(display.singlePassRenderingDisabled ? "MultiPass" : "SinglePass");
			sb.AppendLine().Append("RenderMode=").Append(XRSettings.stereoRenderingMode);
			sb.AppendLine().Append("ResolutionScale=").Append(XRSettings.eyeTextureResolutionScale);
			sb.AppendLine().Append("FPS=").Append(Mathf.RoundToInt(fps));
			status.text = sb.ToString();
		}
		hasChange--;
	}

	public void OnSinglePassButtonPressed()
	{
		if (display == null) return;
#if UNITY_2020_1_OR_NEWER
		display.textureLayout = XRDisplaySubsystem.TextureLayout.Texture2DArray;
#else
		display.singlePassRenderingDisabled = false;
#endif
		hasChange = 3;
	}

	public void OnMultiPassButtonPressed()
	{
		if (display == null) return;
#if UNITY_2020_1_OR_NEWER
		display.textureLayout = XRDisplaySubsystem.TextureLayout.SeparateTexture2Ds;
#else
		display.singlePassRenderingDisabled = true;
#endif
		hasChange = 3;
	}

	public void OnResolutionScale1Pressed()
	{
		if (display == null) return;
		XRSettings.eyeTextureResolutionScale = 1;
		hasChange = 3;
	}

	public void OnResolutionScale075Pressed()
	{
		if (display == null) return;
		XRSettings.eyeTextureResolutionScale = 0.75f;
		hasChange = 3;
	}

	public void OnResolutionScale05Pressed()
	{
		if (display == null) return;
		XRSettings.eyeTextureResolutionScale = 0.5f;
		hasChange = 3;
	}

	public void OnRandomTestTogglePressed()
	{
		randomTest = !randomTest;
		hasChange = 3;
	}
}
