using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActions : InputActions.IUIActions
{
    public Action<Vector2> onMove;
    public Action onSubmit;
    public Action onCancel;
    public Action<Vector2> onLook;
    public Action onPressAnyButton;
    public Action<Vector2> onAdjust;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var value = context.ReadValue<Vector2>();
            onMove?.Invoke(value);
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
            onSubmit?.Invoke();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
            onCancel?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var value = context.ReadValue<Vector2>();
            onLook?.Invoke(value);
        }
    }

    public void OnPressAnyButton(InputAction.CallbackContext context)
    {
        if (context.performed)
            onPressAnyButton?.Invoke();
    }

    public void OnAdjust(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var value = context.ReadValue<Vector2>();
            onAdjust?.Invoke(value);
        }
    }
}
