using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class InputProvider : MonoBehaviour, IInputProvider
    {
        [SerializeField] private InputReader _inputReader;

        private Vector2 _movementInput;
        private Vector2 _mousePosition;
        private float _zoomInput;
        private Vector2 _mouseInput;

        public bool HasMouse => throw new System.NotImplementedException();

        public Vector2 LookInput => throw new System.NotImplementedException();

        public Vector2 MouseInput => throw new System.NotImplementedException();

        public float ZoomInput => throw new System.NotImplementedException();

        public Vector2 MousePosition => throw new System.NotImplementedException();

        public Vector2 MovementInput() => _movementInput; 

        public bool RotationButtonInput()
        {
            throw new System.NotImplementedException();
        }

        public bool CanAlwaysRotate { get; private set; }

        private void OnEnable()
        {
            _inputReader.diceRollActions.onLook += GetMovementInput;
        }

        private void OnDisable()
        {
            _inputReader.diceRollActions.onLook -= GetMovementInput;
        }

        private void GetMovementInput(Vector2 look)
        {
            _movementInput = look;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;

            if (mouse != null && keyboard != null)
            {
                UpdateInput(mouse, keyboard);
            }
        }

        void UpdateInput(Mouse mouse, Keyboard keyboard)
        {
            _mousePosition = mouse.position.value;
            _zoomInput = mouse.scroll.value.y;
        }
    }
}