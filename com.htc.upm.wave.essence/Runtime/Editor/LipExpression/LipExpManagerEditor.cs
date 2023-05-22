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
namespace Wave.Essence.LipExpression.Editor
{
    [CustomEditor(typeof(LipExpManager))]
    public class LipExpManagerEditor : UnityEditor.Editor
    {
		const string kLipExpManager = "LipExpManager";
		private static LipExpManager m_LipExpManager = null;

		[MenuItem("Wave/GameObject/Add Lip Expression Manager", priority = 302, validate = true)]
		public static bool AddLipExpManagerValidate()
		{
			return (FindObjectOfType<LipExpManager>() == null && m_LipExpManager == null);
		}
		[MenuItem("Wave/GameObject/Add Lip Expression Manager", priority = 302)]
		public static void AddLipExpManager()
		{
			var lem = new GameObject(kLipExpManager);
			m_LipExpManager = lem.AddComponent<LipExpManager>();
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}

		SerializedProperty m_InitialStart;
		private void OnEnable()
		{
			m_InitialStart = serializedObject.FindProperty("m_InitialStart");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			LipExpManager myScript = target as LipExpManager;

			GUILayout.Space(5);
			EditorGUILayout.HelpBox(
				"Activate the Lip Expression interface when AP starts.",
				MessageType.Info);
			EditorGUILayout.PropertyField(m_InitialStart);

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
				EditorUtility.SetDirty((LipExpManager)target);
		}
	}
}
#endif