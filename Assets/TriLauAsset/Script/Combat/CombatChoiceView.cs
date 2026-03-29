using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyRule.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class CombatChoiceView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.4f;

        [Header("Top down")]
        [SerializeField] private Transform waveViewTDParent;
        [SerializeField] private Button tdBtn;
        [SerializeField] private GameObject tdWaveViewPreb;

        [Header("Turn base")]
        [SerializeField] private Transform waveViewTBParent;
        [SerializeField] private Button tbBtn;
        [SerializeField] private GameObject tbWaveViewPreb;

        private EventBinding<UpdateTDCombatWavesEvent> updateTDEventBinding;
        private EventBinding<UpdateTBCombatWavesEvent> updateTBEventBinding;
        private EventBinding<ShowCombatChoiceEvent> showCombatChoiceEventBinding;

        private GroupWave tdWaves;
        private GroupWave tbWaves;

        private void OnEnable()
        {
            updateTDEventBinding = new EventBinding<UpdateTDCombatWavesEvent>(UpdateTDWaveView);
            EventBus<UpdateTDCombatWavesEvent>.Register(updateTDEventBinding);

            updateTBEventBinding = new EventBinding<UpdateTBCombatWavesEvent>(UpdateTBWaveView);
            EventBus<UpdateTBCombatWavesEvent>.Register(updateTBEventBinding);

            showCombatChoiceEventBinding = new EventBinding<ShowCombatChoiceEvent>(ShowCombatHandle);
            EventBus<ShowCombatChoiceEvent>.Register(showCombatChoiceEventBinding);
        }

        private void OnDisable()
        {
            EventBus<UpdateTDCombatWavesEvent>.Deregister(updateTDEventBinding);
            EventBus<UpdateTBCombatWavesEvent>.Deregister(updateTBEventBinding);
            EventBus<ShowCombatChoiceEvent>.Deregister(showCombatChoiceEventBinding);
        }

        private void Start()
        {
            tdBtn.onClick.AddListener(LoadTDScene);
            tbBtn.onClick.AddListener(LoadTBScene);
        }

        private void UpdateTDWaveView(UpdateTDCombatWavesEvent evt)
        {
            tdWaves = evt.groupWave;

            for (int i = 0; i < evt.groupWave.WaveDatas.Length; i++)
            {
                WaveData waveData = evt.groupWave.WaveDatas[i];

                if (waveData != null)
                {
                    var waveViewObj = Instantiate(tdWaveViewPreb, waveViewTDParent);
                    CombatWaveView combatWaveView = waveViewObj.GetComponent<CombatWaveView>();
                    combatWaveView.SetUpWave(i + 1, waveData);
                }
            }
        }

        private void UpdateTBWaveView(UpdateTBCombatWavesEvent evt)
        {
            tbWaves = evt.groupWave;

            for (int i = 0; i < evt.groupWave.WaveDatas.Length; i++)
            {
                WaveData waveData = evt.groupWave.WaveDatas[i];

                if (waveData != null)
                {
                    var waveViewObj = Instantiate(tbWaveViewPreb, waveViewTBParent);
                    CombatWaveView combatWaveView = waveViewObj.GetComponent<CombatWaveView>();
                    combatWaveView.SetUpWave(i + 1, waveData);
                }
            }
        }

        private void ShowCombatHandle(ShowCombatChoiceEvent evt)
        {
            if (evt.showCombatChoice)
            {
                Show();
                EventBus<OpenHUDEvent>.Raise(new OpenHUDEvent(false));
            }
            else
            {
                Hide();
                EventBus<OpenHUDEvent>.Raise(new OpenHUDEvent(true));
            }
        }

        private void Show()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1f, fadeDuration);
            VolumeController.Instance.AdjustUIVolumeWeight();
        }

        private void Hide()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0f, fadeDuration);
            VolumeController.Instance.AdjustUIVolumeWeight();
        }

        private async void LoadTDScene()
        {
            EventBus<WaveEvent>.Raise(new WaveEvent(tdWaves));
            CombatManager.Instance.CombatData.SetScene(Loader.EScene.TopDown);
            await DelayToLoad();
            await Loader.LoadSceneDirect(Loader.EScene.TopDown);
        }
        private async void LoadTBScene()
        {
            EventBus<WaveEvent>.Raise(new WaveEvent(tbWaves));
            CombatManager.Instance.CombatData.SetScene(Loader.EScene.TurnBase);
            await DelayToLoad();
            await Loader.LoadSceneDirect(Loader.EScene.TurnBase);
        }

        private async UniTask DelayToLoad()
        {
            Hide();
            await UniTask.Delay(400);
            BlackFade.Instance.FadeIn();
            await UniTask.Delay(1000);
        }
    }
}