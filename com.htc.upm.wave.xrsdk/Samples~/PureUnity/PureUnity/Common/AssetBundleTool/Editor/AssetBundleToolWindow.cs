// AssetBundleTool is designed for general Unity environment.
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetBundleTool
{
	public class AssetBundleToolWindow : EditorWindow
	{
		[MenuItem("Window/AssetBundleToolWindow")]
		public static void ShowWindow()
		{
			GetWindow<AssetBundleToolWindow>("AB Tools");
		}

		public void OnGUI()
		{
			if (GUILayout.Button("Export AssetBundle"))
			{
				GetWindow<AssetBundleToolBuildWindow>("Build Scene", typeof(AssetBundleToolWindow));
			}

			if (GUILayout.Button("Find GameObject with scripts"))
			{
				GetWindow<FindGameObjectWithScript>("Find Script", typeof(AssetBundleToolWindow));
			}
		}
	}

	public class AssetBundleToolBuildWindow : EditorWindow
	{
		private const string TAG = "ABTEW";
		private const string dragDropIdentifier = "abte_scene";
		private Rect area = new Rect(1, 1, 600, 300);
		private Vector2 scrollPosition = Vector2.zero;
		private string AssetBundleExtension = ".assetbundle";
		private string outputName = "scenes";
		private string outputPath = "";
		private int target;

		private const string SCENE_NAME = TAG + "_SCENE_NAME";
		private const string SCENE_PATH = TAG + "_SCENE_PATH";
		private const string SCENES_PATHS = TAG + "_SCENES_PATHS";
		private const string BUILD_PLATFORM = TAG + "_BUILD_PLATFORM";

		public class SceneData
		{
			public string assetPath;
			public string name;
		}

		private List<SceneData> SceneList = new List<SceneData>();

		private void ReloadFromPrefs()
		{
			outputName = EditorPrefs.GetString(SCENE_NAME, outputName);
			var path = Path.GetFullPath(Application.dataPath + "/..");
			outputPath = EditorPrefs.GetString(SCENE_PATH, path);
			target = EditorPrefs.GetInt(BUILD_PLATFORM, 0 /*Android*/);

			string scenesPaths = EditorPrefs.GetString(SCENES_PATHS, "");
			//Debug.Log("Load " + scenesPaths);

			char[] separators = { ';' };
			string[] paths = scenesPaths.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			foreach (string p in paths)
			{
				if (string.IsNullOrEmpty(p) || !File.Exists(p))
				{
					Debug.LogError("Scene " + p + " is not exist.  Remove from list");
					continue;
				}
				SceneData sd = new SceneData()
				{
					name = Path.GetFileNameWithoutExtension(p),
					assetPath = p
				};
				SceneList.Add(sd);
			}
		}

		private void SaveToPrefs()
		{
			EditorPrefs.SetString(SCENE_NAME, outputName);
			EditorPrefs.SetString(SCENE_PATH, outputPath);
			EditorPrefs.SetInt(BUILD_PLATFORM, target);

			if (SceneList.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				bool first = true;
				foreach (var sd in SceneList)
				{
					if (sd == null)
						continue;
					if (!first)
						sb.Append(";");
					sb.Append(sd.assetPath);
					first = false;
				}
				EditorPrefs.SetString(SCENES_PATHS, sb.ToString());
			}
			else
				EditorPrefs.SetString(SCENES_PATHS, "");
		}

		private void Awake()
		{
			minSize = new Vector2(100, 120);
			// load scene list from property
			ReloadFromPrefs();
		}

		public void OnGUI()
		{
			// Reset area
			area = new Rect(2, 0, position.width - 2, position.height - 100);

			using (var vs = new GUILayout.VerticalScope())
			{
				using (var svs = new GUILayout.ScrollViewScope(scrollPosition, GUILayout.Width(area.width), GUILayout.Height(area.height)))
				{
					scrollPosition = svs.scrollPosition;

					RedrawScrollView();
				}
			}

			DragDropGUI(area, null);

			GUILayout.Label("Drap and drop the scenes you want to build above");
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Clear"))
			{
				SceneList.Clear();
				SaveToPrefs();
			}
			if (GUILayout.Button("Build"))
			{
				BuildProject(outputPath, target == 0 ? BuildTarget.Android : BuildTarget.StandaloneWindows64);
				SaveToPrefs();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Output name");
			outputName = GUILayout.TextField(outputName);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Output path");
			outputPath = GUILayout.TextField(outputPath);
			GUILayout.EndHorizontal();

			string[] list = { "Android", "Windows64" };
			GUILayout.BeginHorizontal();
			GUILayout.Label("target");
			target = EditorGUILayout.Popup(target, list);
			GUILayout.EndHorizontal();
		}

		public void BuildProject(string outputPath, BuildTarget target = BuildTarget.Android, BuildOptions buildOptions = BuildOptions.None, BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None)
		{
			// Check output path
			var bundleOutputDir = Path.GetFullPath(outputPath);
			if (!Directory.Exists(bundleOutputDir))
			{
				Directory.CreateDirectory(bundleOutputDir);
			}
			// Collect & Build assetbundles

			var ans = new string[SceneList.Count];
			for (int i = 0; i < SceneList.Count; i++)
			{
				ans[i] = SceneList[i].assetPath;
				Debug.Log("Export asset " + SceneList[i].name + " at " + ans[i]);
			}

			var assetBundleBuilds = new AssetBundleBuild[] {
				new AssetBundleBuild {
					assetBundleName = outputName + AssetBundleExtension,
					assetNames = ans
				}
			};
			BuildPipeline.BuildAssetBundles(bundleOutputDir, assetBundleBuilds, buildAssetBundleOptions, target);
			Debug.Log("AssetBundle is here: " + Path.Combine(bundleOutputDir, outputName + AssetBundleExtension));
		}

		private void RedrawScrollView()
		{
			EditorGUI.DrawRect(area, Color.grey);
			foreach (var sd in SceneList)
			{
				GUILayout.Label(sd.name);
			}
		}

		protected bool IsDragTargetValid()
		{
			// Drag from Project window have path, but not from Hierarchy window
			if (DragAndDrop.paths.Length != DragAndDrop.objectReferences.Length)
			{
				return false;
			}
			return true;
		}

		protected void DragDropGUI(Rect dropArea, SerializedProperty property)
		{
			// Cache References:
			Event currentEvent = Event.current;
			EventType currentEventType = currentEvent.type;

			//// The DragExited event does not have the same mouse position data as the other events,
			//// so it must be checked now:
			if (currentEventType == EventType.DragExited)
				DragAndDrop.SetGenericData(dragDropIdentifier, null);// Clear generic data when user pressed escape. (Unfortunately, DragExited is also called when the mouse leaves the drag area)

			if (!dropArea.Contains(currentEvent.mousePosition))
				return;

			switch (currentEventType)
			{
				case EventType.MouseDown:
					break;
				case EventType.MouseDrag:
					break;
				case EventType.DragUpdated:
					if (IsDragTargetValid())
						DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
					else
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

					currentEvent.Use();
					break;
				case EventType.Repaint:
					if (DragAndDrop.visualMode == DragAndDropVisualMode.None ||
						DragAndDrop.visualMode == DragAndDropVisualMode.Rejected)
						break;
					break;
				case EventType.DragPerform:
					var objs = DragAndDrop.objectReferences;
					var paths = DragAndDrop.paths;

					//Debug.Log("Objs:" + objs.Length + " paths:" + paths.Length);

					if (objs != null)
					{
						foreach (var obj in objs)
						{
							if (!AssetDatabase.IsMainAsset(obj))
								continue;

							var path = AssetDatabase.GetAssetPath(obj);
							if (string.IsNullOrEmpty(path))
								continue;

							string name = Path.GetFileNameWithoutExtension(path);
							SceneData sd = SceneList.Find(x => x.assetPath == path);
							if (sd != null) {
								Debug.LogWarning("Skip scene " + name + " at " + path + " which already exist in the list");
								continue;
							}
							//Debug.Log("Add scene " + name + " at " + path);

							sd = new SceneData()
							{
								assetPath = path,
								name = name
							};
							SceneList.Add(sd);
						}
					}
					DragAndDrop.AcceptDrag();
					break;
				case EventType.MouseUp:
					break;
			}

		}

		public void OnDestroy()
		{
			SaveToPrefs();
		}
	}
}

#endif
