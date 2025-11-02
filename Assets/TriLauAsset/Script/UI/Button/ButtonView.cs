using TMPro;
using UnityEngine;
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

    public class ButtonView : MonoBehaviour, IButtonView
    {
        [SerializeField] private ButtonType buttonViewType = ButtonType.AnyButton;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private GameObject highlightButton;
        [SerializeField] private Color selectColor = Color.white;
        [SerializeField] private Color deselectColor = Color.gray;

        private ButtonPresenter presenter;

        public ButtonType Type => buttonViewType;

        private void OnEnable()
        {
            presenter = new ButtonPresenter(this);
        }

        private void OnDisable()
        {
            presenter.CleanUp();
        }

        public void Select()
        {
            highlightButton.SetActive(true);

            buttonText.color = selectColor;
        }

        public void Deselect()
        {
            highlightButton.SetActive(false);

            buttonText.color = deselectColor;
        }

        public void Submit()
        {
            button.onClick.Invoke();
        }
    }
}