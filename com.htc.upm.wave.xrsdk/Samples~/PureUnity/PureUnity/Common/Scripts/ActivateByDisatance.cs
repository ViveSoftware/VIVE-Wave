using UnityEngine;
using UnityEngine.Events;
using Wave.XR.Sample.Input;

public class ActivateByDisatance : MonoBehaviour
{
    public float distance = 2.0f;
    public float hysteresis = 0.5f;
    public Transform rig;

    public UnityEvent onEnter;
    public UnityEvent onLeave;

    public Button isEntered = new Button();
    public Button isLeaved = new Button();
    float currDistance = 0;

    Transform GetRig()
    {
        //if (WaveRig.Instance != null) return WaveRig.Instance.transform;
        if (Camera.main != null) return Camera.main.transform;
        return null;
    }

    void Update()
    {
        if (rig == null)
            rig = GetRig();
        if (!rig) return;

        var rigPos = rig.position;
        rigPos.y = 0;
        var thisPos = transform.position;
        thisPos.y = 0;

        currDistance = Vector3.Distance(rigPos, thisPos);

        var t1 = distance;
        var t2 = distance + hysteresis;
        isEntered.Set((currDistance < t1) || ((currDistance < t2) && isEntered.IsPressed));

        if (isEntered.IsDown)
        {
            onEnter.Invoke();
        }
        if (isEntered.IsUp)
        {
            onLeave.Invoke();
        }
    }
}
