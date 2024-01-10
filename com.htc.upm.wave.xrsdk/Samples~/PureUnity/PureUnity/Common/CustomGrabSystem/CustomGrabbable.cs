using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Wave.XR.Sample.Input
{
	[RequireComponent(typeof(Rigidbody))]
    public class CustomGrabbable : MonoBehaviour
    {
        public enum GrabPoseType { Neutral, Left, Right }

        [Tooltip("Unless first grabbed, this object will not move.")]
        public bool keepObjectStill = true;

        [Tooltip("When released, this object keep orignal kinematic state.  After grabbered, if not keep and orignal is kinematic, the object will be released as no isKinematic.")]
        public bool keepObjectKinematic = true;

        public bool IsKinematic { get; set; }

        private Rigidbody rb;

        [Tooltip("CenterOfMassRef will be used to update rigidbody.centerOfMass. Must be a child of this object. Scale must be Vector.one.")]
        [SerializeField]
        Transform centerOfMassRef;
        public Vector3 centerOfMass;

        [Tooltip("Effect: grabPoseNRef, grabPoseRRef, grabPoseLRef")]
        public bool useFixedPose = false;

        [Tooltip("how to hold this object.  Let the grabPose's forward match grabber's forward. Must be a child of this object. Scale must be Vector.one.")]
        [SerializeField]
        Transform grabPoseNRef;
        public Pose grabPoseN;

        [Tooltip("how to hold this object.  Let the grabPose's forward match grabber's forward. Must be a child of this object. Scale must be Vector.one.")]
        [SerializeField]
        Transform grabPoseRRef;
        public Pose grabPoseR;

        [Tooltip("how to hold this object.  Let the grabPose's forward match grabber's forward. Must be a child of this object. Scale must be Vector.one.")]
        [SerializeField]
        Transform grabPoseLRef;
        public Pose grabPoseL;

        public bool IsGrabbed { get; internal set; }
        public CustomGrabber Grabber { get; internal set; }

        public Rigidbody GetRigidbody() { return rb; }

        public Transform GetCenterOfMassRef() { return centerOfMassRef; }
        public Vector3 GetCenterOfMass() { return centerOfMass; }

        public UnityEvent OnGrabbed;
        public UnityEvent OnReleased;

        // This func will never return null.
        public Transform GetGrabPoseRef(GrabPoseType type = GrabPoseType.Neutral)
        {
            switch (type)
            {
                case GrabPoseType.Left:
                    if (grabPoseLRef == null) break;
                    return grabPoseLRef;
                case GrabPoseType.Right:
                    if (grabPoseRRef == null) break;
                    return grabPoseRRef;
            }
            return grabPoseNRef;
        }

        public Pose GetGrabPose(GrabPoseType type = GrabPoseType.Neutral)
        {
            switch (type)
            {
                case GrabPoseType.Left:
                    return grabPoseL;
                case GrabPoseType.Right:
                    return grabPoseR;
            }
            return grabPoseN;
        }


        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (centerOfMassRef != null)
                rb.centerOfMass = centerOfMass = centerOfMassRef.localPosition;
            else
                rb.centerOfMass = centerOfMass;
            rb.ResetInertiaTensor();
            IsKinematic = rb.isKinematic;

            if (grabPoseNRef != null)
            {
                grabPoseN = new Pose(grabPoseNRef.localPosition, grabPoseNRef.localRotation);
            }
            else
            {
                grabPoseNRef = transform;
            }

            if (grabPoseRRef != null)
            {
                grabPoseR = new Pose(grabPoseRRef.localPosition, grabPoseRRef.localRotation);
            }
            else
                grabPoseR = grabPoseN;

            if (grabPoseLRef != null)
            {
                grabPoseL = new Pose(grabPoseLRef.localPosition, grabPoseLRef.localRotation);
            }
            else
                grabPoseL = grabPoseN;

            gameObject.tag = "grabbable";

            if (keepObjectStill)
            {
                rb.Sleep();
            }
        }

        private void OnValidate()
        {
            if (centerOfMassRef != null)
            {
                if (centerOfMassRef.parent != transform)
                {
                    centerOfMassRef = null;
                    return;
                }
                centerOfMass = centerOfMassRef.localPosition;
                //centerOfMassRef = null;
            }

            if (grabPoseNRef != null)
            {
                grabPoseN = new Pose(grabPoseNRef.localPosition, grabPoseNRef.localRotation);
                if (grabPoseNRef.parent != transform)
                {
                    grabPoseNRef = null;
                    return;
                }
                grabPoseN = new Pose(grabPoseNRef.localPosition, grabPoseNRef.localRotation);
            }

            if (grabPoseRRef != null)
            {
                if (grabPoseRRef.parent != transform)
                {
                    grabPoseRRef = null;
                    return;
                }
                grabPoseR = new Pose(grabPoseRRef.localPosition, grabPoseRRef.localRotation);
            }
            else
                grabPoseR = grabPoseN;

            if (grabPoseLRef != null)
            {
                if (grabPoseLRef.parent != transform)
                {
                    grabPoseLRef = null;
                    return;
                }
                grabPoseL = new Pose(grabPoseLRef.localPosition, grabPoseLRef.localRotation);
            }
            else
                grabPoseL = grabPoseN;

			if (Application.isEditor && !Application.isPlaying)
			{
				if (gameObject.tag != "grabbable")
					gameObject.tag = "grabbable";
			}
        }
    }
}
