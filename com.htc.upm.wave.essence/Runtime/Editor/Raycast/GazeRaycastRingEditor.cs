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
    [CustomEditor(typeof(GazeRaycastRing))]
    public class GazeRaycastRingEditor : UnityEditor.Editor
    {
        /// RaycastImpl options
        SerializedProperty m_IgnoreReversedGraphics, m_PhysicsCastDistance, m_PhysicsEventMask, s_GraphicTags;
        /// RaycastRing options
        SerializedProperty m_PointerRingWidth, m_PointerCircleRadius, m_PointerDistance, m_PointerColor, m_ProgressColor, m_PointerMaterial, m_PointerRenderQueue, m_PointerSortingOrder, m_TimeToGaze;
        /// GazeRaycastRing options
        SerializedProperty m_EyeTracking, m_Eye, m_InputEvent, m_ControlKey, m_ControlHand, m_AlwaysEnable;
#if ENABLE_INPUT_SYSTEM
        SerializedProperty m_RotationInput;
#endif
        private void OnEnable()
        {
            /// Physics Raycaster options
            m_IgnoreReversedGraphics = serializedObject.FindProperty("m_IgnoreReversedGraphics");
            m_PhysicsCastDistance = serializedObject.FindProperty("m_PhysicsCastDistance");
            m_PhysicsEventMask = serializedObject.FindProperty("m_PhysicsEventMask");
            s_GraphicTags = serializedObject.FindProperty("s_GraphicTags");
            /// RaycastRing options
            m_PointerRingWidth = serializedObject.FindProperty("m_PointerRingWidth");
            m_PointerCircleRadius = serializedObject.FindProperty("m_PointerCircleRadius");
            m_PointerDistance = serializedObject.FindProperty("m_PointerDistance");
            m_PointerColor = serializedObject.FindProperty("m_PointerColor");
            m_ProgressColor = serializedObject.FindProperty("m_ProgressColor");
            m_PointerMaterial = serializedObject.FindProperty("m_PointerMaterial");
            m_PointerRenderQueue = serializedObject.FindProperty("m_PointerRenderQueue");
            m_PointerSortingOrder = serializedObject.FindProperty("m_PointerSortingOrder");
            m_TimeToGaze = serializedObject.FindProperty("m_TimeToGaze");
            /// GazeRaycastRing options
            m_EyeTracking = serializedObject.FindProperty("m_EyeTracking");
            m_Eye = serializedObject.FindProperty("m_Eye");
            m_InputEvent = serializedObject.FindProperty("m_InputEvent");
            m_ControlKey = serializedObject.FindProperty("m_ControlKey");
            m_ControlHand = serializedObject.FindProperty("m_ControlHand");
#if ENABLE_INPUT_SYSTEM
            m_RotationInput = serializedObject.FindProperty("m_RotationInput");
#endif
            m_AlwaysEnable = serializedObject.FindProperty("m_AlwaysEnable");
        }
        bool PhysicsRaycasterOptions = false, /*GraphicRaycasterOptions = false, */RingOptions = false, EyeTrackingOptions = false, GazeOptions = true;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GazeRaycastRing myScript = target as GazeRaycastRing;

            PhysicsRaycasterOptions = EditorGUILayout.Foldout(PhysicsRaycasterOptions, "Physics Raycaster Settings");
            if (PhysicsRaycasterOptions)
            {
                EditorGUILayout.PropertyField(m_IgnoreReversedGraphics);
                EditorGUILayout.PropertyField(m_PhysicsCastDistance);
                EditorGUILayout.PropertyField(m_PhysicsEventMask);
            }

            /*GraphicRaycasterOptions = EditorGUILayout.Foldout(GraphicRaycasterOptions, "Graphic Raycaster Settings");
            if (GraphicRaycasterOptions)
            {
                EditorGUILayout.HelpBox(
                    "Keeps the tag list empty to ignore the \"Tag\" of a GameObject.\n" +
                    "If the tag list is NOT empty, only the GameObject with the specified \"Tag\" will respond to GraphicRaycaster.\n" +
                    "If you don't know what the \"Tag\" is, just keep empty.",
                    MessageType.Info);
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(s_GraphicTags);
                GUILayout.EndHorizontal();
            }*/

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            RingOptions = EditorGUILayout.Foldout(RingOptions, "Ring Settings");
            if (RingOptions)
            {
                EditorGUILayout.PropertyField(m_PointerRingWidth);
                EditorGUILayout.PropertyField(m_PointerCircleRadius);
                EditorGUILayout.PropertyField(m_PointerDistance);
                EditorGUILayout.PropertyField(m_PointerColor);
                EditorGUILayout.PropertyField(m_ProgressColor);
                EditorGUILayout.PropertyField(m_PointerMaterial);
                EditorGUILayout.PropertyField(m_PointerRenderQueue);
                EditorGUILayout.PropertyField(m_PointerSortingOrder);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EyeTrackingOptions = EditorGUILayout.Foldout(EyeTrackingOptions, "Eye Tracking Settings");
            if (EyeTrackingOptions)
            {
                EditorGUILayout.PropertyField(m_EyeTracking);
#if ENABLE_INPUT_SYSTEM
                myScript.UseInputAction = EditorGUILayout.Toggle("Use Input Action", myScript.UseInputAction);
                if (myScript.UseInputAction)
                {
                    EditorGUILayout.PropertyField(m_RotationInput);
                }
                else
                {
                    EditorGUILayout.PropertyField(m_Eye);
                }
#else
                EditorGUILayout.PropertyField(m_Eye);
#endif
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GazeOptions = EditorGUILayout.Foldout(GazeOptions, "Gaze Settings");
            if (GazeOptions)
            {
                // Moves m_TimeToGaze here thus developers can easily set the value.
                EditorGUILayout.PropertyField(m_TimeToGaze);
                EditorGUILayout.PropertyField(m_InputEvent);
                EditorGUILayout.PropertyField(m_ControlKey);
                EditorGUILayout.PropertyField(m_ControlHand);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.PropertyField(m_AlwaysEnable);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty((GazeRaycastRing)target);
        }
    }
}
#endif