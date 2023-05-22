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
	[CustomEditor(typeof(ControllerRaycastPointer))]
	public class ControllerRaycastPointerEditor : UnityEditor.Editor
    {
		/// Physics Raycaster options
		SerializedProperty m_IgnoreReversedGraphics, m_PhysicsCastDistance, m_PhysicsEventMask;
		/// RaycastPointer options
		SerializedProperty m_ShowRay, m_RayStartWidth, m_RayEndWidth, m_RayMaterial, m_Pointer;
		/// ControllerRaycastPointer options
		SerializedProperty m_Controller, m_ControlKey, m_HideWhenIdle, m_AlwaysEnable;

		private void OnEnable()
		{
			/// Physics Raycaster options
			m_IgnoreReversedGraphics = serializedObject.FindProperty("m_IgnoreReversedGraphics");
			m_PhysicsCastDistance = serializedObject.FindProperty("m_PhysicsCastDistance");
			m_PhysicsEventMask = serializedObject.FindProperty("m_PhysicsEventMask");
			/// RaycastPointer options
			m_ShowRay = serializedObject.FindProperty("m_ShowRay");
			m_RayStartWidth = serializedObject.FindProperty("m_RayStartWidth");
			m_RayEndWidth = serializedObject.FindProperty("m_RayEndWidth");
			m_RayMaterial = serializedObject.FindProperty("m_RayMaterial");
			m_Pointer = serializedObject.FindProperty("m_Pointer");
			/// ControllerRaycastPointer options
			m_Controller = serializedObject.FindProperty("m_Controller");
			m_ControlKey = serializedObject.FindProperty("m_ControlKey");
			m_HideWhenIdle = serializedObject.FindProperty("m_HideWhenIdle");
			m_AlwaysEnable = serializedObject.FindProperty("m_AlwaysEnable");
		}

		bool PhysicsRaycasterOptions = false, RayOptions = true, ControllerOptions = true;
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			ControllerRaycastPointer myScript = target as ControllerRaycastPointer;

			PhysicsRaycasterOptions = EditorGUILayout.Foldout(PhysicsRaycasterOptions, "Physics Raycaster Settings");
			if (PhysicsRaycasterOptions)
			{
				EditorGUILayout.PropertyField(m_IgnoreReversedGraphics);
				EditorGUILayout.PropertyField(m_PhysicsCastDistance);
				EditorGUILayout.PropertyField(m_PhysicsEventMask);
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			RayOptions = EditorGUILayout.Foldout(RayOptions, "Ray and Pointer Settings");
			if (RayOptions)
			{
				EditorGUILayout.PropertyField(m_ShowRay);
				EditorGUILayout.PropertyField(m_RayStartWidth);
				EditorGUILayout.PropertyField(m_RayEndWidth);
				EditorGUILayout.PropertyField(m_RayMaterial);
				EditorGUILayout.PropertyField(m_Pointer);
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			ControllerOptions = EditorGUILayout.Foldout(ControllerOptions, "Controller Settings");
			if (ControllerOptions)
			{
				EditorGUILayout.PropertyField(m_Controller);
				EditorGUILayout.PropertyField(m_ControlKey);
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			EditorGUILayout.PropertyField(m_HideWhenIdle);
			EditorGUILayout.PropertyField(m_AlwaysEnable);

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
				EditorUtility.SetDirty((ControllerRaycastPointer)target);
		}
	}
}
#endif