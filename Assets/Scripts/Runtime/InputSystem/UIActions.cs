using MyRule;
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
    public Action<int> onNavigateTab;
    public Action onOpenTabView;

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
