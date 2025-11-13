using UnityEngine;

namespace MyRule.UI
{
    public class UIInputHandler : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private InputReader inputReader;

        [Header("Settings")]
        [SerializeField] private float cooldown = 0.2f;

        private float lastMove;

        private void OnEnable()
        {
            inputReader.SwitchActionMap(ActionMap.UI);

            inputReader.uiActions.onPressAnyButton += OnAnyButtonPress;
            inputReader.uiActions.onSubmit += OnSubmitPress;
            inputReader.uiActions.onCancel += OnCancelPress;
            inputReader.uiActions.onMove += OnMovePress;
            inputReader.uiActions.onAdjust += OnAdjustPress;
        }

        private void OnDisable()
        {
            inputReader.uiActions.onPressAnyButton -= OnAnyButtonPress;
            inputReader.uiActions.onSubmit -= OnSubmitPress;
            inputReader.uiActions.onCancel -= OnCancelPress;
            inputReader.uiActions.onMove -= OnMovePress;
            inputReader.uiActions.onAdjust -= OnAdjustPress;
        }

        private void OnAnyButtonPress()
        {
            EventBus<AnyButtonPressEvent>.Raise(new AnyButtonPressEvent());
        }

        private void OnSubmitPress()
        {
            EventBus<SubmitPressEvent>.Raise(new SubmitPressEvent());
        }

        private void OnCancelPress()
        {
            EventBus<CancelPressEvent>.Raise(new CancelPressEvent());
        }

        private void OnMovePress(Vector2 dir)
        { 
            EventBus<MovePressEvent>.Raise(new MovePressEvent(dir.x, dir.y));
        }

        private void OnAdjustPress(Vector2 value)
        {
            EventBus<AdjustPressEvent>.Raise(new AdjustPressEvent(value.x));
        }
    }
}