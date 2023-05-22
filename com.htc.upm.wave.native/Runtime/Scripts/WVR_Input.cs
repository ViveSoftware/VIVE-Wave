// GENERATED AUTOMATICALLY FROM 'Packages/com.htc.upm.wave.native/Runtime/Scripts/WVR_Input.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Wave.Native
{
    public class @WVR_Input : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @WVR_Input()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""WVR_Input"",
    ""maps"": [
        {
            ""name"": ""mouse"",
            ""id"": ""0d530d8d-74a4-45d7-99e6-71c645e26595"",
            ""actions"": [
                {
                    ""name"": ""axis"",
                    ""type"": ""Value"",
                    ""id"": ""f7a4cffd-4f5a-4017-8acc-fed4ebbd61bf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a91aa86a-743b-459b-96a0-db196ddacb0b"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""WVR Native"",
                    ""action"": ""axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""WVR Native"",
            ""bindingGroup"": ""WVR Native"",
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
            private @WVR_Input m_Wrapper;
            public MouseActions(@WVR_Input wrapper) { m_Wrapper = wrapper; }
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
        private int m_WVRNativeSchemeIndex = -1;
        public InputControlScheme WVRNativeScheme
        {
            get
            {
                if (m_WVRNativeSchemeIndex == -1) m_WVRNativeSchemeIndex = asset.FindControlSchemeIndex("WVR Native");
                return asset.controlSchemes[m_WVRNativeSchemeIndex];
            }
        }
        public interface IMouseActions
        {
            void OnAxis(InputAction.CallbackContext context);
        }
    }
}
#endif
