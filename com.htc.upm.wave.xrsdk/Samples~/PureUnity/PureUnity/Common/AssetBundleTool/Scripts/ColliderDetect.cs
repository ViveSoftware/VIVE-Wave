using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleTool
{
    /*
        If you want to process the contact event from "Trigger", you can create a script to inherent this class.
        Child need call RegisterHandler in OnEnable like this.

        void OnEnable() { ColliderDetect.RegisterHandler(this); }
        void OnDisable() { ColliderDetect.UnregisterHandler(this); }
    */
    public interface IColliderHandler
    {
        void ProcessContactEnter(Collision collision, ContactPoint cp);
        void ProcessContactExit(Collision collision);
    }

    // You need a gameobject named as "Trigger" or with "__CM__Body".
    [RequireComponent(typeof(Rigidbody))]
    public class ColliderDetect : MonoBehaviour
    {
        public bool hit = false;
        public bool criticalHit = false;

        private void Start()
        {
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        //// Only for debug
        //private ContactPoint cp;
        //private void Update()
        //{
        //    Debug.DrawRay(cp.point, -cp.normal, Color.red);
        //}

        private static List<IColliderHandler> Handlers = new List<IColliderHandler>();
        public static void RegisterHandler(IColliderHandler handler)
        {
            if (handler == null)
                return;
            if (!Handlers.Contains(handler))
            {
                Debug.Log("ABT_CD: Add Handler " + ((MonoBehaviour)handler).name);
                Handlers.Add(handler);
            }
        }

        public static void UnregisterHandler(IColliderHandler handler)
        {
            if (handler == null)
                return;
            if (Handlers.Contains(handler))
            {
                Debug.Log("ABT_CD: Remove Handler " + ((MonoBehaviour)handler).name);
                Handlers.Remove(handler);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.otherCollider.name != "Trigger" && !contact.otherCollider.name.Contains("__CM__Body"))
                    continue;

                foreach (var handler in Handlers)
                {
                    handler.ProcessContactEnter(collision, contact);
                }
                return;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.name != "Trigger" && !collision.gameObject.name.Contains("__CM__Body"))
                return;

            foreach (var handler in Handlers)
            {
                handler.ProcessContactExit(collision);
            }
        }
    }
}