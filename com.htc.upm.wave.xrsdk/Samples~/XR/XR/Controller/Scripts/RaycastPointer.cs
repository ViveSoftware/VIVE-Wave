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
using UnityEngine.EventSystems;

namespace Wave.XR.Sample.Controller
{
	[RequireComponent(typeof(LineRenderer))]
	public class RaycastPointer : RaycastImpl
	{
		const string LOG_TAG = "Wave.Essence.Raycast.RaycastPointer";
		void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }

		#region Inspector
		[Tooltip("To show the ray which presents the casting direction.")]
		[SerializeField]
		private bool m_ShowRay = false;
		public bool ShowRay { get { return m_ShowRay; } set { m_ShowRay = value; } }

		[SerializeField]
		private float m_RayStartWidth = 0.01f;
		public float RayStartWidth { get { return m_RayStartWidth; } set { m_RayStartWidth = value; } }

		[SerializeField]
		private float m_RayEndWidth = 0.01f;
		public float RayEndWidth { get { return m_RayEndWidth; } set { m_RayEndWidth = value; } }

		[SerializeField]
		private Material m_RayMaterial = null;
		public Material RayMaterial { get { return m_RayMaterial; } set { m_RayMaterial = value; } }

		[SerializeField]
		private GameObject m_Pointer = null;
		public GameObject Pointer { get { return m_Pointer; } set { m_Pointer = value; } }
		#endregion

		LineRenderer m_Ray = null;
		Vector3 m_PointerScale = new Vector3(.15f, .15f, .15f);

		#region MonoBehaviour overrides
		protected override void OnEnable()
		{
			DEBUG("OnEnable()");
			base.OnEnable();

			if (m_Ray == null) { m_Ray = GetComponent<LineRenderer>(); }
			if (m_Pointer != null)
			{
				m_PointerScale = m_Pointer.transform.localScale;
				DEBUG("OnEnable() Get default pointer scale (" + m_PointerScale.x + ", " + m_PointerScale.y + ", " + m_PointerScale.z + ")");
			}
		}
		protected override void OnDisable()
		{
			DEBUG("OnDisable()");
			base.OnDisable();

			ActivatePointer(false);
			ActivateRay(false);
		}
		protected override void Update()
		{
			/// Raycast
			base.Update();

			if (!IsInteractable()) { return; }

			/// Draw the ray and pointer.
			DrawRayPointer();
		}
		#endregion

		#region Ray and Pointer
		private void ActivatePointer(bool active)
		{
			if (m_Pointer != null) { m_Pointer.SetActive(active); }
		}
		private void ActivateRay(bool active)
		{
			if (m_Ray != null) { m_Ray.enabled = active; }
		}

		private Vector3 GetIntersectionPosition(Camera cam, RaycastResult raycastResult)
		{
			if (cam == null)
				return Vector3.zero;

			float intersectionDistance = raycastResult.distance + cam.nearClipPlane;
			Vector3 intersectionPosition = cam.transform.forward * intersectionDistance + cam.transform.position;
			return intersectionPosition;
		}

		Vector3 rayStart = Vector3.zero, rayEnd = Vector3.zero;
		const float kRayLengthMin = 0.5f;
		private float m_RayLength = 10;
		protected Vector3 pointerPosition = Vector3.zero;
		private void DrawRayPointer()
		{
			Vector3 hit = GetIntersectionPosition(eventCamera, pointerData.pointerCurrentRaycast);

			rayStart = transform.position;
			if (raycastObject != null)
			{
				m_RayLength = Vector3.Distance(hit, rayStart);
				m_RayLength = m_RayLength > kRayLengthMin ? m_RayLength : kRayLengthMin;
			}

			if (LockPointer())
			{
				Vector3 middle = new Vector3(0, 0, (m_RayLength - 0.2f) / 4);
				DrawCurveRay(rayStart, middle, rayEnd, m_RayStartWidth, m_RayEndWidth, m_RayMaterial);
			}
			else
			{
				rayEnd = rayStart + (transform.forward * (m_RayLength - 0.2f));
				pointerPosition = rayStart + (transform.forward * m_RayLength);
				DrawRay(rayStart, rayEnd, m_RayStartWidth, m_RayEndWidth, m_RayMaterial);
			}

			DrawPointer(pointerPosition);
		}
		const float kPointerDistance = 10;
		private void DrawPointer(Vector3 position)
		{
			if (m_Pointer == null) { return; }

			m_Pointer.transform.position = position;
			m_Pointer.transform.rotation = Camera.main.transform.rotation;
			float distance = Vector3.Distance(position, Camera.main.transform.position);
			m_Pointer.transform.localScale = m_PointerScale * (distance / kPointerDistance);
		}

		private void DrawRay(Vector3 start, Vector3 end, float startWidth, float endWidth, Material material)
		{
			if (m_Ray == null) { return; }

			Vector3[] positions = new Vector3[] { start, end };
			m_Ray.positionCount = positions.Length;
			m_Ray.SetPositions(positions);
			m_Ray.startWidth = startWidth;
			m_Ray.endWidth = endWidth;
			m_Ray.material = material;
			m_Ray.useWorldSpace = true;
		}

		private void DrawCurveRay(Vector3 start, Vector3 middle, Vector3 end, float startWidth, float endWidth, Material material)
		{
			if (m_Ray == null) { m_Ray = GetComponent<LineRenderer>(); }

			Vector3[] positions = GenerateBezierCurve3(50, start, middle, end);
			m_Ray.positionCount = positions.Length;
			m_Ray.SetPositions(positions);
			m_Ray.startWidth = startWidth;
			m_Ray.endWidth = endWidth;
			m_Ray.material = material;
			m_Ray.useWorldSpace = true;
		}
		Vector3[] GenerateBezierCurve2(int iteration, Vector3 start, Vector3 end)
		{
			Vector3[] points = new Vector3[iteration + 1];
			for (int i = 0; i < iteration + 1; i++)
			{
				points.SetValue(start + ((end - start).normalized * (end - start).magnitude * i / iteration), i);
			}
			return points;
		}
		Vector3[] GenerateBezierCurve3(int iteration, Vector3 start, Vector3 middle, Vector3 end)
		{
			Vector3[] points1 = GenerateBezierCurve2(iteration, start, middle);
			Vector3[] points2 = GenerateBezierCurve2(iteration, start, end);
			Vector3[] points = new Vector3[iteration + 1];
			for (int i = 0; i < iteration + 1; i++)
			{
				points.SetValue(points1[i] + ((points2[i] - points1[i]).normalized * (points2[i] - points1[i]).magnitude * i / iteration), i);
			}
			return points;
		}
		#endregion

		private bool IsInteractable()
		{
			ActivatePointer(m_Interactable);
			ActivateRay(m_Interactable && m_ShowRay);

			return m_Interactable;
		}

		/// <summary> For DrawRayPointer(), controls whether locking the pointer or not. </summary>
		protected virtual bool LockPointer()
		{
			return false;
		}
	}
}
