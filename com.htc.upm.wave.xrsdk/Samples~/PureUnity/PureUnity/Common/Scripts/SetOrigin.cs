using UnityEngine;
using UnityEngine.XR;

namespace Wave.XR.Sample
{
    public class SetOrigin : MonoBehaviour
    {
        public TrackingOriginModeFlags origin = TrackingOriginModeFlags.Device;

        // Start is called before the first frame update
        void Start()
        {
            if (Utils.InputSubsystem != null)
                Utils.InputSubsystem.TrySetTrackingOriginMode(origin);
            else
                Debug.LogError("Can't set TrackingOriginMode to Device");
        }
    }
}
