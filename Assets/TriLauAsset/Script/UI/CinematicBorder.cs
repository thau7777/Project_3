using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class CinematicBorder : Singleton<CinematicBorder>
    {
        [SerializeField] private Image topBorder;
        [SerializeField] private Image bottomBorder;

        public async UniTask ShowBorder(float duration)
        {
            topBorder.DOFade(1f, duration);
            topBorder.transform.DOLocalMoveY(941.5f, duration).SetEase(Ease.Linear);
            bottomBorder.DOFade(1f, duration);
            bottomBorder.transform.DOLocalMoveY(-941.5f, duration).SetEase(Ease.Linear);

            await UniTask.Delay((int)(duration * 1000));
        }

        public async UniTask HideBorder(float duration)
        {
            topBorder.DOFade(0f, duration);
            topBorder.transform.DOLocalMoveY(1200f, duration).SetEase(Ease.Linear);
            bottomBorder.DOFade(0f, duration);
            bottomBorder.transform.DOLocalMoveY(-1200f, duration).SetEase(Ease.Linear);

            await UniTask.Delay((int)(duration * 1000));
        }
    }
}