using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceStationActions : InputActions.ISpaceStationActions
{
    public Action onEsc;
    public Action onInteract;
    public Action<Vector2> onMove;
    public Action onActve;
    public Action<Vector2> onLook;
    public Action onTab;
    public Action onSpace;

    public void OnActve(InputAction.CallbackContext context)
    {
        if (context.performed)
            onActve?.Invoke();
    }

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.performed)
            onEsc?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            onInteract?.Invoke();
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

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            onMove?.Invoke(context.ReadValue<Vector2>());
        else if (context.canceled)
            onMove?.Invoke(Vector2.zero);
    }

    public void OnSkipTutorial(InputAction.CallbackContext context)
    {
        if (context.performed) onSpace?.Invoke();
    }

    public void OnTab(InputAction.CallbackContext context)
    {
        if (context.performed)
            onTab?.Invoke();
    }
}
