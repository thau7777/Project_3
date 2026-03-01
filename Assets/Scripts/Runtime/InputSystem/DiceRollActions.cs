using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceRollActions : InputActions.IDiceRollActions
{
    public event Action onTab;

    public event Action onRoll;

    public event Action onRightClick;

    public event Action<Vector2> onLook;

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onRightClick?.Invoke();
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onRoll?.Invoke();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLook?.Invoke(context.ReadValue<Vector2>());
        }
        else if (context.canceled)
        {
            onLook?.Invoke(Vector2.zero);
        }
    }

    public void OnTabMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onTab?.Invoke();
        }
    }
}
