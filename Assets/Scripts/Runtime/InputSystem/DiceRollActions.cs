using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceRollActions : InputActions.IDiceRollActions
{
    public event Action onTab;

    public event Action onRoll;

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onRoll?.Invoke();
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
