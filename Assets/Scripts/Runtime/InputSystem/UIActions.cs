using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActions : InputActions.IUIActions
{
    public Action<Vector2> onNavigate;
    public Action onSubmit;
    public Action onEscape;
    public Action<Vector2> onLook;
    public Action onPressAnyButton;

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var value = context.ReadValue<Vector2>();
            onNavigate?.Invoke(value);
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
            onSubmit?.Invoke();
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed)
            onEscape?.Invoke();
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
}
