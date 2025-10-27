using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule
{
    public class UISettingsManager : MonoBehaviour
    {
        private VisualElement _settingsPanel;
        private Button _backButton;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _settingsPanel = root.Q<VisualElement>("SettingsPanel");
            _backButton = root.Q<Button>("BackButton");
        }

        private void Start()
        {
            _backButton.clicked += OnBackButtonClicked;
        }

        private void OnBackButtonClicked()
        {
            MainMenuSwithCam.Instance.SwithCamera();
            _settingsPanel.AddToClassList("HideSettingsPanel");
        }

        public void ShowSettingsPanel()
        {
            _settingsPanel.RemoveFromClassList("HideSettingsPanel");
        }
    }
}