using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyRule.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class AchievementNoctificationView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _achievementName;
        [SerializeField] private float _moveDuration = 0.5f;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float targetY = 920;
        [SerializeField] private float originalY = 1400;

        private EventBinding<NoctificationAchievementEvent> noctificationAchievementEvt;

        private void OnEnable()
        {
            noctificationAchievementEvt = new EventBinding<NoctificationAchievementEvent>(OnNoctificationAchievement);
            EventBus<NoctificationAchievementEvent>.Register(noctificationAchievementEvt);
        }

        private void OnDisable()
        {
            EventBus<NoctificationAchievementEvent>.Deregister(noctificationAchievementEvt);
        }

        private async void OnNoctificationAchievement(NoctificationAchievementEvent evt)
        {
            AchievementConfig achievementConfig = AchievementManager.Instance.GetAchievementById(evt.achievement.ID);

            _icon.sprite = achievementConfig.icon;
            _achievementName.text = achievementConfig.achievementName;

            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DOLocalMoveY(targetY, _moveDuration).SetEase(Ease.OutBack));
            seq.Join(canvasGroup.DOFade(1, _moveDuration));

            seq.AppendInterval(2f);

            seq.Append(transform.DOLocalMoveY(originalY, _moveDuration).SetEase(Ease.InBack));
            seq.Join(canvasGroup.DOFade(0, _moveDuration));

            await seq.AsyncWaitForCompletion();
        }
    }
}