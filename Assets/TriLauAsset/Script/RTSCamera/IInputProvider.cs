using UnityEngine;

namespace MyRule
{
    public interface IInputProvider
    {
        public bool HasMouse { get; }

        public Vector2 MovementInput();
        public Vector2 LookInput { get; }
        public Vector2 MouseInput { get; }
        public float ZoomInput { get; }
        public bool RotationButtonInput();
        public bool CanAlwaysRotate { get; }
        public Vector2 MousePosition { get; }
    }
}