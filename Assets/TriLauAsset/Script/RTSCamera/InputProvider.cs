using MyRule.CommandPattern;
using MyRule.UI;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class InputProvider : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        Vector2 moveInput;
        Vector2 lookInput;
        Vector2 scrollInput;
        bool middleClickInput;

        public Vector2 MoveInput => moveInput;
        public Vector2 LookInput => lookInput;
        public Vector2 ScrollInput => scrollInput;
        public bool MiddleClickInput => middleClickInput;

        private void OnEnable()
        {
            inputReader.diceRollActions.onMove += OnMove;
            inputReader.diceRollActions.onLook += OnLook;
            inputReader.diceRollActions.onRightClick += OnMiddleClick;
            inputReader.diceRollActions.onScroll += OnScrollWheel;
            inputReader.diceRollActions.onTab += OnTab;
            inputReader.diceRollActions.onEsc += OnEsc;
        }

        private void OnDisable()
        {
            inputReader.diceRollActions.onMove -= OnMove;
            inputReader.diceRollActions.onLook -= OnLook;
            inputReader.diceRollActions.onRightClick -= OnMiddleClick;
            inputReader.diceRollActions.onScroll -= OnScrollWheel;
            inputReader.diceRollActions.onTab -= OnTab;
            inputReader.diceRollActions.onEsc -= OnEsc;
        }

        private void Start()
        {
            inputReader.SwitchActionMap(ActionMap.DiceRoll);
        }

        void OnMove(Vector2 value)
        {
            moveInput = value;
        }

        void OnLook(Vector2 value)
        {
            lookInput = value;
        }

        void OnScrollWheel(Vector2 value)
        {
            scrollInput = value;
        }

        void OnMiddleClick(bool value)
        {
            middleClickInput = value;
        }

        void OnTab()
        {
            EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Stats));
        }

        void OnEsc()
        {
            CommandInvoker.UndoCommand();
        }
    }
}