using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceRollActions : InputActions.IDiceRollActions
{
    public event Action onTab;

    public void OnTabMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onTab?.Invoke();
        }
    }
}
