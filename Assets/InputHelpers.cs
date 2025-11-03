using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;                 // New Input System
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public static class InputHelpers
{
    public static bool TryGetScreenTap(out Vector2 pos, bool ignoreUI = true)
    {
        pos = default;

        // EDITOR/PC: Mouse
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pos = Mouse.current.position.ReadValue();
            if (ignoreUI && EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(-1)) // mouse pointerId
                return false;
            return true;
        }
#endif
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            pos = Touchscreen.current.primaryTouch.position.ReadValue();
            if (ignoreUI && EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(0))  // primary touch id
                return false;
            return true;
        }

        return false;
    }
}