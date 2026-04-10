using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class WorldSpaceButtonView : MonoBehaviour
    {
        private enum ButtonState
        {
            Hidden,
            Far,
            Near
        }

        [SerializeField] private Image farBtnImg;
        [SerializeField] private CanvasGroup nearBtnImg;
        [SerializeField] private float switchFormDistance = 10f;
        [SerializeField] private float visibleDistance = 20f;
        [SerializeField] private Transform target;

        private ButtonState currentState = ButtonState.Hidden;

        private void Update()
        {
            if (target == null) return;

            float distance = Vector3.Distance(transform.position, target.position);

            ButtonState newState;

            if (distance > visibleDistance)
                newState = ButtonState.Hidden;
            else if (distance > switchFormDistance)
                newState = ButtonState.Far;
            else
                newState = ButtonState.Near;

            SetState(newState);
        }

        private void SetState(ButtonState newState)
        {
            if (currentState == newState) return;

            currentState = newState;

            farBtnImg.DOKill();
            nearBtnImg.DOKill();

            switch (currentState)
            {
                case ButtonState.Hidden:
                    farBtnImg.DOFade(0f, 0.3f);
                    nearBtnImg.DOFade(0f, 0.3f);
                    break;

                case ButtonState.Far:
                    farBtnImg.DOFade(1f, 0.3f);
                    nearBtnImg.DOFade(0f, 0.3f);
                    break;

                case ButtonState.Near:
                    farBtnImg.DOFade(0f, 0.3f);
                    nearBtnImg.DOFade(1f, 0.3f);
                    break;
            }
        }
    }
}