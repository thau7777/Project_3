using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurnBasedActions : InputActions.IPlayerTurnBasedActions
{
    public event Action LeftEvent;
    public event Action RightEvent;
    public event Action QEvent;
    public event Action REvent;
    public event Action EEvent;
    public event Action SpaceEvent;
    public event Action SummonEvent;


    public void OnA(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LeftEvent?.Invoke();
        }
    }

    public void OnD(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RightEvent?.Invoke();
        }
    }

    public void OnQ(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            QEvent?.Invoke();
        }
    }

    public void OnE(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EEvent?.Invoke();
        }
    }

    public void OnR(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            REvent?.Invoke();
        }
    }
    public void OnSpace(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SpaceEvent?.Invoke();
        }
    }

    public void OnF(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SummonEvent?.Invoke();
        }
    }


}
