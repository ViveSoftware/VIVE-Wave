// GENERATED AUTOMATICALLY FROM 'Assets/Samples/Wave/PureUnity/PureUnity/Common/KeyboardMouseControl/Scripts/WaveXR_KMCInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Wave.XR.Sample.KMC
{
    public class @WaveXR_KMCInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @WaveXR_KMCInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""WaveXR_KMCInput"",
    ""maps"": [
        {
            ""name"": ""mouse"",
            ""id"": ""b4f0013b-6844-4251-b956-56b25c4ba870"",
            ""actions"": [
                {
                    ""name"": ""axis"",
                    ""type"": ""Value"",
                    ""id"": ""60665d76-fe83-468f-bbf9-5c51fbf0b23e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ce7ca386-b815-47d9-a986-3e0eaa9849ce"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Wave PureUnity"",
                    ""action"": ""axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Wave PureUnity"",
            ""bindingGroup"": ""Wave PureUnity"",
            ""devices"": []
        }
    ]
}");
            // mouse
            m_mouse = asset.FindActionMap("mouse", throwIfNotFound: true);
            m_mouse_axis = m_mouse.FindAction("axis", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // mouse
        private readonly InputActionMap m_mouse;
        private IMouseActions m_MouseActionsCallbackInterface;
        private readonly InputAction m_mouse_axis;
        public struct MouseActions
        {
            private @WaveXR_KMCInput m_Wrapper;
            public MouseActions(@WaveXR_KMCInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @axis => m_Wrapper.m_mouse_axis;
            public InputActionMap Get() { return m_Wrapper.m_mouse; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
            public void SetCallbacks(IMouseActions instance)
            {
                if (m_Wrapper.m_MouseActionsCallbackInterface != null)
                {
                    @axis.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnAxis;
                    @axis.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnAxis;
                    @axis.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnAxis;
                }
                m_Wrapper.m_MouseActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @axis.started += instance.OnAxis;
                    @axis.performed += instance.OnAxis;
                    @axis.canceled += instance.OnAxis;
                }
            }
        }
        public MouseActions @mouse => new MouseActions(this);
        private int m_WavePureUnitySchemeIndex = -1;
        public InputControlScheme WavePureUnityScheme
        {
            get
            {
                if (m_WavePureUnitySchemeIndex == -1) m_WavePureUnitySchemeIndex = asset.FindControlSchemeIndex("Wave PureUnity");
                return asset.controlSchemes[m_WavePureUnitySchemeIndex];
            }
        }
        public interface IMouseActions
        {
            void OnAxis(InputAction.CallbackContext context);
        }
    }
}
#endif
