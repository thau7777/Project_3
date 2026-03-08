using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceRollActions : InputActions.IDiceRollActions
{
    public event Action onTab;

    public event Action onRoll;

    public event Action<bool> onRightClick;

    public event Action<Vector2> onLook;
    public event Action<Vector2> onMove;
    public event Action<Vector2> onScroll;

    public event Action onEsc;

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.performed) onEsc?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLook?.Invoke(context.ReadValue<Vector2>());
        }
        else
        {
            onLook?.Invoke(Vector2.zero);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onMove?.Invoke(context.ReadValue<Vector2>());
        }
        else
        {
            onMove?.Invoke(Vector2.zero);
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onRightClick?.Invoke(true);
        }
        else
        {
            onRightClick?.Invoke(false);
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onRoll?.Invoke();
        }
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onScroll?.Invoke(context.ReadValue<Vector2>());
        }
        else
        {
            onScroll?.Invoke(Vector2.zero);
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
