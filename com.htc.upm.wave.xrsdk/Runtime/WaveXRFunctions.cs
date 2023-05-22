using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Wave.XR.Loader;
using static Wave.XR.Constants;

namespace Wave.XR.Function
{
    public static class Functions
    {
        public delegate void SetFoveatedRenderingParameterDelegate(uint eye, float fov, uint quality, float ndcFocalPointX, float ndcFocalPointY);
        public static SetFoveatedRenderingParameterDelegate SetFoveatedRenderingParameter = null;
    }

    public static class FunctionsHelper
    {
        [DllImport("wvrunityxr", EntryPoint = "GetFuncPtr")]
        internal static extern ErrorCode GetFuncPtr(string name, ref System.IntPtr value);

        [DllImport("wvrunityxr", EntryPoint = "SetFuncPtr")]
        internal static extern ErrorCode SetFuncPtr(string name, System.IntPtr value);

        public static System.IntPtr GetFuncPtr(string name)
        {
            System.IntPtr ptr = new System.IntPtr();
            ErrorCode ec = GetFuncPtr(name, ref ptr);
            Debug.Log("GetFuncPtr(" + name + ", [out]" + ptr + ")=" + ec);
            if (ec != ErrorCode.NoError)
            {
                return System.IntPtr.Zero;
            }

            return ptr;
        }

        public static T GetFuncPtr<T>(string name)
        {
            System.IntPtr ptr = new System.IntPtr();
            ErrorCode ec = GetFuncPtr(name, ref ptr);
            Debug.Log("GetFuncPtr<" + typeof(T) + ">(" + name + ", [out]" + ptr + ")=" + ec);
            if (ec != ErrorCode.NoError)
            {
                return default;
            }

            return Marshal.GetDelegateForFunctionPointer<T>(ptr);
        }

        public static bool SetFuncPtr<T>(string name, T d) where T: System.Delegate
        {
            System.IntPtr ptr = Marshal.GetFunctionPointerForDelegate<T>(d);
            ErrorCode ec = SetFuncPtr(name, ptr);
            Debug.Log("SetFuncPtr<" + typeof(T) + ">(" + name + ", " + ptr + ")=" + ec);
            return ec == ErrorCode.NoError;
        }

        internal static void Process(WaveXRLoader loader)
        {
    #if UNITY_EDITOR
            if (Application.isEditor)
                return;
    #endif

            if (Functions.SetFoveatedRenderingParameter == null)
                Functions.SetFoveatedRenderingParameter = GetFuncPtr<Functions.SetFoveatedRenderingParameterDelegate>("SetFoveatedRenderingParameter");
        }
    }
}
