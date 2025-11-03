using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class MainMenuView : BaseUIView, IMainMenuPanel
    {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject menuButtonsPanel;
        [SerializeField] private ButtonView anyButton;

        private MainMenuPresenter presenter;

        public ButtonView CurrentButton { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            presenter = new MainMenuPresenter(this, this);

            panelType = PanelType.MainMenu;

            CurrentButton = anyButton;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            presenter.CleanUp();
            presenter = null;
        }

        protected override void Start()
        {
            base.Start();

            anyButton.Select();

            UIStateMachine.Reset(PanelType.MainMenu);
        }

        public void ShowMenuButtons() => menuButtonsPanel.SetActive(true);

        public void HideAnyButton() => anyButton.gameObject.SetActive(false);

        public override void Show() => mainMenuPanel.SetActive(true);

        public override void Hide() => mainMenuPanel.SetActive(false);
    }
}