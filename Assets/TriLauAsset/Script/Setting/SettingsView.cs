using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public class SettingsView : BaseUIToolkitView, ISettingsView
    {
        private VisualElement settingsPanel;
        private Button generalButton;
        private Button graphicsButton;
        private Button soundButton;
        private Button consoleButton;
        private Button keyboardButton;

        private SettingsPresenter presenter;

        protected override void OnEnable()
        {
            base.OnEnable();
            panelType = PanelType.Settings;

            settingsPanel = root.Q<VisualElement>("SettingsPanel");
            generalButton = root.Q<Button>("GeneralButton");
            graphicsButton = root.Q<Button>("GraphicsButton");
            soundButton = root.Q<Button>("SoundButton");
            consoleButton = root.Q<Button>("ConsoleButton");
            keyboardButton = root.Q<Button>("KeyboardButton");

            presenter = new SettingsPresenter(this, this);

            Hide();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public void Select(Button button) => button.AddToClassList("SelectedButton");

        public void Deselect(Button button) => button.RemoveFromClassList("SelectedButton");

        public override void Show() => settingsPanel.AddToClassList("HideSettingsPanel");

        public override void Hide() => settingsPanel.AddToClassList("HideSettingsPanel");
    }
}