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
namespace Wave.Essence.Hand.Editor
{
	[CustomEditor(typeof(HandManager))]
	public class HandManagerEditor : UnityEditor.Editor
	{
		const string kHandManager = "HandManager";
		private static HandManager m_HandManager = null;

		[MenuItem("Wave/GameObject/Add Hand Manager", priority = 101)]
		public static void AddHandManager()
		{
			var hm = new GameObject(kHandManager);
			m_HandManager = hm.AddComponent<HandManager>();
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		[MenuItem("Wave/GameObject/Add Hand Manager", priority = 101, validate = true)]
		public static bool AddHandManagerValidate()
		{
			return (FindObjectOfType<HandManager>() == null && m_HandManager == null);
		}

		SerializedProperty m_GestureOptions/*, m_TrackerOptions*/;
		private void OnEnable()
		{
			m_GestureOptions = serializedObject.FindProperty("m_GestureOptions");
			//m_TrackerOptions = serializedObject.FindProperty("m_TrackerOptions");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			HandManager myScript = target as HandManager;

			/// Gesture Options.
			EditorGUILayout.PropertyField(m_GestureOptions);

			/// Tracker Options.
			//EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			/*EditorGUILayout.HelpBox(
				"The Natural and Electronic hand tracker are both disabled by default.\n" +
				"To enable the tracker will consume power.",
				MessageType.Warning);

			EditorGUILayout.PropertyField(m_TrackerOptions);*/
			GUILayout.Space(5);
			EditorGUILayout.HelpBox(
				"Note: You have to check the menu item\n" +
				"Wave > HandTracking > EnableHandTracking",
				MessageType.Info);
			myScript.TrackerOptions.Natural.InitialStart = EditorGUILayout.Toggle("Initial Start Natural Hand", myScript.TrackerOptions.Natural.InitialStart);

			serializedObject.ApplyModifiedProperties();

			if (GUI.changed)
				EditorUtility.SetDirty((HandManager)target);
		}
	}
}
#endif

