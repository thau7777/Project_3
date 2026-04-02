using Ami.BroAudio;
using MyRule.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule.UI
{
    public enum ButtonType
    {
        AnyButton,
        ContinueButton,
        NewGameButton,
        LoadGameButton,
        SystemButton,
        CreditsButton,
        QuitButton,
        PlayButton,
        PauseButton,
        StopButton,
        RestartButton,
        ProfileButton,
        DiaryButton,
        ShopButton,
    }

    public class ButtonView : MonoBehaviour, IButtonView, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] protected ButtonType buttonViewType = ButtonType.AnyButton;
        [SerializeField] protected Button button;

        protected ButtonPresenter presenter;

        private void OnEnable()
        {
            presenter = new ButtonPresenter(this);
        }

        private void OnDisable()
        {
            presenter.CleanUp();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            AudioManager.Instance.PlaySound("UIButtonClick");
            button.onClick?.Invoke();

            Navigator.OnSubmitPress(button, buttonViewType);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            button.Select();
            AudioManager.Instance.PlaySound("UIButtonClick");
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
        }
    }
}