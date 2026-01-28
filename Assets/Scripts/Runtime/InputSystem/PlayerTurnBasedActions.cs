using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurnBasedActions : InputActions.IPlayerTurnBasedActions
{
    public event Action OnTestEvent;

    public void OnTest(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnTestEvent?.Invoke();
        }
    }

}
