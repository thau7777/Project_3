using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class CombatButtonChoiceView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image combatImg;
        [SerializeField] private float moveDuration = 0.4f;
        [SerializeField] private float originalX;
        [SerializeField] private float inX;

        private Tween moveTween;
        private Tween fadeTween;
        private Tween scaleTween;

        private void Start()
        {
            combatImg.material.SetFloat("_CycleTime", 0);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            KillAllTween();

            moveTween = transform.DOLocalMoveX(inX, moveDuration);
            fadeTween = combatImg.DOFade(1f, moveDuration);
            scaleTween = combatImg.transform.DOScale(1.2f, moveDuration);
            combatImg.material.SetFloat("_CycleTime", 2);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            KillAllTween();

            moveTween = transform.DOLocalMoveX(originalX, moveDuration);
            fadeTween = combatImg.DOFade(0.25f, moveDuration);
            scaleTween = combatImg.transform.DOScale(1f, moveDuration);
            combatImg.material.SetFloat("_CycleTime", 0);
        }

        private void KillAllTween()
        {
            moveTween?.Kill();
            fadeTween?.Kill();
            scaleTween?.Kill();
        }
    }
}