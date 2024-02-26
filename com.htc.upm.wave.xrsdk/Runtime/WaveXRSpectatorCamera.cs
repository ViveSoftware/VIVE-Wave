using UnityEngine;

namespace Wave.XR
{
    public sealed class WaveXRSpectatorCamera : MonoBehaviour
    {
        private WaveXRSpectatorCameraHandle handle;

        public void SetHandle(WaveXRSpectatorCameraHandle handle)
        {
            this.handle = handle;
        }

        private void OnPostRender()
        {
            if (handle == null) { enabled = false; return; }
            handle.OnCameraPostRender();
        }
    }
}