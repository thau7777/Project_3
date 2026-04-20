using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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

        private CancellationTokenSource cts;

        protected override void OnEnable()
        {
            base.OnEnable();

            presenter = new MainMenuPresenter();
            inputReader.uiActions.onPressAnyButton += OnAnyPressed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            presenter.CleanUp();
            presenter = null;

            inputReader.uiActions.onPressAnyButton -= OnAnyPressed;
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
            if (!anyBtn.gameObject.activeSelf) return;
            anyBtn.GetComponent<Button>().onClick.Invoke();
            Debug.Log("Any button pressed");
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