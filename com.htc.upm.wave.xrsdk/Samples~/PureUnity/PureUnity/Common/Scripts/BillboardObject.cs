using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VRSStudio.Common
{

    public class BillboardObject : MonoBehaviour
    {
        [Tooltip("If target is null, Aim the Camera.main.")]
        public Transform target;
        private Transform tInternal;

        [Tooltip("If not checked, the euler angle X will not be updated.")]
        public bool axisX = true;
        public bool axisY = true;
        public bool axisZ = true;

        [Tooltip("If inverse, this object's backward direction will face the target.")]
        public bool inverse;

        // How fast the object follow target
        public float updateTime;

        Timer updateTimer = new Timer(0.2f);

        private Quaternion targetRotation = Quaternion.identity;

        private void Start()
        {
            tInternal = target;
            updateTimer.Set(updateTime);
        }

        void CheckTarget()
        {
            if (target != null)
            {
                tInternal = target;
                return;
            }
            if (Camera.main == null) return;
            tInternal = Camera.main.transform;
        }

        public void UpdateRotation(float t)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, t);
        }

        public void DoBillboard()
        {
            CheckTarget();
            if (tInternal == null) return;

            Vector3 lookDir = (tInternal.position - transform.position).normalized;
            // world rotation
            var rotW = Quaternion.LookRotation(lookDir, Vector3.up);
            // to this object's local space
            var rotL = Quaternion.Inverse(transform.parent.rotation) * rotW;

            if (inverse) rotL *= Quaternion.Euler(0f, 180f, 0f);

            var orgAngles = transform.localRotation.eulerAngles;
            var newAngles = rotL.eulerAngles;

            // If not allow modify, use original rotation.
            if (!axisX) newAngles.x = orgAngles.x;
            if (!axisY) newAngles.y = orgAngles.y;
            if (!axisZ) newAngles.z = orgAngles.z;

            rotL.eulerAngles = newAngles;

            targetRotation = rotL;

            if (tInternal != target)
                tInternal = null;
        }

        void Update()
        {
            if (updateTimer.Check())
            {
                updateTimer.Set();
                DoBillboard();
            }
            UpdateRotation(updateTimer.Progress() * 0.95f + 0.05f);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BillboardObject))]
    public class BillboardObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Trigger Billboard"))
            {
                BillboardObject billboardObject = (BillboardObject)target;
                Undo.RecordObject(billboardObject.transform, "DoBillboard");
                billboardObject.DoBillboard();
                billboardObject.UpdateRotation(1);
            }
        }
    }
#endif
}
