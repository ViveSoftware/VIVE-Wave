using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wave.Generic.Sample
{
	public enum SceneBuildConfiguration
	{
		DontCare = 0x00,
		Development = 0x01,  // Used for local test
		Verification = 0x02,  // Used for verification
		DevelopmentAndVerification = 0x03,
		Release = 0x04,  // Release to developer
		DevelopmentAndRelease = 0x05,
		VerificationAndRelease = 0x06,
		DevelopmentAndVerificationAndRelease = 0x07,
		AlwaysBuild = 0x08,  // Used for portal
	}

	[Serializable]
	public struct SceneData
	{
		public SceneData(bool entry, string titleName, string pathOrAssetName, bool hasHelpScene = false, bool pc = true, SceneBuildConfiguration build = SceneBuildConfiguration.VerificationAndRelease)
		{
			name = titleName;  // if has name, is entry
			path = pathOrAssetName;
			isEntry = entry;
			buildConfig = build;
			hasHelp = hasHelpScene;
			onPC = pc;
		}

		public string name;
		public string path;
		public SceneBuildConfiguration buildConfig;
		public bool isEntry;
		public bool hasHelp;
		public bool onPC;
	}

	[Serializable]
	public class VRTestAppScenes : ScriptableObject
	{
		public static VRTestAppScenes instance = null;
		public static VRTestAppScenes Instance
		{
			get
			{
#if UNITY_EDITOR
				if (instance == null)
				{
					instance = new VRTestAppScenes();
					instance.scenesData = GetPredefinedScenesData();
				}
#endif
				return instance;
			}
		}

		public List<string> pathes = new List<string>();
		public List<SceneData> scenesData = new List<SceneData>();

		// For showing your scene in VRTestApp, you need add your scenes into this function's return list.
		public static List<SceneData> GetPredefinedScenesData()
		{
#pragma warning disable
			const SceneBuildConfiguration cfgVR = SceneBuildConfiguration.VerificationAndRelease;  // default
#pragma warning restore
			const SceneBuildConfiguration cfgR = SceneBuildConfiguration.Release;
			const SceneBuildConfiguration cfgV = SceneBuildConfiguration.Verification;
#pragma warning disable
			const SceneBuildConfiguration cfgD = SceneBuildConfiguration.Development;
#pragma warning restore
			const SceneBuildConfiguration cfgA = SceneBuildConfiguration.AlwaysBuild;
			const bool NotForPC = false;
			const bool HasHelp = true;
			const bool NoHelp = false;
			const bool Entry = true;
			const bool NotEntry = false;
			const string NoTitle = "";
			/**
			 *  Define title ###
			 *    2XX is for Render Team's test No.XX
			 *    3XX is for Engine Team's test No.XX
			 *    000 is undefined test case.  Maybe the test scene is from sample.
			 *  Please use the predefined const define above when you input the arguments of SceneData.
			**/
			return new List<SceneData>() {
				new SceneData(NotEntry, NoTitle, "VRTestApp", false, true, cfgA),  // VRTestApp Loader
				new SceneData(Entry, "301 SeaOfCubes", "SeaOfCubeMain", HasHelp),
				new SceneData(NotEntry, NoTitle, "SeaOfCubeWithTwoHead", HasHelp),
				new SceneData(Entry, "302 CameraTextureTest_DisableSyncPose", "CameraTextureTest_DisableSyncPose", NoHelp, NotForPC),
				new SceneData(Entry, "303 PermissionMgrTest", "PermissionMgrTest", NoHelp, NotForPC),
				new SceneData(Entry, "207 Foveation Test", "FoveatedTest", NoHelp, NotForPC),
				new SceneData(Entry, "209 AQDR_Test", "AQDR_Test", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "AQDR_Loading"),
				new SceneData(Entry, "210 FadeOut Test", "FadeOut_Test", NoHelp, NotForPC),
				new SceneData(Entry, "211 FSE Test", "FSE_Test", NoHelp, NotForPC),

				new SceneData(Entry, "304 WaveControllerTest", "WaveControllerTest"),

				new SceneData(Entry, "305 Interaction Mode", "InteractionMode"),
				new SceneData(Entry, "306 Button Test", "ButtonTest"),

				//new SceneData(Entry, "307 Movie Mode (no exit)", "MovieMode", NoHelp, NotForPC, cfgD),
				new SceneData(Entry, "308 RenderDoc", "RenderDocSample", NoHelp, NotForPC, cfgR),
				//new SceneData(Entry, "309 MSAA On/Off", "MSAAOnOffTest", NoHelp, NotForPC),
				new SceneData(Entry, "310 StereoRenderMode", "StereoRenderMode"),
				new SceneData(Entry, "311 RenderMask Test", "RenderMask_Test"),

				new SceneData(Entry, "312 Balls Room", "BallsRoom", NoHelp, NotForPC, cfgV),
				new SceneData(NotEntry, NoTitle, "BallRoom", NoHelp, NotForPC, cfgV),
				new SceneData(NotEntry, NoTitle, "BallRoomAnimation", NoHelp, NotForPC, cfgV),
				new SceneData(NotEntry, NoTitle, "BallRoomPhysical", NoHelp, NotForPC, cfgV),

				new SceneData(Entry, "313 AMC Test", "AMCTest", NoHelp, NotForPC),

				new SceneData(Entry, "314 ControllerTipsTest", "ControllerTipsTest"),

				new SceneData(Entry, "315 CompositorLayerTest", "CompositorLayerTest", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_Quad_Overlay", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_Quad_Underlay", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_Cylinder_Overlay", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_Cylinder_Underlay", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_UICanvasOverlay", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_UICanvasUnderlay", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_ShapeSwitch", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_Scaling", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_Passthrough", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "CompositorLayerTest_CustomShader", NoHelp, NotForPC),

				new SceneData(Entry, "318 Mirror Test", "MirrorTest", NoHelp, NotForPC),

				new SceneData(Entry, "501 IME Test", "IMETest", NoHelp, NotForPC),

				new SceneData(Entry, "319 Natural Hand", "NaturalHand"),
				new SceneData(Entry, "320 Hand Gesture", "HandGesture"),
				new SceneData(Entry, "321 Role Change", "RoleChange"),
				new SceneData(Entry, "322 Pass Through", "PassThrough"),
				new SceneData(Entry, "323 Trackers", "Trackers"),
				new SceneData(Entry, "324 Facial Expression", "FaceSample_v2"),
				new SceneData(Entry, "325 Eye Tracking", "RaycastEyeTracking"),
				new SceneData(Entry, "326 Spectator Test", "SpectatorTest", NoHelp, NotForPC),

				new SceneData(Entry, "327 Scene Perception", "ScenePerceptionDemoEntrance", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "ScenePerceptionDemo(PlaneSpatialAnchor)", NoHelp, NotForPC),
				new SceneData(NotEntry, NoTitle, "ScenePerceptionDemo(SceneMeshAnchor)", NoHelp, NotForPC),

				new SceneData(Entry, "328 Aruco Marker", "TrackableMarkerDemo(Aruco)", NoHelp, NotForPC),
				new SceneData(Entry, "329 Spectator Camera Adv", "Spectator_Adv_Demo", NoHelp, NotForPC),
			};
		}

#if false
		public static List<string> GetScenePathes(VRTestAppScenes asset, bool isPC = false)
		{
			int N = asset.scenesData.Count;
			List<string> o = new List<string>(N);
			for (int i = 0; i < N; i++)
			{
				// Skip non PC scene
				if (isPC && !asset.scenesData[i].onPC)
					continue;

				string path = asset.scenesData[i].path;
				if (!File.Exists(path))
				{
					Debug.LogWarning("Drop lost scene: " + path);
					continue;
				}
				o.Add(path);

				if (asset.scenesData[i].hasHelp)
				{
					string helpPath = path.Remove(path.Length - 6);
					helpPath += "_Help";
					o.Add(helpPath);
				}
			}
			return o;
		}
#endif

		private void Awake()
		{
			Debug.Log("VRTestAppScenes preload asset loaded");
			instance = this;
		}
	}
}
