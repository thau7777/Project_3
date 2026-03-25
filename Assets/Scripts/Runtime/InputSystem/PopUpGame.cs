using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpGame : InputActions.IPopUpGameActions
{
    public Action<Vector2> onNaviagate;
    public Action onLeftClick;
    public Action onThrow;
    public Action onReset;

    public void OnNaviagate(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        onNaviagate?.Invoke(value);
    }
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            onLeftClick?.Invoke();
    }
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
            onThrow?.Invoke();
            
    }
    public void OnReset(InputAction.CallbackContext context)
    {
        if(context.performed)
            onReset?.Invoke();
    }
     
}
