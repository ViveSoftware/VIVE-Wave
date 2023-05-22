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
using Wave.Essence;

namespace Wave.Sample
{
	public class FoveatedIndicator : MonoBehaviour
	{
		public static string billboardName = "FoveatedBillboard";
		private const int billboardCount = 1;

		// Just for debug.  Don't assign value.
		public WaveXR_FoveatedRendering foveated = null;
		public GameObject billboard = null;
		public GameObject[] billboards = null;
		public Material[] indicatorMaterials = null;

		public void Start()
		{
			billboards = null;
			indicatorMaterials = null;
			billboard = null;
		}

		public void Update()
		{
			if (foveated == null)
			{
				foveated = WaveXR_FoveatedRendering.Instance;
				if (!foveated)
					return;
			}

			if (!foveated.enabled)
			{
				RemoveIndicator();
				return;
			}

			if (foveated.TrackedObject == null)
			{
				RemoveIndicator();
				return;
			}

			if (billboard == null || indicatorMaterials == null || foveated.TrackedObject != billboard)
				CreateIndicator(foveated.TrackedObject);

			var averageFOV = (foveated.LeftClearVisionFOV + foveated.RightClearVisionFOV) / 2;
			for (int i = 0; i < billboardCount; i++)
			{
				var fov = (averageFOV + 20 * i);
				indicatorMaterials[i].SetFloat("_TangentOfHalfFov", Mathf.Tan(Mathf.Deg2Rad * fov / 2));
			}
		}

		public void RemoveIndicator()
		{
			if (billboard != null)
			{
				billboard.SetActive(false);
			}
		}

		public void CreateIndicator(GameObject root)
		{
			if (billboard != null)
			{
				billboard.SetActive(true);
			}
			else
			{
				billboard = new GameObject(billboardName);
				billboards = new GameObject[billboardCount];
				for (int i = 0; i < billboardCount; i++)
				{
					billboards[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
					billboards[i].name = billboardName + i;
					billboards[i].transform.SetParent(billboard.transform, false);
				}
			}

			billboard.transform.SetParent(root.transform, false);

			if (indicatorMaterials == null)
			{
				var shader = Shader.Find("WaveVR/FixedFOVBillboards");
				var texture = Resources.Load<Texture>("crosshair_NoCenter_512");

				indicatorMaterials = new Material[billboardCount];
				for (int i = 0; i < billboardCount; i++)
				{
					indicatorMaterials[i] = new Material(shader);
					indicatorMaterials[i].SetTexture("_MainTex", texture);
					indicatorMaterials[i].renderQueue = indicatorMaterials[i].renderQueue + 10;
				}
			}

			for (int i = 0; i < billboardCount; i++)
			{
				var renderer = billboards[i].GetComponent<MeshRenderer>();
				renderer.material = indicatorMaterials[i];
			}
		}
	}
}
