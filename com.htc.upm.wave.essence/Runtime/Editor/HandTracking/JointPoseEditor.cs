using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Wave.Essence.Hand.Editor
{
	[CustomEditor(typeof(JointPose))]
	public class JointPoseEditor : UnityEditor.Editor
	{
		SerializedProperty m_IsLeft, m_Joint, m_HideWhenPoseInvalid;
		private void OnEnable()
		{
			m_IsLeft = serializedObject.FindProperty("m_IsLeft");
			m_Joint = serializedObject.FindProperty("m_Joint");
			m_HideWhenPoseInvalid = serializedObject.FindProperty("m_HideWhenPoseInvalid");
		}
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.HelpBox(
				"Does this joint belong to the left hand? Unchecked for the right hand.",
				MessageType.Info);
			EditorGUILayout.PropertyField(m_IsLeft);

			EditorGUILayout.HelpBox(
				"Selects a joint.",
				MessageType.Info);
			EditorGUILayout.PropertyField(m_Joint);

			EditorGUILayout.HelpBox(
				"If this option is selected, this GameObject will be hidden when the selected joint has a invalid pose.",
				MessageType.Info);
			EditorGUILayout.PropertyField(m_HideWhenPoseInvalid);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
