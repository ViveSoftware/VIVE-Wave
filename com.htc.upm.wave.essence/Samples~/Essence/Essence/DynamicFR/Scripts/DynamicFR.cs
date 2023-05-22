using UnityEngine;
using UnityEngine.XR;
using Wave.Essence;
using Wave.Native;

public class DynamicFR : MonoBehaviour
{
	InputDevice centerEye;

	WaveXR_FoveatedRendering foveated;

	public float AngularVelocity { get; private set; }
	public float Velocity { get; private set; }

	public int FOV { get; private set; }
	public WVR_PeripheralQuality Quality { get; private set; }
	public float RS { get; private set; }
	public float VS { get; private set; }
	public float MotionScale { get; private set; }

	public float AVWeight = 2.0f / 3;
	public float VFWeight = 0.5f;
	public float VBWeight = 0.5f;
	public float VZWeight = 0.5f;

	public float FRWeight = 1;
	public float RSWeight = 0; // 1 / 5.0f;
	public float VSWeight = 0; // 1 / 5.0f;

	[Range(1.0f, 179.0f)]
	public float FOV_Narrow = 38.0f;
	[Range(1.0f, 179.0f)]
	public float FOV_Wide = 55.0f;

	void Start()
	{
		foveated = WaveXR_FoveatedRendering.Instance;

		centerEye = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		if (centerEye == default)
		{
			enabled = false;
			return;
		}

		AngularVelocity = 0;
		Velocity = 0;
		MotionScale = 0;
		FOV = 55;
		Quality = (WVR_PeripheralQuality)2;
		RS = 1;
		VS = 1;
		foveated.Set(FOV, Quality);
		foveated.Apply();
		XRSettings.eyeTextureResolutionScale = 1;
	}

	WVR_PeripheralQuality MotionScaleToQuality(float motionScale)
	{
		return (WVR_PeripheralQuality)(2 - Mathf.Clamp(Mathf.FloorToInt(motionScale * FRWeight * 3f), 0, 2));
	}

	int MotionScaleToFOV(float motionScale)
	{
		return Mathf.Clamp(Mathf.FloorToInt((FOV_Wide - FOV_Narrow) * (1 - motionScale * FRWeight) + FOV_Narrow), (int)FOV_Narrow, (int)FOV_Wide);
	}

	float MotionScaleToRS(float motionScale)
	{
		// The output will be quantized with step 0.1f
		return Mathf.Clamp(Mathf.FloorToInt((1 - motionScale * RSWeight) * 11), 6, 10) / 10.0f;  // 0.6 ~ 1.0
	}

	float MotionScaleToVS(float motionScale)
	{
		// The output will be quantized with step 0.1f
		return Mathf.Clamp(Mathf.FloorToInt((1 - motionScale * VSWeight) * 11), 6, 10) / 10.0f;  // 0.6 ~ 1.0
	}

	void Update()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
			return;
#endif
		centerEye.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 angularVelocity);
		centerEye.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);

		AngularVelocity = angularVelocity.magnitude;
		Velocity = velocity.magnitude;

		var vz = velocity.z;
		var vxy = new Vector2(velocity.x, velocity.y);

		// Calculate Motion Scale by weight
		MotionScale =
			AngularVelocity * AVWeight +
			(vz > 0 ? (vz * VFWeight) : 0) +
			(vz < 0 ? (-vz * VBWeight) : 0) +
			vxy.magnitude * VZWeight +
			0;

		//Debug.Log("AVW"+ AVWeight + " VFW" + VFWeight + " VBW" + VBWeight + " VZW" + VZWeight + " MotionScale=" + MotionScale);

		// Should we do normalize?
		//float motionScaleNomolizer = AVWeight + Mathf.Max(VFWeight, VBWeight) + VZWeight;
		//MotionScale /= motionScaleNomolizer;

		MotionScale = Mathf.Clamp(MotionScale, 0.001f, 2.0f);

		Quality = MotionScaleToQuality(MotionScale);
		FOV = MotionScaleToFOV(MotionScale);
		RS = MotionScaleToRS(MotionScale);
		VS = MotionScaleToVS(MotionScale);

		foveated.Set(FOV, Quality);
		foveated.Apply();
		XRSettings.eyeTextureResolutionScale = RS;
		XRSettings.renderViewportScale = VS;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		FOV = 55;
		Quality = (WVR_PeripheralQuality)2;
		RS = 1;
		foveated.Set(FOV, Quality);
		foveated.Apply();
		XRSettings.eyeTextureResolutionScale = 1;
		XRSettings.renderViewportScale = 1;
	}

	private void OnValidate()
	{
		if (FOV_Wide <= FOV_Narrow)
		{
			FOV_Wide = FOV_Narrow + 1;
			FOV_Wide = Mathf.Clamp(FOV_Wide, 1, 179);
			if (FOV_Wide == FOV_Narrow)
				FOV_Narrow -= 1;
		}
	}
}
