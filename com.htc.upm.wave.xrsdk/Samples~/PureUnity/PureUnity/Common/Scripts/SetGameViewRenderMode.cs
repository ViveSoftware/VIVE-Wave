using UnityEngine;
using UnityEngine.XR;

public class SetGameViewRenderMode : MonoBehaviour
{
	public GameViewRenderMode gameViewRenderMode = GameViewRenderMode.BothEyes;

	void Start()
	{
		XRSettings.gameViewRenderMode = gameViewRenderMode;
	}

	void Validate()
	{
		XRSettings.gameViewRenderMode = gameViewRenderMode;
	}
}
