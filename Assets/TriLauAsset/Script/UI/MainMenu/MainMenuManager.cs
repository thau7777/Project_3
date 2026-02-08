using MyRule.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button newGameBtn;
        [SerializeField] private Button settingBtn;

        private EventBinding<MainMenuButtonSelectedEvent> mainMenuButtonSelectedEventBinding;

        private void OnEnable()
        {
            mainMenuButtonSelectedEventBinding = new EventBinding<MainMenuButtonSelectedEvent>(HandleSelectBtn);
            EventBus<MainMenuButtonSelectedEvent>.Register(mainMenuButtonSelectedEventBinding);
        }

        private void OnDisable()
        {
            EventBus<MainMenuButtonSelectedEvent>.Deregister(mainMenuButtonSelectedEventBinding);
        }

        private void Start()
        {
            AudioManager.Instance.PlayMusic(MusicType.MainMenu);
            
        }

        private void OnDestroy()
        {
            
        }

        private void HandleSelectBtn(MainMenuButtonSelectedEvent evt)
        {
            switch (evt.ButtonType)
            {
                case UI.ButtonType.NewGameButton:
                    newGameBtn.Select();
                    break;
                case UI.ButtonType.SystemButton:
                    settingBtn.Select();
                    break;
                default:
                    break;
            }
        }
    }
}