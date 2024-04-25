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
		SerializedProperty m_OnSwipeToRight;
		SerializedProperty m_OnSwipeToLeft;
		SerializedProperty m_OnSwipeToUp;
		SerializedProperty m_OnSwipeToDown;
		bool swipeEvent = false;
		private void OnEnable()
		{
			//m_UseXRDevice = serializedObject.FindProperty("m_UseXRDevice");
			m_InitialStartTracker = serializedObject.FindProperty("m_InitialStartTracker");
			m_OnSwipeToRight = serializedObject.FindProperty("m_OnSwipeToRight");
			m_OnSwipeToLeft = serializedObject.FindProperty("m_OnSwipeToLeft");
			m_OnSwipeToUp = serializedObject.FindProperty("m_OnSwipeToUp");
			m_OnSwipeToDown = serializedObject.FindProperty("m_OnSwipeToDown");
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

			swipeEvent = EditorGUILayout.Foldout(swipeEvent, "Swipe Event");
			if (swipeEvent)
			{
				GUILayout.Space(5);
				EditorGUILayout.HelpBox(
					"When received value 0, it means TrackId 0.",
					MessageType.Info);
				GUILayout.Space(5);
				EditorGUILayout.PropertyField(m_OnSwipeToRight);
				EditorGUILayout.PropertyField(m_OnSwipeToLeft);
				EditorGUILayout.PropertyField(m_OnSwipeToUp);
				EditorGUILayout.PropertyField(m_OnSwipeToDown);
			}

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
