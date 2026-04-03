using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class LobbyShopFeaturedView : MonoBehaviour
    {
        [SerializeField] private Image[] images;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float delay = 2f;

        private int currentIndex = 0;
        private CancellationTokenSource cts;

        private void Start()
        {
            Init();
            StartAutoSlide().Forget();
        }

        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }

        private void Init()
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = new Color(1, 1, 1, i == 0 ? 1 : 0);
            }
        }

        private async UniTaskVoid StartAutoSlide()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            try
            {
                while (true)
                {
                    await UniTask.Delay((int)(delay * 1000), cancellationToken: cts.Token);

                    int nextIndex = (currentIndex + 1) % images.Length;
                    await ChangeImage(nextIndex, cts.Token);
                }
            }
            catch (System.OperationCanceledException)
            {
                
            }
        }

        private async UniTask ChangeImage(int newIndex, CancellationToken token)
        {
            if (newIndex == currentIndex) return;

            images[currentIndex].DOKill();
            images[newIndex].DOKill();

            Sequence seq = DOTween.Sequence();
            seq.Join(images[currentIndex].DOFade(0, duration));
            seq.Join(images[newIndex].DOFade(1, duration));

            currentIndex = newIndex;

            await seq.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);
        }

        public void OnClickButton(int index)
        {
            StartAutoSlide().Forget();

            ChangeImage(index, cts.Token).Forget();
        }
    }
}