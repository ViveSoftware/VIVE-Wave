// Lucifer
// https://zhuanlan.zhihu.com/p/159913409
// https://zhuanlan.zhihu.com/p/163095303

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Renderer))]
public class BottleLiquid : MonoBehaviour
{
    private Material bottleLiquidMat;
    private BoxCollider bottleBox;
    private Vector4[] localPos = new Vector4[6];

    [SerializeField]
    float meshScale = 0.98f;

    [Tooltip("The rigbody will effect this liquid.  Can be null.  Used to calculate the gravity direction.")]
    public Rigidbody rb;

    private float noise;

    void initializeVolum()
    {
        bottleBox = GetComponent<BoxCollider>();
        Vector4 centerPos = bottleBox.center * meshScale;
        Vector4 boxSize = bottleBox.size * meshScale;
        centerPos.w = 1;
        boxSize.w = 0;

        var right = new Vector4(1, 0, 0, 0);
        var up = new Vector4(0, 1, 0, 0);
        var forward = new Vector4(0, 0, 1, 0);

        // +X -X +Y -Y +Z -Z
        localPos[0] = centerPos + 0.5f * boxSize.x * right;
        localPos[1] = centerPos - 0.5f * boxSize.x * right;
        localPos[2] = centerPos + 0.5f * boxSize.y * up;
        localPos[3] = centerPos - 0.5f * boxSize.y * up;
        localPos[4] = centerPos + 0.5f * boxSize.z * forward;
        localPos[5] = centerPos - 0.5f * boxSize.z * forward;
    }

    private void OnEnable()
    {
        bottleLiquidMat = GetComponent<Renderer>().material;
        if (bottleLiquidMat.HasProperty("_BBoxPX"))
        {
            initializeVolum();
            bottleLiquidMat.SetVector("_BBoxPX", localPos[0]);
            bottleLiquidMat.SetVector("_BBoxNX", localPos[1]);
            bottleLiquidMat.SetVector("_BBoxPY", localPos[2]);
            bottleLiquidMat.SetVector("_BBoxNY", localPos[3]);
            bottleLiquidMat.SetVector("_BBoxPZ", localPos[4]);
            bottleLiquidMat.SetVector("_BBoxNZ", localPos[5]);

            bottleLiquidMat.SetFloat("_MeshScale", meshScale);
        }
        else
            enabled = false;
    }

    Vector3 lastV = Vector3.zero;
    Vector3 lastAV = Vector3.zero;
    float lastT;

    //private void FixedUpdate()
    //{
    //    if (rb == null) return;
    //    if (noise == 0) return;

    //    if (velocityInit)
    //    {
    //        lastV = rb.velocity;
    //        lastAV = rb.angularVelocity;
    //        lastT = Time.fixedUnscaledTime;
    //        velocityInit = false;
    //        return;
    //    }

    //    if (rb && Time.fixedUnscaledTime != lastT)
    //    {
    //        float dt = Time.fixedUnscaledTime - lastT;
    //        Vector3 linearAcceleration = (rb.velocity - lastV) / dt;
    //        Vector3 angularAcceleration = (rb.angularVelocity - lastAV) * rb.mass / dt;
    //        Vector3 totalAcceleration = -linearAcceleration - angularAcceleration + 9.8f * Vector3.up;
    //        //Vector3 totalAcceleration = -linearAcceleration;
    //        gravityNormal = totalAcceleration.normalized;

    //        Debug.Log("Set GravityUp: " + gravityNormal + " " + linearAcceleration.x * 100 + " rb.velocity " + rb.velocity);

    //        lastV = rb.velocity;
    //        lastAV = rb.angularVelocity;
    //        lastT = Time.fixedUnscaledTime;
    //    }
    //}

    //bool velocityInit = false;

    //Vector3 gravityNormal = Vector3.up;

    void Update()
    {

        if (transform.hasChanged)
        {
            //if (rb)
            //    velocityInit = true;
            transform.hasChanged = false;
            noise = 1;
        }
        else
        {
            if (noise > 0)
            {
                noise = Mathf.Max(noise - Time.deltaTime / 5, 0);
                bottleLiquidMat.SetFloat("_Noise", noise);
                //if (rb)
                //{
                //    if (noise > 0)
                //    {
                //        bottleLiquidMat.SetVector("_GravityUp", gravityNormal);
                //    }
                //    else
                //    {
                //        bottleLiquidMat.SetVector("_GravityUp", Vector3.up);
                //        velocityInit = false;
                //    }
                //}
                
            }
        }
    }
}