using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceStationActions : InputActions.ISpaceStationActions
{
    public Action onEsc;
    public Action onInteract;
    public Action<Vector2> onMove;

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

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            onMove?.Invoke(context.ReadValue<Vector2>());
        else if (context.canceled)
            onMove?.Invoke(Vector2.zero);
    }
}
