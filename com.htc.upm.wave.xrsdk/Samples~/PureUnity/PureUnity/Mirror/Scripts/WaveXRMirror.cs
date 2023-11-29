// original source from: http://wiki.unity3d.com/index.php/MirrorReflection4
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Text;

//[ExecuteInEditMode]
public class WaveXRMirror : MonoBehaviour
{
	public bool m_DisablePixelLights = true;
	public int m_TextureSize = 256;
	public float m_ClipPlaneOffset = 0.07f;
	public float m_FarDistance = 20.0f;
    public float m_NearDistance = 0.03f;
	[Tooltip("Unused now")]
    public int m_framesNeededToUpdate = 0;

	public LayerMask m_ReflectLayers = -1;

	private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();

	private RenderTexture m_ReflectionTextureLeft = null;
	private RenderTexture m_ReflectionTextureRight = null;
    private RenderTexture m_ReflectionTextureMono = null;
    private int m_OldReflectionTextureSize = 0;

	private uint m_frameCounter = 0;

	private int frameCount = 0;
	private static bool s_InsideRendering = false;

	private XRDisplaySubsystem targetDisplay = null;

	private int passCount = 0;

	private Matrix4x4 worldToCameraMatrixL, worldToCameraMatrixR;
	private Matrix4x4 projectionMatrixL, projectionMatrixR;

	[Tooltip("MirrorPlane's forward direction will be going out of the mirror perpendicularly.  MirrorPlane's position should be a point on the mirror's plane.  If empty, use this gameobject as the mirror plane.")]
	public GameObject mirrorPlaneObj;

	List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();

	private bool GetTargetDisplaySubsystem()
	{
		SubsystemManager.GetInstances(displays);

		if (displays.Count == 0)
			return false;
			//throw new Exception("No XR display provider.");

		foreach (var d in displays)
		{
			if (d.running)
			{
				targetDisplay = d;
				return true;
			}
		}
		return false;
	}

	private bool GetWorldCameraMatrixFromDisplayProvider()
	{
		XRDisplaySubsystem.XRRenderPass renderPass;
		XRDisplaySubsystem.XRRenderParameter renderParameter;

		if (!GetTargetDisplaySubsystem()) return false;
		var display = targetDisplay;
		if (display == null) return false;

		passCount = display.GetRenderPassCount();
		if (passCount == 1)
		{
			// SinglePass
			display.GetRenderPass(0, out renderPass);
			int parameterCount = renderPass.GetRenderParameterCount();
			if (parameterCount != 2)
				Debug.LogError("weird");

			renderPass.GetRenderParameter(Camera.main, 0, out renderParameter);
			worldToCameraMatrixL = renderParameter.view;  // This is in Unity's convension
			projectionMatrixL = renderParameter.projection;

			renderPass.GetRenderParameter(Camera.main, 1, out renderParameter);
			worldToCameraMatrixR = renderParameter.view;  // This is in Unity's convension
			projectionMatrixR = renderParameter.projection;
		}
		else if (passCount == 2)
		{
			// MultiPass
			int parameterCount = 0;

			display.GetRenderPass(0, out renderPass);
			parameterCount = renderPass.GetRenderParameterCount();
			if (parameterCount != 1)
				Debug.LogError("weird");

			renderPass.GetRenderParameter(Camera.main, 0, out renderParameter);
			worldToCameraMatrixL = renderParameter.view;  // This is in Unity's convension
			projectionMatrixL = renderParameter.projection;

			display.GetRenderPass(1, out renderPass);
			parameterCount = renderPass.GetRenderParameterCount();
			if (parameterCount != 1)
				Debug.LogError("weird");

			renderPass.GetRenderParameter(Camera.main, 0, out renderParameter);
			worldToCameraMatrixR = renderParameter.view;  // This is in Unity's convension
			projectionMatrixR = renderParameter.projection;
		}
		else
		{
			// Stop all mirror camera
			passCount = 0;

			// If editor, need wait for several frame to have it.
			Debug.LogWarning("weird.   No RenderPass.");
			projectionMatrixL = projectionMatrixR = Camera.main.projectionMatrix;
			return false;
		}

		//DebugMatrix("w2cL", worldToCameraMatrixL, debugMode);
		//DebugMatrix("w2cR", worldToCameraMatrixR, debugMode);
		//DebugMatrix("projL", projectionMatrixL, debugMode);
		//DebugMatrix("projR", projectionMatrixR, debugMode);
		return true;
	}

	private void OnEnable()
	{
		if (mirrorPlaneObj == null)
			mirrorPlaneObj = gameObject;
	}

	bool hasDisplaySubsystem;
	IEnumerator Start()
	{
		// Make sure we got the display provider.
		while (targetDisplay == null)
		{
#pragma warning disable
			bool ret = false;
#pragma warning restore
			try
			{
				if (GetTargetDisplaySubsystem())
				{
					hasDisplaySubsystem = true;
					Debug.Log("We got a display provider:" + targetDisplay.SubsystemDescriptor.id);
					break;
				}
			}
			catch
			{
				// stop running this script
				//enabled = false;
				break;
			}
			yield return null;
		}
	}

	bool NeedUpdate(bool isSinglePass, Camera.MonoOrStereoscopicEye stereoActiveEye)
	{
		Debug.Log("NeedUpdate isSinglePass=" + isSinglePass + ", stereoActiveEye=" + stereoActiveEye);

		if (isSinglePass || stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
		{
			if (m_frameCounter > 0)
			{
				m_frameCounter--;
				return false;
			}
			m_frameCounter = (uint)m_framesNeededToUpdate;
			return true;
		}
		else
		{
			if (stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
			{
				if (m_frameCounter > 0)
				{
					m_frameCounter--;
					return false;
				}
				return true;
			}
			else if (stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
			{
				if (m_frameCounter > 0)
					return false;
				m_frameCounter = (uint)m_framesNeededToUpdate;
				return true;
			}
		}
		return false;
	}

	string propertyL = "_ReflectionTexLeft";
    string propertyR = "_ReflectionTexRight";

    // This is called when it's known that the object will be rendered by some
    // camera. We render reflections and do other updates here.
    // Because the script executes in edit mode, reflections for the scene view
    // camera will just work!
    public void OnWillRenderObject()
	{
		var rend = GetComponent<Renderer>();
		if (!enabled || !rend || !rend.sharedMaterial || !rend.enabled)
			return;

		Camera cam = Camera.current;
		if (!cam)
			return;

		// Safeguard from recursive reflections.  
		if (s_InsideRendering)
			return;

		// Only run on VR camera.  Or if a scene view camera try see it, we update it only in editor
		if (cam.stereoTargetEye != StereoTargetEyeMask.Both)
		{
			if (cam == Camera.main && Application.isEditor)
			{
				s_InsideRendering = true;
				RenderCamera(cam, rend, ref m_ReflectionTextureMono);
				Material[] ms = rend.materials;
				foreach (Material mat in ms)
				{
					if (mat.HasProperty(propertyL))
						mat.SetTexture(propertyL, m_ReflectionTextureMono);
					if (mat.HasProperty(propertyR))
						mat.SetTexture(propertyR, m_ReflectionTextureMono);
				}
				s_InsideRendering = false;
			}
			return;
		}

		if (targetDisplay == null)
			return;

		if (!GetWorldCameraMatrixFromDisplayProvider())
			return;

		if (frameCount == Time.frameCount)
			return;
		frameCount = Time.frameCount;

		s_InsideRendering = true;
		RenderCamera(cam, rend, Camera.StereoscopicEye.Left, ref m_ReflectionTextureLeft);
		RenderCamera(cam, rend, Camera.StereoscopicEye.Right, ref m_ReflectionTextureRight);
		s_InsideRendering = false;

		// Update material after the render command.
		Material[] materials = rend.materials;
		foreach (Material mat in materials)
		{
			if (mat.HasProperty(propertyL))
				mat.SetTexture(propertyL, m_ReflectionTextureLeft);
			if (mat.HasProperty(propertyR))
				mat.SetTexture(propertyR, m_ReflectionTextureRight);
		}
	}

	private void GetMatrixAndPoseOfCamera(Camera cam, Camera.StereoscopicEye eye, out Matrix4x4 worldToCameraMatrix, out Pose cameraPose)
	{
		if (cam.stereoEnabled)
		{
			worldToCameraMatrix = (eye == Camera.StereoscopicEye.Left ? worldToCameraMatrixL : worldToCameraMatrixR);
			Matrix4x4 cameraToWorldMatrix = worldToCameraMatrix.inverse;
			//worldToCameraMatrix = MatrixGlUnityConvert(worldToCameraMatrix);
			cameraPose = new Pose(cameraToWorldMatrix.MultiplyPoint(Vector3.zero), GetRotation(cameraToWorldMatrix));
		}
		else
		{
			worldToCameraMatrix = cam.worldToCameraMatrix; // GL convension
			cameraPose = new Pose(cam.transform.position, GetRotation(worldToCameraMatrix));
		}

/*
		if (cam.stereoEnabled)
		{
			Vector3 eyeOffset;

			worldToCameraMatrix = cam.GetStereoViewMatrix(eye);

			InputTracking.GetNodeStates(nodeStates);
			XRNodeState leftEyeState = findNode(nodeStates, XRNode.LeftEye);
			XRNodeState rightEyeState = findNode(nodeStates, XRNode.RightEye);

			if (eye == Camera.StereoscopicEye.Left)
				leftEyeState.TryGetPosition(out eyeOffset);
			else
				rightEyeState.TryGetPosition(out eyeOffset);

			eyeOffset.z = 0.0f;
			oldEyePos = cam.transform.position + cam.transform.TransformVector(eyeOffset);
		}
		else
		{
			worldToCameraMatrix = cam.worldToCameraMatrix;
			oldEyePos = cam.transform.position;
		}
*/
	}
	private void GetMatrixAndPoseOfCamera(Camera cam, out Matrix4x4 worldToCameraMatrix, out Pose cameraPose)
	{
        worldToCameraMatrix = cam.worldToCameraMatrix; // GL convension
        cameraPose = new Pose(cam.transform.position, GetRotation(worldToCameraMatrix));
    }

    private void RenderCamera(Camera cam, Renderer rend, Camera.StereoscopicEye eye, ref RenderTexture reflectionTexture)
	{
		Camera reflectionCamera;
		CreateMirrorObjects(cam, eye, out reflectionCamera, ref reflectionTexture);
		if (reflectionCamera == null)
			return;

		Vector3 pos = mirrorPlaneObj.transform.position;
		Vector3 normal = mirrorPlaneObj.transform.forward;
		/*
		Vector3 pos = transform.position;
		Vector3 normal = transform.up;
		*/

		int oldPixelLightCount = QualitySettings.pixelLightCount;
		if (m_DisablePixelLights)
			QualitySettings.pixelLightCount = 0;

		CopyCameraProperties(cam, reflectionCamera);


		float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
		Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

		Matrix4x4 reflection = Matrix4x4.zero;
		CalculateReflectionMatrix(ref reflection, reflectionPlane);

		Vector3 oldEyePos;
		Matrix4x4 worldToCameraMatrix;
		Pose cameraPose;
		GetMatrixAndPoseOfCamera(cam, eye, out worldToCameraMatrix, out cameraPose);
		oldEyePos = cameraPose.position;

		reflectionCamera.projectionMatrix = cam.projectionMatrix;
		Vector3 newEyePos = reflection.MultiplyPoint(oldEyePos);
		reflectionCamera.transform.position = newEyePos;

		reflectionCamera.worldToCameraMatrix = worldToCameraMatrix * reflection;

		Vector4 clipPlane = CameraSpacePlane(worldToCameraMatrix * reflection, pos, normal, 1.0f);

		Matrix4x4 projectionMatrix;
		if (cam.stereoEnabled)
			projectionMatrix = cam.GetStereoProjectionMatrix(eye);
		else
			projectionMatrix = cam.projectionMatrix;

		MakeProjectionMatrixOblique(ref projectionMatrix, clipPlane);

		//projectionMatrix = CalculateObliqueMatrix(reflectionCamera, clipPlane);

		reflectionCamera.projectionMatrix = projectionMatrix;
		reflectionCamera.cullingMatrix = projectionMatrix * reflectionCamera.worldToCameraMatrix;
		reflectionCamera.cullingMask = m_ReflectLayers.value;
		reflectionCamera.targetTexture = reflectionTexture;
		GL.invertCulling = true;
		reflectionCamera.Render();
		reflectionCamera.enabled = false;
		GL.invertCulling = false;

		if (m_DisablePixelLights)
			QualitySettings.pixelLightCount = oldPixelLightCount;

		s_InsideRendering = false;
	}

	// For Mono
    private void RenderCamera(Camera cam, Renderer rend, ref RenderTexture reflectionTexture)
    {
        Camera reflectionCamera;
        CreateMirrorObjects(cam, out reflectionCamera, ref reflectionTexture);
        if (reflectionCamera == null)
            return;

        Vector3 pos = mirrorPlaneObj.transform.position;
        Vector3 normal = mirrorPlaneObj.transform.forward;
        /*
		Vector3 pos = transform.position;
		Vector3 normal = transform.up;
		*/

        int oldPixelLightCount = QualitySettings.pixelLightCount;
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = 0;

        CopyCameraProperties(cam, reflectionCamera);


        float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        Matrix4x4 reflection = Matrix4x4.zero;
        CalculateReflectionMatrix(ref reflection, reflectionPlane);

        Vector3 oldEyePos;
        Matrix4x4 worldToCameraMatrix;
        Pose cameraPose;
        GetMatrixAndPoseOfCamera(cam, out worldToCameraMatrix, out cameraPose);
        oldEyePos = cameraPose.position;

        reflectionCamera.projectionMatrix = cam.projectionMatrix;
        Vector3 newEyePos = reflection.MultiplyPoint(oldEyePos);
        reflectionCamera.transform.position = newEyePos;

        reflectionCamera.worldToCameraMatrix = worldToCameraMatrix * reflection;

        Vector4 clipPlane = CameraSpacePlane(worldToCameraMatrix * reflection, pos, normal, 1.0f);

        Matrix4x4 projectionMatrix = cam.projectionMatrix;

        MakeProjectionMatrixOblique(ref projectionMatrix, clipPlane);

        //projectionMatrix = CalculateObliqueMatrix(reflectionCamera, clipPlane);

        reflectionCamera.projectionMatrix = projectionMatrix;
        reflectionCamera.cullingMatrix = projectionMatrix * reflectionCamera.worldToCameraMatrix;
        reflectionCamera.cullingMask = m_ReflectLayers.value;
        reflectionCamera.targetTexture = reflectionTexture;
        GL.invertCulling = true;
        reflectionCamera.Render();
        reflectionCamera.enabled = false;
        GL.invertCulling = false;

        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = oldPixelLightCount;

        s_InsideRendering = false;
    }


    void OnDisable()
	{
		if (m_ReflectionTextureLeft)
		{
			m_ReflectionTextureLeft.Release();
			Destroy(m_ReflectionTextureLeft);
			m_ReflectionTextureLeft = null;
		}
		if (m_ReflectionTextureRight)
		{
			m_ReflectionTextureRight.Release();
			Destroy(m_ReflectionTextureRight);
			m_ReflectionTextureRight = null;
		}
		if (m_ReflectionTextureMono)
		{
			m_ReflectionTextureMono.Release();
			Destroy(m_ReflectionTextureMono);
			m_ReflectionTextureMono = null;
		}
		foreach (var kvp in m_ReflectionCameras)
		{
			var value = (Camera)kvp.Value;
			if (value == null) continue;
			var obj = value.gameObject;
			if (obj == null) continue;
			Destroy(obj);
		}
		m_ReflectionCameras.Clear();
	}

	private void CopyCameraProperties(Camera src, Camera dest)
	{
		if (dest == null)
			return;
		//dest.clearFlags = CameraClearFlags.Color;
		//dest.backgroundColor = new Color(.9f, .9f, .9f);
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!sky || !sky.material)
			{
				mysky.enabled = false;
			}
			else
			{
				mysky.enabled = true;
				mysky.material = sky.material;
			}
		}

		//dest.farClipPlane = 30;
		dest.farClipPlane = m_FarDistance;  // src.farClipPlane;
		dest.nearClipPlane = m_NearDistance;  // src.nearClipPlane;
        dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
		dest.stereoTargetEye = StereoTargetEyeMask.None;
		dest.stereoTargetEye = StereoTargetEyeMask.None;
		//dest.clearFlags = CameraClearFlags.SolidColor;
	}

	private void CreateMirrorObjects(Camera currentCamera, Camera.StereoscopicEye eye, out Camera reflectionCamera, ref RenderTexture reflectionTexture)
	{
		reflectionCamera = null;

		if (!reflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
		{
			if (reflectionTexture)
			{
				reflectionTexture.Release();
				DestroyImmediate(reflectionTexture);
			}
			reflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
			reflectionTexture.name = "__MirrorReflection" + eye.ToString() + GetInstanceID();
			reflectionTexture.isPowerOfTwo = true;
			reflectionTexture.hideFlags = HideFlags.DontSave;
			m_OldReflectionTextureSize = m_TextureSize;
		}

		if (!m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera))
		{
			GameObject go = new GameObject("Mirror Reflection Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
			reflectionCamera = go.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = transform.position;
			reflectionCamera.transform.rotation = transform.rotation;
			reflectionCamera.gameObject.AddComponent<FlareLayer>();
			go.hideFlags = HideFlags.DontSave;
			go.hideFlags = HideFlags.HideInInspector;
			go.hideFlags = HideFlags.HideInHierarchy;
			m_ReflectionCameras.Add(currentCamera, reflectionCamera);
		}
	}

    // For Mono
	private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera, ref RenderTexture reflectionTexture)
    {
        reflectionCamera = null;


        if (!reflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
        {
            if (reflectionTexture)
			{
				reflectionTexture.Release();
				DestroyImmediate(reflectionTexture);
			}
            reflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
            reflectionTexture.name = "__MirrorReflectionMono" + GetInstanceID();
            reflectionTexture.isPowerOfTwo = true;
            reflectionTexture.hideFlags = HideFlags.DontSave;
            m_OldReflectionTextureSize = m_TextureSize;
        }

        if (!m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera))
        {
            GameObject go = new GameObject("Mirror Reflection Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
            reflectionCamera = go.GetComponent<Camera>();
            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.gameObject.AddComponent<FlareLayer>();
            go.hideFlags = HideFlags.DontSave;
            go.hideFlags = HideFlags.HideInInspector;
            go.hideFlags = HideFlags.HideInHierarchy;
            m_ReflectionCameras.Add(currentCamera, reflectionCamera);
        }
    }

    private Vector4 CameraSpacePlane(Matrix4x4 worldToCameraMatrix, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
		Vector3 cpos = worldToCameraMatrix.MultiplyPoint(offsetPos);
		Vector3 cnormal = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	// Calculates reflection matrix around the given plane
	// https://en.wikipedia.org/wiki/Transformation_matrix#Reflection_2
	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		// ax + by + cz + d = 0
		float a, b, c, d;
		a = plane[0];
		b = plane[1];
		c = plane[2];
		d = plane[3];

		reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
		reflectionMat.m01 = (-2F * plane[0] * plane[1]);
		reflectionMat.m02 = (-2F * plane[0] * plane[2]);
		reflectionMat.m03 = (-2F * plane[3] * plane[0]);

		reflectionMat.m10 = (-2F * plane[1] * plane[0]);
		reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
		reflectionMat.m12 = (-2F * plane[1] * plane[2]);
		reflectionMat.m13 = (-2F * plane[3] * plane[1]);

		reflectionMat.m20 = (-2F * plane[2] * plane[0]);
		reflectionMat.m21 = (-2F * plane[2] * plane[1]);
		reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
		reflectionMat.m23 = (-2F * plane[3] * plane[2]);

		reflectionMat.m30 = 0F;
		reflectionMat.m31 = 0F;
		reflectionMat.m32 = 0F;
		reflectionMat.m33 = 1F;
	}

	private static float sgn(float a)
	{
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
	}

	// taken from http://www.terathon.com/code/oblique.html
	private static void MakeProjectionMatrixOblique(ref Matrix4x4 matrix, Vector4 clipPlane)
	{
		Vector4 q;

		q.x = (sgn(clipPlane.x) + matrix[8]) / matrix[0];
		q.y = (sgn(clipPlane.y) + matrix[9]) / matrix[5];
		q.z = -1.0F;
		q.w = (1.0F + matrix[10]) / matrix[14];

		Vector4 c = clipPlane * (2.0F / Vector3.Dot(clipPlane, q));

		matrix[2] = c.x;
		matrix[6] = c.y;
		matrix[10] = c.z + 1.0F;
		matrix[14] = c.w;
	}

	// taken from https://blog.csdn.net/a1047120490/article/details/106743734
	private Matrix4x4 CalculateObliqueMatrix(Camera camera, Vector4 plane)
	{
		var viewSpacePlane = camera.worldToCameraMatrix.inverse.transpose * plane;
		var projectionMatrix = camera.projectionMatrix;

		var clipSpaceFarPanelBoundPoint = new Vector4(Mathf.Sign(viewSpacePlane.x), Mathf.Sign(viewSpacePlane.y), 1, 1);
		var viewSpaceFarPanelBoundPoint = camera.projectionMatrix.inverse * clipSpaceFarPanelBoundPoint;

		var m4 = new Vector4(projectionMatrix.m30, projectionMatrix.m31, projectionMatrix.m32, projectionMatrix.m33);
		//u = 2 * (M4·E)/(E·P)，而M4·E == 1，化简得
		//var u = 2.0f * Vector4.Dot(m4, viewSpaceFarPanelBoundPoint) / Vector4.Dot(viewSpaceFarPanelBoundPoint, viewSpacePlane);
		var u = 2.0f / Vector4.Dot(viewSpaceFarPanelBoundPoint, viewSpacePlane);
		var newViewSpaceNearPlane = u * viewSpacePlane;

		//M3' = P - M4
		var m3 = newViewSpaceNearPlane - m4;

		projectionMatrix.m20 = m3.x;
		projectionMatrix.m21 = m3.y;
		projectionMatrix.m22 = m3.z;
		projectionMatrix.m23 = m3.w;

		return projectionMatrix;
	}

	#region unused
	XRNodeState findNode(List<XRNodeState> nodeStates, XRNode node)
	{
		XRNodeState nodeState = nodeStates[0];
		foreach (var node_i in nodeStates)
		{
			if (node_i.nodeType == node)
			{
				nodeState = node_i;
				break;
			}
		}

		return nodeState;
	}

	void DoNothing()
	{
		// In Unity XR, the StereoViewMatrix didn't work.  Both results are the same.
		//Camera cam = Camera.main;
		//worldToCameraMatrixL = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
		//worldToCameraMatrixR = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
	}

	private bool isMultiviewEnabled = false;
	private bool isSinglePassEnabled = false;

	void ToggleSinglePass(bool enable)
	{
		if (enable)
		{
			isMultiviewEnabled = false;
			isSinglePassEnabled = false;
			if (Shader.IsKeywordEnabled("STEREO_MULTIVIEW_ON"))
			{
				Shader.DisableKeyword("STEREO_MULTIVIEW_ON");
				isMultiviewEnabled = true;

			}
			if (Shader.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO"))
			{
				Shader.DisableKeyword("UNITY_SINGLE_PASS_STEREO");
				isSinglePassEnabled = true;
			}
		}
		else
		{
			if (isMultiviewEnabled)
				Shader.EnableKeyword("UNITY_SINGLE_PASS_STEREO");
			if (isSinglePassEnabled)
				Shader.EnableKeyword("UNITY_SINGLE_PASS_STEREO");
		}
		//Debug.Log("Keyword mv=" + isMultiviewEnabled + ", sp=" + isSinglePassEnabled);
	}

	#endregion unused

	public static Quaternion ReflectRotation(Quaternion source, Vector3 normal)
	{
		// source is from GL matrix.  Forward should be -Z.
		return Quaternion.LookRotation(Vector3.Reflect(source * -Vector3.forward, normal), Vector3.Reflect(source * Vector3.up, normal));
	}

	public static Quaternion GetRotation(Matrix4x4 matrix)
	{
		return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
	}

	// Before a matrix multiply a Unity's vector, do the convert.
	public static Matrix4x4 MatrixGlUnityConvert(Matrix4x4 mIn)
	{
		var m = mIn;
		float sign = -1;

		m[0, 2] = mIn[0, 2] * sign;
		m[1, 2] = mIn[1, 2] * sign;
		m[2, 0] = mIn[2, 0] * sign;
		m[2, 1] = mIn[2, 1] * sign;
		m[2, 3] = mIn[2, 3] * sign;

		return m;
	}

	public static Quaternion GetRotation2(Matrix4x4 matrix)
	{
		float tr = matrix.m00 + matrix.m11 + matrix.m22;
		float qw, qx, qy, qz;
		if (tr > 0)
		{
			float S = Mathf.Sqrt(tr + 1.0f) * 2; // S=4*qw
			qw = 0.25f * S;
			qx = (matrix.m21 - matrix.m12) / S;
			qy = (matrix.m02 - matrix.m20) / S;
			qz = (matrix.m10 - matrix.m01) / S;
		}
		else if ((matrix.m00 > matrix.m11) & (matrix.m00 > matrix.m22))
		{
			float S = Mathf.Sqrt(1.0f + matrix.m00 - matrix.m11 - matrix.m22) * 2; // S=4*qx
			qw = (matrix.m21 - matrix.m12) / S;
			qx = 0.25f * S;
			qy = (matrix.m01 + matrix.m10) / S;
			qz = (matrix.m02 + matrix.m20) / S;
		}
		else if (matrix.m11 > matrix.m22)
		{
			float S = Mathf.Sqrt(1.0f + matrix.m11 - matrix.m00 - matrix.m22) * 2; // S=4*qy
			qw = (matrix.m02 - matrix.m20) / S;
			qx = (matrix.m01 + matrix.m10) / S;
			qy = 0.25f * S;
			qz = (matrix.m12 + matrix.m21) / S;
		}
		else
		{
			float S = Mathf.Sqrt(1.0f + matrix.m22 - matrix.m00 - matrix.m11) * 2; // S=4*qz
			qw = (matrix.m10 - matrix.m01) / S;
			qx = (matrix.m02 + matrix.m20) / S;
			qy = (matrix.m12 + matrix.m21) / S;
			qz = 0.25f * S;
		}
		return new Quaternion(qx, qy, qz, qw).normalized;
	}

	private static void DebugMatrix(string name, Matrix4x4 m, bool show = true)
	{
		if (!show) return;
		StringBuilder sb = new StringBuilder(160);
		sb.AppendFormat("Matrix {0,-16}", name).AppendLine();
		sb.AppendFormat("/ {0:F6} {1:F6} {2:F6} {3:F6} \\", m.m00, m.m01, m.m02, m.m03).AppendLine();
		sb.AppendFormat("| {0:F6} {1:F6} {2:F6} {3:F6} |", m.m10, m.m11, m.m12, m.m13).AppendLine();
		sb.AppendFormat("| {0:F6} {1:F6} {2:F6} {3:F6} |", m.m20, m.m21, m.m22, m.m23).AppendLine();
		sb.AppendFormat("\\ {0:F6} {1:F6} {2:F6} {3:F6} /", m.m30, m.m31, m.m32, m.m33);
		Debug.Log(sb.ToString());
	}

	private static void DebugVector3(string name, Vector3 vector, bool show = true)
	{
		if (!show) return;
		Debug.LogFormat("Vector {0,-16} {1:F6} {2:F6} {3:F6}", name, vector.x, vector.y, vector.z);
	}

	private static void DebugVector4(string name, Vector4 vector, bool show = true)
	{
		if (!show) return;
		Debug.LogFormat("Vector {0,-16} {1:F6} {2:F6} {3:F6} {4:F6}", name, vector.x, vector.y, vector.z, vector.w);
	}

	private static void DebugVector4(string name, Quaternion vector, bool show = true)
	{
		if (!show) return;
		Debug.LogFormat("Vector {0,-16} {1:F6} {2:F6} {3:F6} {4:F6}", name, vector.x, vector.y, vector.z, vector.w);
	}

	private static void DebugPlane(string name, Plane plane, bool show = true)
	{
		if (!show) return;
		Vector4 v4 = ToVector4Plane(plane);
		Debug.LogFormat("Plane {0,-16} ({1:F6}, {2:F6}, {3:F6}, {4:F6})", name, v4.x, v4.y, v4.z, v4.w);
	}

	// positive offset is along with normal's direction
	public static Vector4 ToVector4Plane(Plane plane)
	{
		Vector3 normal = plane.normal;
		// Plane equation : Ax + By + Cz + D = 0
		//return new Vector4(normal.x, normal.y, normal.z, -plane.distance * (plane.GetSide(Vector3.zero) ? 1 : -1));
		// Plane.distance: A positive value for distance results in the Plane facing towards the origin. A negative distance value results in the Plane facing away from the origin.
		return new Vector4(normal.x, normal.y, normal.z, plane.distance);
	}
}
