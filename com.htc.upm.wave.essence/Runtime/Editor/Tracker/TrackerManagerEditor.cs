// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
namespace Wave.Essence.Tracker.Editor
{
	[CustomEditor(typeof(TrackerManager))]
	public class TrackerManagerEditor : UnityEditor.Editor
	{
		const string kTrackerManager = "TrackerManager";
		private static TrackerManager m_TrackerManager = null;

		[MenuItem("Wave/GameObject/Add Tracker Manager", priority = 102, validate = true)]
		public static bool AddTrackerManagerValidate()
		{
			return (FindObjectOfType<TrackerManager>() == null && m_TrackerManager == null);
		}
		[MenuItem("Wave/GameObject/Add Tracker Manager", priority = 102)]
		public static void AddTrackerManager()
		{
			var hm = new GameObject(kTrackerManager);
			m_TrackerManager = hm.AddComponent<TrackerManager>();
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}

		SerializedProperty m_InitialStartTracker/*, m_UseXRDevice*/;
		private void OnEnable()
		{
			//m_UseXRDevice = serializedObject.FindProperty("m_UseXRDevice");
			m_InitialStartTracker = serializedObject.FindProperty("m_InitialStartTracker");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			TrackerManager myScript = target as TrackerManager;

			GUILayout.Space(5);
			EditorGUILayout.HelpBox(
				"Activate the Tracker interface when AP starts.",
				MessageType.Info);
			EditorGUILayout.PropertyField(m_InitialStartTracker);

			/*GUILayout.Space(5);
			EditorGUILayout.HelpBox(
				"Retrieve the Tracker data from UnityEngine.XR.InputDevice.",
				MessageType.Info);
			EditorGUILayout.PropertyField(m_UseXRDevice);*/

			serializedObject.ApplyModifiedProperties();

			if (GUI.changed)
				EditorUtility.SetDirty((TrackerManager)target);
		}
	}
}
#endif
