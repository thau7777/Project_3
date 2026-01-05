using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class MainMenuView : BaseUIView, IMainMenuPanel
    {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject menuButtonsPanel;

        private PanelType panelType;

        public PanelType Type
        {
            get => panelType;
            set => panelType = value;
        }

        private MainMenuPresenter presenter;

        protected override void OnEnable()
        {
            base.OnEnable();

            presenter = new MainMenuPresenter(this, this);

            panelType = PanelType.MainMenu;
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

            panelType = PanelType.MainMenu;
        }

        public override void Show() => mainMenuPanel.SetActive(true);

        public override void Hide() => mainMenuPanel.SetActive(false);
    }
}