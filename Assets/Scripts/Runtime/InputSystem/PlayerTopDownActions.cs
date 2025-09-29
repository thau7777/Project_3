using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTopDownActions : InputActions.IPlayerTopDownActions
{
    public Action<Vector2> onMove;
    public Action onAttack;
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

}
