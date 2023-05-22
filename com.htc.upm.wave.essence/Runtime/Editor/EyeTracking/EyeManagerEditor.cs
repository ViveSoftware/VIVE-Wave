// "Wave SDK 
// © 2017 HTC Corporation. All Rights Reserved.
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
namespace Wave.Essence.Eye.Editor
{
	[CustomEditor(typeof(EyeManager))]
	public class EyeManagerEditor : UnityEditor.Editor
	{
		const string kEyeManager = "EyeManager";
		private static EyeManager m_EyeManager = null;

		[MenuItem("Wave/GameObject/Add Eye Manager", priority = 301)]
		public static void AddEyeManager()
		{
			var em = new GameObject(kEyeManager);
			m_EyeManager = em.AddComponent<EyeManager>();
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		[MenuItem("Wave/GameObject/Add Eye Manager", priority = 301, validate = true)]
		public static bool AddEyeManagerValidate()
		{
			return (FindObjectOfType<EyeManager>() == null && m_EyeManager == null);
		}

		SerializedProperty m_EnableEyeTracking, m_LocationSpace;
		private void OnEnable()
		{
			m_EnableEyeTracking = serializedObject.FindProperty("m_EnableEyeTracking");
			m_LocationSpace = serializedObject.FindProperty("m_LocationSpace");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EyeManager myScript = target as EyeManager;

			EditorGUILayout.PropertyField(m_EnableEyeTracking);
			EditorGUILayout.PropertyField(m_LocationSpace);

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
				EditorUtility.SetDirty((EyeManager)target);
		}
	}
}
#endif
