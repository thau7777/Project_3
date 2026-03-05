using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerTopDownActions : InputActions.IPlayerTopDownActions
{
    public Action<Vector2> onMove;
    public Action onLeftClick;
    public Action<bool> onRightClick;
    public Action<bool,int> onSkillUse;
    public Action<int> onItemUse;


    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLeftClick?.Invoke();
            // Check if pointer is over UI element
            //if (!IsPointerOverUI())
            //{
            //    onLeftClick?.Invoke();
            //}
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

    public void OnSkill_1(InputAction.CallbackContext context)
    {
        if(context.performed)
            onSkillUse?.Invoke(true, 0);
        else if(context.canceled)
            onSkillUse?.Invoke(false, 0);
    }

    public void OnSkill_2(InputAction.CallbackContext context)
    {
        if (context.performed)
            onSkillUse?.Invoke(true, 1);
        else if (context.canceled)
            onSkillUse?.Invoke(false, 1);
    }

    public void OnSkill_3(InputAction.CallbackContext context)
    {
        if (context.performed)
            onSkillUse?.Invoke(true, 2);
        else if (context.canceled)
            onSkillUse?.Invoke(false, 2);
    }

    public void OnSkill_4(InputAction.CallbackContext context)
    {
        if (context.performed)
            onSkillUse?.Invoke(true, 3);
        else if (context.canceled)
            onSkillUse?.Invoke(false, 3);
    }

    public void OnSkill_5(InputAction.CallbackContext context)
    {
        if (context.performed)
            onSkillUse?.Invoke(true, 4);
        else if (context.canceled)
            onSkillUse?.Invoke(false, 4);
    }

    public void OnSkill_6(InputAction.CallbackContext context)
    {
        if (context.performed)
            onSkillUse?.Invoke(true, 5);
        else if (context.canceled)
            onSkillUse?.Invoke(false, 5);
    }

    public void OnItem_1(InputAction.CallbackContext context)
    {
        if (context.performed)
            onItemUse?.Invoke(0);
    }

    public void OnItem_2(InputAction.CallbackContext context)
    {
        if (context.performed)
            onItemUse?.Invoke(1);
    }

    public void OnItem_3(InputAction.CallbackContext context)
    {
        if (context.performed)
            onItemUse?.Invoke(2);
    }

    public void OnItem_4(InputAction.CallbackContext context)
    {
        if (context.performed)
            onItemUse?.Invoke(3);
    }

    public void OnItem_5(InputAction.CallbackContext context)
    {
        if (context.performed)
            onItemUse?.Invoke(4);
    }

    public void OnItem_6(InputAction.CallbackContext context)
    {
        if (context.performed)
            onItemUse?.Invoke(5);
    }
}