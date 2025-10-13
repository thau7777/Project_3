using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTopDownActions : InputActions.IPlayerTopDownActions
{
    public Action<Vector2> onMove;
    public Action onLeftClick;
    public Action<bool> onRightClick;

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            onLeftClick?.Invoke();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        onMove?.Invoke(value);
    }
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if(context.performed)
            onRightClick?.Invoke(true);
        else if(context.canceled)
            onRightClick?.Invoke(false);
    }
}
