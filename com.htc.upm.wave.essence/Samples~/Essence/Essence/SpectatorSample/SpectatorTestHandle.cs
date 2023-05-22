using UnityEngine;
using UnityEngine.UI;
using Wave.Essence.Events;
using Wave.Generic.Sample;
using Wave.Native;

public class SpectatorTestHandle : MonoBehaviour
{
	public Text textStatus;
	public GameObject captured;
	float timeAcc = 0;
	bool started;

	private void OnEnable()
	{
		SystemEvent.Listen(EventHandler);
	}

	private void OnDisable()
	{
		SystemEvent.Remove(EventHandler);
	}

	public void OnBackClicked()
	{
		var msm = MasterSceneManager.Instance;
		if (!msm)
		{
			Application.Quit();
		}
		msm.LoadPrevious();
	}

	public void EventHandler(WVR_Event_t wvrEvent)
	{
		switch (wvrEvent.common.type)
		{
			case WVR_EventType.WVR_EventType_SpectatingStarted:
				started = true;
				break;
			case WVR_EventType.WVR_EventType_SpectatingStopped:
				started = false;
				break;
			default:
				break;
		}
	}


	void Update()
    {
		timeAcc += Time.unscaledDeltaTime;
		if (timeAcc < 0.2f)
			return;
		timeAcc = 0;
		UpdateSpectatorParameters();
	}

	float l, r, t, b;
	uint w, h;
	WVR_SpectatorState state;

	void UpdateSpectatorParameters()
	{
		Interop.WVR_GetSpectatorClippingPlaneBoundary(ref l, ref r, ref t, ref b);
		Interop.WVR_GetSpectatorRenderTargetSize(ref w, ref h);
		bool ret = Interop.WVR_PreSpectatorRender(ref state);

		var sb = Log.CSB.Clear();
		sb.Append("Spectator Status").AppendLine()
			.Append("Resolution: w=").Append(w).Append(", h=").Append(h)
			.Append("  FOV: l=").Append(l).Append(", r=").Append(r).Append(", t=").Append(t).Append(", b=").Append(b).AppendLine()
			.Append("Started=").Append(started).AppendLine()
			.Append("PreSpectatorRender ret=").Append(ret).Append(", shouldRender=").Append(state.shouldRender);

		captured.transform.localScale = new Vector3(w / (float)h, 1, 1);
		textStatus.text = sb.ToString();
	}
}
