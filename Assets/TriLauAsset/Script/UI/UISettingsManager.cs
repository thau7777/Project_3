using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule
{
    public class UISettingsManager : MonoBehaviour
    {
        public static UISettingsManager Instance;

        private VisualElement _settingsPanel;

        private void Awake()
        {
            Instance = this;

            var root = GetComponent<UIDocument>().rootVisualElement;

            _settingsPanel = root.Q<VisualElement>("SettingsPanel");
        }

        private void Start()
        {
            
        }

        public void HideSettingsPanel()
        {
            _settingsPanel.AddToClassList("HideSettingsPanel");
        }

        public void ShowSettingsPanel()
        {
            _settingsPanel.RemoveFromClassList("HideSettingsPanel");
        }
    }
}