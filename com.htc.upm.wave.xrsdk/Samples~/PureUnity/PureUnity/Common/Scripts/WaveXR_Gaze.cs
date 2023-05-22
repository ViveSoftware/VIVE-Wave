using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

// Copy code from XRAcceptance GazeInput.cs
[RequireComponent(typeof(MeshRenderer))]
public class WaveXR_Gaze : MonoBehaviour
{
	List<RaycastResult> m_RaycastResults;
	Camera m_Camera;
	EventSystem m_EventSystem;
	MeshRenderer m_MeshRenderer;
	Material m_Material;

	public Color disableColor;
	public Color enableColor;
	[Range(1, 20), Tooltip("In meter")]
	public float defaultDistance = 15.0f;
	[Range(0.1f, 10), Tooltip("In meter")]
	public float spriteMeshSize = 1;
	[Range(1f, 20), Tooltip("In degree.  How big does gaze sprite look like.")]
	public float degreeFOV = 2;

	void Start()
	{
		m_RaycastResults = new List<RaycastResult>();
		m_Camera = Camera.main;
		m_EventSystem = FindObjectOfType<EventSystem>();
		m_MeshRenderer = GetComponent<MeshRenderer>();
		m_Material = m_MeshRenderer.material;
	}

	void Update()
	{
		if (!m_EventSystem)
			return;

		var pointerData = new PointerEventData(m_EventSystem);

		pointerData.position = new Vector2(m_Camera.scaledPixelWidth * 0.5f, m_Camera.scaledPixelHeight * 0.5f);
		m_RaycastResults.Clear();
		m_EventSystem.RaycastAll(pointerData, m_RaycastResults);

		if (m_RaycastResults.Count == 0)
		{
			m_Material.SetColor("_Color", disableColor);
			float scale = Mathf.Tan(Mathf.Deg2Rad * degreeFOV / 2) * defaultDistance / spriteMeshSize;
			transform.position = m_Camera.transform.position + m_Camera.transform.forward * defaultDistance;
			transform.localScale = new Vector3(scale, scale, 1);
			transform.LookAt(m_Camera.transform);

		}
		else
		{
			m_Material.SetColor("_Color", enableColor);

			var closestResult = new RaycastResult { distance = float.MaxValue };
			foreach (var r in m_RaycastResults)
			{
				if (r.distance < closestResult.distance)
					closestResult = r;
			}

			transform.position = m_Camera.transform.position + m_Camera.transform.forward * (closestResult.distance - 0.1f);
			float scale = Mathf.Tan(Mathf.Deg2Rad * degreeFOV / 2) * (closestResult.distance - 0.1f) / spriteMeshSize;
			transform.localScale = new Vector3(scale, scale, 1);
			transform.LookAt(m_Camera.transform);
		}
	}
}
