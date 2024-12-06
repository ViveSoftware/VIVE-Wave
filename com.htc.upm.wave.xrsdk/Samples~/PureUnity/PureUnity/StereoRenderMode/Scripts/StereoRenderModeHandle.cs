using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

// Stresstest for StereoRenderMode

// 20241022 Try not update too frequently.
// Unity will fail to create texture if update too frequently. Then Unity close app itself.
// Logged in developer mode with Wave render native flag opened:
//  D TexturePool: UnityTexture created: c=0 d=0 u=634 w=1200 h=1200 q=1
//  D TexturePool: Texture[2] Query: c = 0 d = 0 u = 0  <---- This is the error, Unity failed to create texture
//  D Unity   : Error on graphics thread: 1" when create texture
public class StereoRenderModeHandle : MonoBehaviour
{
	XRDisplaySubsystem display;
	bool randomTest = false;

	public Text status = null;

	uint hasChange = 1;
	uint skipFrame = 3;
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
		if (accTime < 0.25f)
			return;

		accTime = 0;
		// Every 0.25s update the status.
		hasChange = 1;
	}

#if UNITY_2020_3_OR_NEWER
	StackTraceLogType originalSTLT;
#endif

	private void OnEnable()
	{
		//var display = Wave.XR.Sample.Utils.DisplaySubsystem;
		//if (display == null)
		//{
		//	Debug.LogError("XR Display subsystem is not exist");
		//	return;
		//}
#if UNITY_2020_3_OR_NEWER
		originalSTLT = Application.GetStackTraceLogType(LogType.Log);
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
#endif
	}


	private void OnDisable()
	{
#if UNITY_2020_3_OR_NEWER
		Application.SetStackTraceLogType(LogType.Log, originalSTLT);
#endif
	}

	public void RandomTest()
	{
		if (display == null) return;
		int random = Random.Range(0, 97);
		
		// Avoid crash if change too frequently.
		if (hasChange > 0) return;

		switch (random % 8)
		{
			case 0:
				// SinglePass
				Debug.Log("StereoRenderMode: Random: SinglePass");
#if UNITY_2020_3_OR_NEWER
				display.textureLayout = XRDisplaySubsystem.TextureLayout.Texture2DArray;
#else
				display.singlePassRenderingDisabled = false;
#endif
				break;
			case 1:
				// MultiPass
				Debug.Log("StereoRenderMode: Random: MultiPass");
#if UNITY_2020_3_OR_NEWER
				display.textureLayout = XRDisplaySubsystem.TextureLayout.SeparateTexture2Ds;
#else
				display.singlePassRenderingDisabled = true;
#endif
				break;
			case 2:
				Debug.Log("StereoRenderMode: Random: RS=1");
				XRSettings.eyeTextureResolutionScale = 1;
				break;
			case 3:
				Debug.Log("StereoRenderMode: Random: RS=0.75");
				XRSettings.eyeTextureResolutionScale = 0.75f;
				break;
			case 4:
				Debug.Log("StereoRenderMode: Random: RS=0.5");
				XRSettings.eyeTextureResolutionScale = 0.5f;
				break;
			default:
				// 567 Do nothing
				break;
		}
		hasChange = skipFrame;
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
			sb.Append("StereoRenderMode: Status");

			int rpc = display.GetRenderPassCount();
			sb.AppendLine().Append("RenderPassCount=").Append(rpc);
			for (int i = 0; i < rpc; i++)
			{
				display.GetRenderPass(i, out XRDisplaySubsystem.XRRenderPass pass);
				sb.AppendLine().Append("RP").Append(i).Append(".RenderParamCount=").Append(pass.GetRenderParameterCount());
			}
#if UNITY_2020_3_OR_NEWER
			if (display.textureLayout == XRDisplaySubsystem.TextureLayout.Texture2DArray)
				sb.AppendLine().Append("RenderMode=SinglePass");
			else
				sb.AppendLine().Append("RenderMode=MultiPass");
#else
			sb.AppendLine().Append("RenderMode=").Append(display.singlePassRenderingDisabled ? "MultiPass" : "SinglePass");
#endif
			sb.AppendLine().Append("StereoRenderingMode=").Append(XRSettings.stereoRenderingMode);
			sb.AppendLine().Append("ResolutionScale=").Append(XRSettings.eyeTextureResolutionScale);
			sb.AppendLine().Append("FPS=").Append(Mathf.RoundToInt(fps));
			status.text = sb.ToString();
		}

		if (hasChange != 0)
			hasChange--;
	}

	public void OnSinglePassButtonPressed()
	{
		if (display == null) return;
		Debug.Log("StereoRenderMode: Manual: SinglePass");
#if UNITY_2020_1_OR_NEWER
		display.textureLayout = XRDisplaySubsystem.TextureLayout.Texture2DArray;
#else
		display.singlePassRenderingDisabled = false;
#endif
		hasChange = skipFrame;
	}

	public void OnMultiPassButtonPressed()
	{
		if (display == null) return;
		Debug.Log("StereoRenderMode: Manual: MultiPass");
#if UNITY_2020_1_OR_NEWER
		display.textureLayout = XRDisplaySubsystem.TextureLayout.SeparateTexture2Ds;
#else
		display.singlePassRenderingDisabled = true;
#endif
		hasChange = skipFrame;
	}

	public void OnResolutionScale1Pressed()
	{
		if (display == null) return;
		Debug.Log("StereoRenderMode: Manual: RS=1");
		XRSettings.eyeTextureResolutionScale = 1;
		hasChange = skipFrame;
	}

	public void OnResolutionScale075Pressed()
	{
		if (display == null) return;
		Debug.Log("StereoRenderMode: Manual: RS=0.75");
		XRSettings.eyeTextureResolutionScale = 0.75f;
		hasChange = skipFrame;
	}

	public void OnResolutionScale05Pressed()
	{
		if (display == null) return;
		Debug.Log("StereoRenderMode: Manual: RS=0.5");
		XRSettings.eyeTextureResolutionScale = 0.5f;
		hasChange = skipFrame;
	}

	public void OnRandomTestTogglePressed()
	{
		if (display == null) return;
		randomTest = !randomTest;
		if (randomTest)
		{
			Debug.Log("StereoRenderMode: RandomTest: Start");
			hasChange = skipFrame;
		}
		else
			Debug.Log("StereoRenderMode: RandomTest: Stop");
	}
}
