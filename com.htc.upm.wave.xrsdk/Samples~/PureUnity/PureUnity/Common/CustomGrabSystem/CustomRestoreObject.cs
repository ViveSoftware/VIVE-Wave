using UnityEngine;
using VRSStudio.Common;

[RequireComponent(typeof(Rigidbody))]
public class CustomRestoreObject : MonoBehaviour
{
    // Unless first grabbed, this object will not move.
    public bool keepObjectStill = true;

    public float distance = 0.5f;
    public float time = 1f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    bool sleepCheck = false;
    bool freeFallCheck = false;

    bool resetAtLastFrame = false;

    readonly Timer timer = new Timer(1);
    void Update()
    {
        if (resetAtLastFrame)
        {
            if (keepObjectStill)
                rb.Sleep();
            resetAtLastFrame = false;
        }

        if (rb.isKinematic)
        {
            sleepCheck = false;
            freeFallCheck = false;
            timer.Reset();
            return;
        }

        var v = rb.velocity.sqrMagnitude;
        var av = rb.angularVelocity.sqrMagnitude;
        var sleepOrAlmostStill = (rb.IsSleeping() && v > 0.0001f) || (v < 0.0001f && av < 0.01f);
        if (!sleepCheck && sleepOrAlmostStill)
        {
            sleepCheck = true;
            timer.Set(time);
            return;
        }

        if (sleepCheck && !sleepOrAlmostStill)
        {
            sleepCheck = false;
            timer.Reset();
        }

        if (!freeFallCheck && v > 10000)
        {
            timer.Set(time);
            freeFallCheck = true;
            return;
        }

        if (freeFallCheck && v < 10000)
        {
            freeFallCheck = false;
            timer.Reset();
        }

        if (!timer.IsPaused && timer.Check())
        {
            sleepCheck = false;
            freeFallCheck = false;

            if (Vector3.Distance(transform.position, originalPosition) > distance)
            {
                rb.velocity = Vector3.zero;
                rb.position = originalPosition;
                rb.rotation = originalRotation;
                rb.ResetInertiaTensor();
                resetAtLastFrame = true;
            }
        }
    }
}
