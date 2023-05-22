using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Wave.XR.Settings;
using Wave.XR.Render;
using Wave.Native;
using Wave.Generic.Sample;
using UnityEngine.SceneManagement;
using System.IO;

public class AMCGameHandle : MonoBehaviour
{
	[System.Serializable]
	public struct AMCSave
	{
		public AMCCubeData [] cubeData;
		public AMCCreatedObject[] objectData;
	}

	[System.Serializable]
	public struct AMCCubeData
	{
		// Position
		public float px, py, pz;
		// Quaternion
		public float qx, qy, qz, qw;
	}

	[System.Serializable]
	public struct AMCCreatedObject
	{
		public float angularVelocity;
		// Position
		public float px, py, pz;
		// Quaternion
		public float qx, qy, qz, qw;
		// Rand Quaternion
		public float rqx, rqy, rqz, rqw;
	}

	public struct AMCCreatedObjectRuntimeData
	{
		public GameObject obj;

		public float angularVelocity;
		// Position
		public Vector3 position;
		// Quaternion
		public Quaternion rotation;
		// Rand Quaternion
		public Quaternion randRotation;
	}

	// Rotated objects' origin
	public GameObject rotatedObjectOrigin = null;

	private List<AMCCreatedObjectRuntimeData> objects = new List<AMCCreatedObjectRuntimeData>();

	// Adapt random materials from the list
	public List<Material> materials = new List<Material>();
	// Adapt random meshes from the list
	public List<Mesh> meshes = new List<Mesh>();

	public bool increaseObject = false;
	public bool decreaseObject = false;
	public bool back = false;
	public bool reset = false;

	uint sleepGT = 0;
	uint sleepRT = 0;

	private float accTimeUpdate = 0;
	private float accTime3s = 0;
	private float accTime8s = 0;
	private int frameCount3s = 0;
	private int frameCount8s = 0;
	private float fps = 75;  // for instance
	private float avg3sFPS = 0;
	private float avg8sFPS = 0;

	[Tooltip("The choosed mode will be set at start.")]
	public WVR_AMCMode AMCMode = WVR_AMCMode.Auto;
	private WVR_AMCMode buildAMCMode = WVR_AMCMode.Auto;

	void Start()
	{
		Time.timeScale = 1.0f;
		Random.InitState(Mathf.CeilToInt(Time.unscaledTime * 100000));
	}

	private void OnEnable()
	{
		var settings = WaveXRSettings.GetInstance();
		if (settings != null)
			buildAMCMode = (WVR_AMCMode)settings.amcMode;
		else
			Debug.LogError("WVRXRSettings instance is missing");

		// Set by project settings
		Interop.WVR_SetAMCMode(AMCMode);

		Time.timeScale = 1.0f;
		if (rotatedObjectOrigin == null || materials.Count == 0 || meshes.Count == 0)
		{
			enabled = false;
			return;
		}

		if (velocityText == null || velocitySlider == null ||
			distanceText == null || distanceSlider == null ||
			heightText == null || heightSlider == null ||
			sleepGTText == null || sleepGTSlider == null ||
			sleepRTText == null || sleepRTSlider == null ||
			timeScaleText == null || timeScaleSlider == null ||
			giantRoot == null || statusText == null)
		{
			enabled = false;
			return;
		}
		SliderTextInit();

		LoadData();
	}

	private void OnDisable()
	{
		Time.timeScale = 1.0f;

		SaveData();
	}

	private Material GetRandomMaterial()
	{
		int N = materials.Count - 1;
		int i = Random.Range(0, N);
		Debug.Log("GetRandomMaterial=" + i);
		return materials[i];
	}

	private Mesh GetRandomMesh()
	{
		int N = meshes.Count - 1;
		int i = Random.Range(0, N);
		Debug.Log("GetRandomMesh=" + i);
		return meshes[i];
	}

	private float GetRandomDistance()
	{
		return Random.Range(3.0f, 7.0f);
	}

	private float GetRandomHeight()
	{
		return Random.Range(-2, 2.0f);
	}

	private float GetRandomSpeed()
	{
		float max = 35.0f;
		float speed = Random.Range(-max, max);
		// Be careful, don't make dead loop.
		if (Mathf.Abs(speed) < (max / 7))
			return GetRandomSpeed();
		return speed;
	}

	private Quaternion GetRandomQuaternion()
	{
		return Random.rotation;
	}

	void IncreaseObject(Vector3 initialPosition, Quaternion initalRotation, float anglePerSecond, Quaternion pose)
	{
		var data = new AMCCreatedObjectRuntimeData();
		data.angularVelocity = anglePerSecond;
		data.randRotation = pose;

		var rotorObj = new GameObject("Rotor");
		rotorObj.transform.SetParent(rotatedObjectOrigin.transform, false);
		rotorObj.transform.localRotation = initalRotation;
		data.obj = rotorObj;

		var rotor = rotorObj.AddComponent<rotatebytime>();
		rotor.AxisX = false;
		rotor.AxisY = true;
		rotor.AxisZ = false;
		rotor.AnglePerSecond = anglePerSecond;
		rotor.ChangePerSecond = 0;
		rotor.useFixedTime = false;
		var armObj = new GameObject("Arm");
		armObj.transform.SetParent(rotorObj.transform, false);
		armObj.transform.localPosition = initialPosition;

		var meshesObj = new GameObject("Meshes");
		var meshFilter = meshesObj.AddComponent<MeshFilter>();
		var meshRenderer = meshesObj.AddComponent<MeshRenderer>();
		meshesObj.transform.SetParent(armObj.transform, false);
		meshesObj.transform.localRotation = pose;

		var randomMesh = GetRandomMesh();
		var randomMaterial = GetRandomMaterial();

		meshFilter.sharedMesh = randomMesh;
		meshRenderer.sharedMaterial = randomMaterial;

		objects.Add(data);
	}

	void DecreaseObject()
	{
		int lastIndex = objects.Count - 1;
		Destroy(objects[lastIndex].obj, 1);
		objects.RemoveAt(lastIndex);
	}

	public class TestMessage : Message
	{
		public int sleepRT;
	}

	RenderThreadSyncObject renderThreadRunnable = new RenderThreadSyncObject(
		// receiver
		(queue) => {
			lock (queue)
			{
				// Run in RenderThread
				var msg = (TestMessage)queue.Dequeue();
				System.Threading.Thread.Sleep(msg.sleepRT);
				queue.Release(msg);
			}
		});

	private void UpdateStatusText()
	{
		WVR_AMCMode currentAMCMode = Interop.WVR_GetAMCMode();
		WVR_AMCState currentAMCState = Interop.WVR_GetAMCState();

		StringBuilder sb = new StringBuilder();
		sb.Append("instance FPS = ").Append(fps).AppendLine()
			.Append("Avg 3s FPS = ").Append(avg3sFPS).AppendLine()
			.Append("Avg 8s FPS = ").Append(avg8sFPS).AppendLine()
			.Append("Build with AMC Mode = ").Append(buildAMCMode).AppendLine();

		if (buildAMCMode < WVR_AMCMode.Auto)
		{
			sb.Append("<b> PMC will not work.</b>").AppendLine();
		}

		sb
			.Append("Current AMC Mode = ").Append(currentAMCMode).AppendLine()
			.Append("Current AMC Status = ").Append(currentAMCState).AppendLine()
			.Append("Set AMC Mode = ").Append(AMCMode).AppendLine()
			.Append("Obj Count = ").Append(objects.Count).AppendLine()
			.Append("Giant Mode = ").Append(giantMode).AppendLine();
		statusText.text = sb.ToString();
	}

	// Update is called once per frame
	void Update()
	{
		// For UI test
		if (increaseObject)
		{
			increaseObject = false;
			IncreaseObject(new Vector3(0, GetRandomHeight(), GetRandomDistance()), Quaternion.identity, GetRandomSpeed(), GetRandomQuaternion());
		}

		if (decreaseObject)
		{
			decreaseObject = false;
			DecreaseObject();
		}

		if (back)
		{
			back = false;
			SaveData();
			if (MasterSceneManager.Instance)
				MasterSceneManager.Instance.LoadPrevious();
		}

		if (reset)
		{
			reset = false;
			DeleteData();
			SceneManager.LoadScene("AMCTest", LoadSceneMode.Single);
		}

		if (sleepGT > 0)
		{
			System.Threading.Thread.Sleep((int)sleepGT);
		}

		if (sleepRT > 0)
		{
			var queue = renderThreadRunnable.Queue;
			lock (queue)
			{
				var msg = queue.Obtain<TestMessage>();
				msg.sleepRT = (int)sleepRT;
				queue.Enqueue(msg);
			}
			renderThreadRunnable.IssueEvent();
		}

		#region Update FPS
		do
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			accTimeUpdate += unscaledDeltaTime;
			accTime3s += unscaledDeltaTime;
			accTime8s += unscaledDeltaTime;
			frameCount3s++;
			frameCount8s++;

			if (accTime3s > 3)
			{
				avg3sFPS = (frameCount3s - 1) / 3.0f;
				frameCount3s = 1;
				accTime3s = 0;
			}

			if (accTime8s > 8)
			{
				avg8sFPS = (frameCount8s - 1) / 8.0f;
				frameCount8s = 1;
				accTime8s = 0;
			}

			float interp = unscaledDeltaTime / (0.5f + unscaledDeltaTime);
			float currentFPS = 1.0f / unscaledDeltaTime;
			fps = Mathf.Lerp(fps, currentFPS, interp);
			var showFps = fps;

			// Avoid update Canvas too frequently.
			if (accTimeUpdate > 0.20f)
			{
				UpdateStatusText();
				accTimeUpdate = 0;
			}
		} while (false);
		#endregion Update FPS
	}

	public Transform movableObjRoot;


	void SaveData()
	{
		AMCSave save = new AMCSave();

		List<AMCCubeData> cubeList = new List<AMCCubeData>();
		int N = movableObjRoot.childCount;
		for (int i = 0; i < N; i++)
		{
			var objTransform = movableObjRoot.GetChild(i);
			var p = objTransform.localPosition;
			var q = objTransform.localRotation;
			var data = new AMCCubeData()
			{
				px = p.x,
				py = p.y,
				pz = p.z,
				qx = q.x,
				qy = q.y,
				qz = q.z,
				qw = q.w
			};
			cubeList.Add(data);
		}

		List<AMCCreatedObject> objList = new List<AMCCreatedObject>();
		N = objects.Count;
		for (int i = 0; i < N; i++)
		{
			var objData = objects[i];
			var obj = objData.obj.transform;
			var arm = obj.transform.GetChild(0);
			var p = arm.localPosition;
			var q = obj.localRotation;
			var data = new AMCCreatedObject()
			{
				angularVelocity = objData.angularVelocity,
				px = p.x,
				py = p.y,
				pz = p.z,
				qx = q.x,
				qy = q.y,
				qz = q.z,
				qw = q.w,
				rqx = objData.randRotation.x,
				rqy = objData.randRotation.y,
				rqz = objData.randRotation.z,
				rqw = objData.randRotation.w
			};
			objList.Add(data);
		}

		save.cubeData = cubeList.ToArray();
		save.objectData = objList.ToArray();

		string json = JsonUtility.ToJson(save);

		var savePath = Application.persistentDataPath + "/AMCSave.json";
		try
		{
			File.WriteAllText(savePath, json);
		}
		catch (System.Exception)
		{
			Debug.Log("LoadSave fail.");
			return;
		}
		Debug.Log("Saved at " + savePath);
	}

	void LoadData()
	{
		var savePath = Application.persistentDataPath + "/AMCSave.json";
		Debug.Log("LoadSave savePath=" + savePath);
		string json = null;
		try
		{
			json = File.ReadAllText(savePath);
		}
		catch (FileNotFoundException)
		{
			Debug.Log("LoadSave fail.");
			return;
		}
		if (json == null || json.Length == 0)
			return;

		AMCSave save = JsonUtility.FromJson<AMCSave>(json);

		int N = movableObjRoot.childCount;
		int M = save.cubeData.Length;
		if (N == M)
		{
			for (int i = 0; i < N; i++)
			{
				var d = save.cubeData[i];
				var obj = movableObjRoot.GetChild(i);

				obj.localPosition = new Vector3(d.px, d.py, d.pz);
				obj.localRotation = new Quaternion(d.qx, d.qy, d.qz, d.qw);

				var rigidbody = obj.GetComponent<Rigidbody>();
				rigidbody.ResetInertiaTensor();
			}
		}

		N = save.objectData.Length;
		for (int i = 0; i < N; i++)
		{
			var d = save.objectData[i];
			var p = new Vector3(d.px, d.py, d.pz);
			var q = new Quaternion(d.qx, d.qy, d.qz, d.qw);
			var rq = new Quaternion(d.rqx, d.rqy, d.rqz, d.rqw);
			IncreaseObject(p, q, d.angularVelocity, rq);
		}
	}

	void DeleteData()
	{
		var savePath = Application.persistentDataPath + "/AMCSave.json";
		try
		{
			File.Delete(savePath);
		}
		catch (System.Exception)
		{
			return;
		}
	}

	#region UI
	public Text velocityText = null;
	public Slider velocitySlider = null;
	float velocity = 30;

	public Text distanceText = null;
	public Slider distanceSlider = null;
	float distance = 2;

	public Text heightText = null;
	public Slider heightSlider = null;
	float height = 0;

	public Text sleepGTText = null;
	public Slider sleepGTSlider = null;

	public Text sleepRTText = null;
	public Slider sleepRTSlider = null;

	public Text timeScaleText = null;
	public Slider timeScaleSlider = null;

	private int giantMode = 0;
	public GameObject giantRoot = null;

	public Text statusText = null;

	public void SliderTextInit()
	{
		velocityText.text = "Angular Speed (deg/s) = " + velocity;
		distanceText.text = "Distance Offset = " + distance;
		heightText.text = "Height Offset = " + height;
		sleepGTText.text = "Sleep in GameThread (ms) = " + sleepGT;
		sleepRTText.text = "Sleep in RenderThread (ms) = " + sleepRT;
		timeScaleText.text = "Time scale = 1";
	}

	public void OnSliderValueChange(Slider slider)
	{
		Random.InitState(Mathf.CeilToInt(Time.unscaledTime * 100000));

		if (velocitySlider == slider)
		{
			var value = slider.value;
			var sign = Mathf.Sign(value);
			value = sign * Mathf.Clamp(Mathf.Abs(value), 10, 360);
			slider.SetValueWithoutNotify(value);  // Must not use slider.value = xxx.  That will cause recursive callback.

			velocity = value;
			velocityText.text = "Angular Speed (deg/s) =" + velocity;
			return;
		}
		else if (distanceSlider == slider)
		{
			var value = slider.value / 2;
			value = Mathf.Clamp(value, 2, 11);
			slider.SetValueWithoutNotify(value * 2);  // Must not use slider.value = xxx.  That will cause recursive callback.

			distance = value;
			distanceText.text = "Distance Offset =" + distance;
			return;
		}
		else if (heightSlider == slider)
		{
			var value = slider.value / 8;
			value = Mathf.Clamp(value, -4, 4);
			slider.SetValueWithoutNotify(value * 8);  // Must not use slider.value = xxx.  That will cause recursive callback.

			height = value;
			heightText.text = "Height Offset =" + height;
			return;
		}
		else if (sleepGTSlider == slider)
		{
			int value = (int)slider.value;
			value = Mathf.Clamp(value, 0, 50);
			slider.SetValueWithoutNotify(value);  // Must not use slider.value = xxx.  That will cause recursive callback.

			sleepGT = (uint)value;
			sleepGTText.text = "Sleep in GameThread (ms) = " + sleepGT;
			return;
		}
		else if (sleepRTSlider == slider)
		{
			int value = (int)slider.value;
			value = Mathf.Clamp(value, 0, 50);
			slider.SetValueWithoutNotify(value);  // Must not use slider.value = xxx.  That will cause recursive callback.

			sleepRT = (uint)value;
			sleepRTText.text = "Sleep in RenderThread (ms) = " + sleepRT;
			return;
		}
		else if (timeScaleSlider == slider)
		{
			float value = slider.value;
			value = Mathf.Clamp(value, 0, 1);
			slider.SetValueWithoutNotify(value);  // Must not use slider.value = xxx.  That will cause recursive callback.

			Time.timeScale = slider.value;
			//Time.fixedDeltaTime = Mathf.Clamp(1.0f / 75.0f * value, 0.001f, 1.0f / 75.0f);  // Beware, can't be zero;

			timeScaleText.text = "Time scale = " + value;
			return;
		}
	}

	public void OnButtonClick(Button btn)
	{
		if (btn.name == "IncBtn")
		{
			IncreaseObject(new Vector3(0, height, distance), Quaternion.identity, velocity, GetRandomQuaternion());
			return;
		}
		else if (btn.name == "Inc1RandBtn")
		{
			IncreaseObject(new Vector3(0, GetRandomHeight(), GetRandomDistance()), Quaternion.identity, GetRandomSpeed(), GetRandomQuaternion());
			return;
		}
		else if (btn.name == "Inc5RandBtn")
		{
			for (int i = 0; i < 5; i++)
				IncreaseObject(new Vector3(0, GetRandomHeight(), GetRandomDistance()), Quaternion.identity, GetRandomSpeed(), GetRandomQuaternion());
			return;
		}
		else if (btn.name == "DecBtn")
		{
			DecreaseObject();
			return;
		}
		else if (btn.name == "GiantBtn")
		{
			giantMode = ++giantMode % 5;  // Max = 4
			float scale = Mathf.Pow(1.83f, giantMode);  // 1.83 ^ 4 ~= 20
			giantRoot.transform.localScale = new Vector3(scale, scale, scale);
			return;
		}
		else if (btn.name == "AQOffBtn")
		{
			Interop.WVR_EnableAdaptiveQuality(false, 0);
			return;
		}
		else if (btn.name == "AQFRBtn")
		{
			WVR_QualityStrategy flags = 
				WVR_QualityStrategy.WVR_QualityStrategy_SendQualityEvent | 
				WVR_QualityStrategy.WVR_QualityStrategy_AutoAMC;
			Interop.WVR_EnableAdaptiveQuality(true, (uint)flags);
			return;
		}
		else if (btn.name == "AQFRAMCBtn")
		{
			WVR_QualityStrategy flags =
				WVR_QualityStrategy.WVR_QualityStrategy_SendQualityEvent |
				WVR_QualityStrategy.WVR_QualityStrategy_AutoFoveation |
				WVR_QualityStrategy.WVR_QualityStrategy_AutoAMC;
			Interop.WVR_EnableAdaptiveQuality(true, (uint)flags);
			return;
		}
		else if (btn.name == "AMCOffBtn")
		{
			AMCMode = WVR_AMCMode.Off;
			Interop.WVR_SetAMCMode(AMCMode);
			return;
		}
		else if (btn.name == "AMCForceUMCBtn")
		{
			AMCMode = WVR_AMCMode.Force_UMC;
			Interop.WVR_SetAMCMode(AMCMode);
			return;
		}
		else if (btn.name == "AMCAutoBtn")
		{
			AMCMode = WVR_AMCMode.Auto;
			Interop.WVR_SetAMCMode(AMCMode);
			return;
		}
		else if (btn.name == "AMCForcePMCBtn")
		{
			AMCMode = WVR_AMCMode.Force_PMC;
			Interop.WVR_SetAMCMode(AMCMode);
			return;
		}
		else if (btn.name == "ResetBtn")
		{
			DeleteData();
			SceneManager.LoadScene("AMCTest", LoadSceneMode.Single);
			return;
		}
		else if (btn.name == "BackBtn")
		{
			Time.timeScale = 1.0f;
			SaveData();
			if (MasterSceneManager.Instance)
				MasterSceneManager.Instance.LoadPrevious();
			return;
		}
	}

	#endregion UI
}
