using UnityEngine;

namespace MyRule
{
    public class PressAnyButton : MonoBehaviour
    {
        public MainMenuButton targetButton;
        public InputReader inputReader;

        private void OnEnable()
        {
            inputReader.uiActions.onPressAnyButton += OnButtonSubmit;
        }

        private void OnDisable()
        {
            inputReader.uiActions.onPressAnyButton -= OnButtonSubmit;
        }

        private void Start()
        {
            targetButton.SelectObject();
        }

        private void OnButtonSubmit()
        {
            gameObject.SetActive(false);
            targetButton.Submit();
        }
    }
}