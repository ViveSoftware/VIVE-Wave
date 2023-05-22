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
	[CustomEditor(typeof(HandRaycastPointer))]
	public class HandRaycastPointerEditor : UnityEditor.Editor
	{
		/// Physics Raycaster options
		SerializedProperty m_IgnoreReversedGraphics, m_PhysicsCastDistance, m_PhysicsEventMask;
		/// RaycastPointer options
		SerializedProperty m_ShowRay, m_RayStartWidth, m_RayEndWidth, m_RayMaterial, m_Pointer;
		/// HandRaycastPointer options
		SerializedProperty m_Hand, m_UsePose, m_PinchStrength, m_AlwaysEnable;

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
			/// HandRaycastPointer options
			m_Hand = serializedObject.FindProperty("m_Hand");
			m_UsePose = serializedObject.FindProperty("m_UsePose");
			m_PinchStrength = serializedObject.FindProperty("m_PinchStrength");
			m_AlwaysEnable = serializedObject.FindProperty("m_AlwaysEnable");
		}
		bool PhysicsRaycasterOptions = false, RayOptions = true, HandOptions = true;
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			HandRaycastPointer myScript = target as HandRaycastPointer;

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

			HandOptions = EditorGUILayout.Foldout(HandOptions, "Hand Settings");
			if (HandOptions)
			{
				EditorGUILayout.PropertyField(m_Hand);
				EditorGUILayout.PropertyField(m_UsePose);
				EditorGUILayout.PropertyField(m_PinchStrength);
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			EditorGUILayout.PropertyField(m_AlwaysEnable);

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
				EditorUtility.SetDirty((HandRaycastPointer)target);
		}
	}
}
#endif