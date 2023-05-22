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

#if UNITY_EDITOR
using UnityEditor;

namespace Wave.Essence.Raycast.Editor
{
    [CustomEditor(typeof(RaycastSwitch))]
    public class RaycastSwitchEditor : UnityEditor.Editor
    {
        SerializedProperty m_GazeRaycast, m_ControllerRaycast, m_HandRaycast;
		private void OnEnable()
		{
			m_GazeRaycast = serializedObject.FindProperty("m_GazeRaycast");
			m_ControllerRaycast = serializedObject.FindProperty("m_ControllerRaycast");
			m_HandRaycast = serializedObject.FindProperty("m_HandRaycast");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			RaycastSwitch myScript = target as RaycastSwitch;

			EditorGUILayout.PropertyField(m_GazeRaycast);
			EditorGUILayout.PropertyField(m_ControllerRaycast);
			EditorGUILayout.PropertyField(m_HandRaycast);

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
				EditorUtility.SetDirty((RaycastSwitch)target);
		}
	}
}
#endif