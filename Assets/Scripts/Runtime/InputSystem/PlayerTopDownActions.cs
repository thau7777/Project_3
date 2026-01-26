using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerTopDownActions : InputActions.IPlayerTopDownActions
{
    public Action<Vector2> onMove;
    public Action onLeftClick;
    public Action<bool> onRightClick;
    public Action<bool> onSpaceBar;
    public Action<bool> onButtonQ;

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Check if pointer is over UI element
            if (!IsPointerOverUI())
            {
                onLeftClick?.Invoke();
            }
        }
    }

    private bool IsPointerOverUI()
    {
        // Check if EventSystem exists
        if (EventSystem.current == null)
            return false;

        // Check if pointer is over a UI element
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        onMove?.Invoke(value);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            onRightClick?.Invoke(true);
        else if (context.canceled)
            onRightClick?.Invoke(false);
    }

    public void OnSpaceBar(InputAction.CallbackContext context)
    {
        if (context.performed)
            onSpaceBar?.Invoke(true);
        else if (context.canceled)
            onSpaceBar?.Invoke(false);
    }

    public void OnQ(InputAction.CallbackContext context)
    {
        if (context.performed)
            onButtonQ?.Invoke(true);
        else if (context.canceled)
            onButtonQ?.Invoke(false);
    }

    public void OnE(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnR(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnT(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}