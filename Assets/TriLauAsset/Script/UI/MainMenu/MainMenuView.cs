using MyRule.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class MainMenuView : BaseUIView
    {
        [SerializeField] private MainMenuButtonView anyBtn;
        [SerializeField] private MainMenuButtonView continueBtn;
        [SerializeField] private MainMenuButtonView newGameBtn;
        [SerializeField] private MainMenuButtonView loadGameBtn;
        [SerializeField] private MainMenuButtonView settingsBtn;
        [SerializeField] private MainMenuButtonView quitBtn;
 
        private MainMenuPresenter presenter;

        protected override void OnEnable()
        {
            base.OnEnable();

            presenter = new MainMenuPresenter();
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

            inputReader.SwitchActionMap(ActionMap.UI);

            //AudioManager.Instance.PlayMusic(MusicType.MainMenu);
        }

        public override void Show()
        {

        }

        public override void Hide()
        {

        }

        public void OnAnyPressed()
        {
            if (GameSystemManager.Instance.HasSaveData)
            {
                continueBtn.gameObject.SetActive(true);
                
                continueBtn.CurrentButton.Select();
            }
            else
            {
                continueBtn.gameObject.SetActive(false);

                newGameBtn.CurrentButton.Select();
            }
        }
    }
}