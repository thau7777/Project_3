using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyRule.CommandPattern;
using MyRule.Event;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class PopUpView : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.4f;
        [SerializeField] private Button resumeBtn;
        [SerializeField] private Button settingsBtn;
        [SerializeField] private Button pauseBtn;
        [SerializeField] private Button surrender;
        [SerializeField] private Volume uiVolume;

        private bool isShowing = false;

        private CancellationTokenSource cts;

        private void Start()
        {
            pauseBtn.onClick.AddListener(Show);
            resumeBtn.onClick.AddListener(Hide);
            settingsBtn.onClick.AddListener(OpenSettings);
            surrender.onClick.AddListener(Surrender);
        }

        public async void Show()
        {
            if (isShowing) return;  

            EventBus<OpenHUDEvent>.Raise(new OpenHUDEvent(false));

            canvasGroup.DOFade(1f, fadeDuration);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            _inputReader.SwitchActionMap(ActionMap.UI);

            cts?.Cancel();
            cts = new CancellationTokenSource();

            Transition.TransitionValue(
                    setter: value => uiVolume.weight = value,
                    from: uiVolume.weight,
                    to: 1f,
                    duration: fadeDuration,
                    cts.Token).Forget();

            EventSystem.current.SetSelectedGameObject(resumeBtn.gameObject);

            await UniTask.Delay((int)(fadeDuration * 1000));

            Time.timeScale = 0f;

            isShowing = true;
        }

        public void Hide()
        {
            if (!isShowing) return;

            Time.timeScale = 1f;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0f, fadeDuration);

            Loader.EScene currentScene = MatchManager.Instance.MatchData.Scene;
            switch (currentScene)
            {
                case Loader.EScene.GreenlandScene:
                case Loader.EScene.DesertScene:
                case Loader.EScene.IcelandScene:
                    _inputReader.SwitchActionMap(ActionMap.DiceRoll);
                    break;  
            }

            cts?.Cancel();
            cts = new CancellationTokenSource();

            Transition.TransitionValue(
                    setter: value => uiVolume.weight = value,
                    from: uiVolume.weight,
                    to: 0f,
                    duration: fadeDuration,
                    cts.Token).Forget();

            EventBus<OpenHUDEvent>.Raise(new OpenHUDEvent(true));

            isShowing = false;
        }

        private void OpenSettings()
        {
            Loader.EScene eScene = MatchManager.Instance.MatchData.Scene;
            ICommand settingCommand = new SceneCommand(settingsBtn, eScene, Loader.EScene.SettingsScene);
            CommandInvoker.ExecuteCommand(settingCommand);
        }

        private async void Surrender()
        {
            Hide();
            
            await UniTask.Delay((int)(fadeDuration * 1000));

            EventBus<UpdateMatchResultEvent>.Raise(new UpdateMatchResultEvent(EMatchResult.Lose));
        }
    }
}