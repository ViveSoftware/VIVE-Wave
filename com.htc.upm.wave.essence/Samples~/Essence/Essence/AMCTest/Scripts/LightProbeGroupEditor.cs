using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(LightProbeGroup))]
public class LightProbeGroupEditor : MonoBehaviour
{
}

#if UNITY_EDITOR

[CustomEditor(typeof(LightProbeGroupEditor))]
public class LightProbeGroupEditorEditor : Editor
{
	SerializedProperty lookAtPoint;

	void OnEnable()
	{
		lookAtPoint = serializedObject.FindProperty("lookAtPoint");
	}

	public override void OnInspectorGUI()
	{
		GUILayout.BeginVertical();
		if (GUILayout.Button("Fill Light Probes"))
		{
			LightProbeGroupEditor instance = (LightProbeGroupEditor)serializedObject.targetObject;
			LightProbeGroup lpg = instance.GetComponent<LightProbeGroup>();
			List<Vector3> positions = new List<Vector3>();
			Matrix4x4 mat = lpg.transform.localToWorldMatrix;
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					for (int k = 0; k < 10; k++)
					{
						positions.Add(mat * (new Vector3(i - 5, j - 5, k - 5)));
					}
				}
			}
			lpg.probePositions = positions.ToArray();
		}
		GUILayout.EndVertical();
	}
}

#endif
