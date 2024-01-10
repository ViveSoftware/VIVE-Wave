using UnityEngine;
//using Wave.Native;

namespace VRSStudio.Common
{
    class Timer
    {
        //private const string TAG = "Timer";
        public float time;
        public float period;
        public bool IsSet { get; private set; }
        public bool IsPaused { get; private set; }

        public Timer(float sec)
        {
            if (sec < Mathf.Epsilon * 2)
            {
                period = 0;
                IsPaused = true;
                return;
            }
            period = sec;
        }

        // Start/Go/Begin and update the check time.
        public void Set(float sec)
        {
            //Log.d(TAG, "Timer Set", true);
            time = Time.unscaledTime + sec;
            if (sec < Mathf.Epsilon * 2)
            {
                period = 0;
                IsPaused = true;
                IsSet = true;
                return;
            }
            period = sec;
            IsSet = true;
            IsPaused = false;
        }

        // Start/Go/Begin
        public void Set()
        {
            //Log.d(TAG, "Timer Set", true);
            time = Time.unscaledTime + period;
            if (period < Mathf.Epsilon * 2)
            {
                period = 0;
                IsPaused = true;
                IsSet = true;
                return;
            }
            IsSet = true;
            IsPaused = false;
        }

        // Reset timer but not start.
        public void Reset()
        {
            //Log.d(TAG, "Timer Reset", true);
            IsSet = false;
            IsPaused = false;
        }

        public void Pause()
        {
            //Log.d(TAG, "Timer Pause", true);
            IsPaused = true;
        }

        // Check time out
        public bool Check()
        {
            //Log.d(TAG, "Timer Check = " + (time < Time.unscaledTime));
            if (!IsSet) return false;
            var c = Time.unscaledTime > time;
            if (c)
                IsPaused = true;
            return c;
        }

        // progress from 0 to 1
        public float Progress()
        {
            if (period < Mathf.Epsilon * 2)
                return 1;
            return Mathf.Clamp01(1 - ((time - Time.unscaledTime) / period));
        }
    }
}