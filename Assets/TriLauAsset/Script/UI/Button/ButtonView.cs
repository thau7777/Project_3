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
        SettingsButton,
        CreditsButton,
        QuitButton,
        PlayButton,
        PauseButton,
        StopButton,
        RestartButton
    }

    public class ButtonView : MonoBehaviour, IButtonView, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] protected ButtonType buttonViewType = ButtonType.AnyButton;
        [SerializeField] protected Button button;

        protected ButtonPresenter presenter;

        public ButtonType Type => buttonViewType;

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
            AudioManager.Instance.PlaySFX(SFXType.UI_Click);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            AudioManager.Instance.PlaySFX(SFXType.UI_Select);
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
        }
    }
}