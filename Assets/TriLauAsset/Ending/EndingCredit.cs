using DG.Tweening;
using UnityEngine;

namespace MyRule
{
    public class EndingCredit : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public void ShowText()
        {
            canvasGroup.DOFade(1f, 0.4f);
        }
    }
}