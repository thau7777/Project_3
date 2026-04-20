using MyRule;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActions : InputActions.IUIActions
{
    public event Action<Vector2> onMove;
    public event Action onSubmit;
    public event Action onCancel;
    public event Action<Vector2> onLook;
    public event Action onPressAnyButton;
    public event Action<Vector2> onAdjust;
    public event Action<int> onNavigateTab;
    public event Action onOpenTabView;
    public event Action<Vector2> onNavigate;

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
        {
            onCancel?.Invoke();
            Navigator.OnCancelPress();
        }
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

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var value = context.ReadValue<Vector2>();
            onNavigate?.Invoke(value);
        }
    }

    public void OnPreviousTab(InputAction.CallbackContext context)
    {
        if (context.performed) onNavigateTab?.Invoke(-1);
    }

    public void OnNextTab(InputAction.CallbackContext context)
    {
        if (context.performed) onNavigateTab?.Invoke(1);
    }

    public void OnOpenTabView(InputAction.CallbackContext context)
    {
        if (context.performed)
            onOpenTabView?.Invoke();
    }
}
