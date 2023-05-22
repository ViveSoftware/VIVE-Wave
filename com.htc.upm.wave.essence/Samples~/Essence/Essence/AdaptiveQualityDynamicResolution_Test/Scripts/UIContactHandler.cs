using UnityEngine;
using UnityEngine.EventSystems;

namespace AssetBundleTool
{
	/*
		How to use:
		1. Create a GameObject with Button in a Canvas
		2. Attach this script to the Button's GameObject
		3. Make a GameObject, named it "Trigger"
		4. Add a collider and set IsTrigger as false
		5. Add Rigidbody and set IsKinematic as true
		6. Try touch the Button with the Trigger GameObject in Play mode.
	*/
	[RequireComponent(typeof(BoxCollider), typeof(ColliderDetect), typeof(RectTransform))]
	public class UIContactHandler : MonoBehaviour, IColliderHandler
	{
		void OnEnable()
		{
			ColliderDetect.RegisterHandler(this);
		}

		void OnDisable()
		{
			ColliderDetect.UnregisterHandler(this);
		}

		public void ProcessContactEnter(Collision collision, ContactPoint cp)
		{
			//Debug.LogError("ProcessContactEnter " + cp.thisCollider.gameObject.name + "+" + cp.otherCollider.gameObject.name);
			if (cp.thisCollider.gameObject == gameObject || cp.otherCollider.gameObject == gameObject)
				ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
		}

		public void ProcessContactExit(Collision collision)
		{
		}

		public void RectTransformSize(RectTransform rectTransform)
		{
			//bool xFixed = false, yFixed = false;
			//xFixed = rectTransform.anchorMax.x == rectTransform.anchorMin.x;
			//yFixed = rectTransform.anchorMax.y == rectTransform.anchorMin.y;
		}


		void Start()
		{
			var rigidbody = GetComponent<Rigidbody>();
			rigidbody.useGravity = false;
			rigidbody.isKinematic = false;
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;

			var collider = GetComponent<BoxCollider>();
			var rectTransform = (RectTransform)transform;

			Vector3[] corners = new Vector3[4];
			rectTransform.GetLocalCorners(corners);

			Vector3 min = Vector3.Min(Vector3.Min(corners[0], corners[1]), Vector3.Min(corners[2], corners[3]));
			Vector3 max = Vector3.Max(Vector3.Max(corners[0], corners[1]), Vector3.Max(corners[2], corners[3]));

			var size = max - min;

			collider.size = size;
		}
	}
}
