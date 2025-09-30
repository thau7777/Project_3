using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTopDownActions : InputActions.IPlayerTopDownActions
{
    public Action<Vector2> onMove;
    public Action onAttack;
    public Action<bool> onAim;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            onAttack?.Invoke();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        onMove?.Invoke(value);
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        if(context.performed)
            onAim?.Invoke(true);
        else if(context.canceled)
            onAim?.Invoke(false);
    }
}
