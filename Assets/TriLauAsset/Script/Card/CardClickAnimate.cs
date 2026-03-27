using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace MyRule
{
    public class CardClickAnimate : MonoBehaviour
    {
        [SerializeField] private Transform card;
        
        [Header("UpsideDown")]
        [SerializeField] private float flipDuration = 0.4f;

        [Header("Circular")]
        [SerializeField] private float forwardZ = 2f;
        [SerializeField] private float upTime = 0.25f;
        [SerializeField] private float downTime = 0.2f;

        [Header("Shake")]
        [SerializeField] private float shakeAngle = 10f;
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private int shakeVibrato = 20;

        private bool canInteract = false;
        private bool isFaceUp = false;

        private Vector3 originalPos;

        private Tween currentTween;

        private void Awake()
        {
            originalPos = card.localPosition;
            isFaceUp = false;
        }

        public void SetInteract(bool interact) => this.canInteract = interact;

        public async UniTask FlipUp()
        {
            if (isFaceUp) return;

            currentTween?.Kill();

            currentTween = card
                .DOLocalRotateQuaternion(Quaternion.Euler(Vector3.zero), flipDuration)
                .SetEase(Ease.InOutQuad);

            await currentTween.AsyncWaitForCompletion();

            isFaceUp = true;
        }

        public async UniTask FlipDown()
        {
            if (!isFaceUp) return;

            currentTween?.Kill();

            currentTween = card
                .DOLocalRotateQuaternion(Quaternion.Euler(0, 180, 0), flipDuration)
                .SetEase(Ease.InOutQuad);

            await currentTween.AsyncWaitForCompletion();

            isFaceUp = false;
        }

        public async UniTask PlayCircular()
        {
            Debug.Log("PlayCircular");
            currentTween?.Kill();

            Sequence seq = DOTween.Sequence();

            seq.Append(
                card.DOLocalMoveZ(originalPos.z + forwardZ, upTime)
                    .SetEase(Ease.OutCubic)
            );

            seq.Join(
                card.DOLocalRotate(
                    new Vector3(0, 360f, 0),
                    upTime + downTime,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.InOutCubic)
            );

            seq.Append(
                card.DOLocalMoveZ(originalPos.z, downTime)
                    .SetEase(Ease.InQuad)
            );

            seq.Join(
                card.DOLocalRotateQuaternion(Quaternion.Euler(Vector3.zero), 0.1f)
            );

            seq.Append(
                card.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1f)
            );

            currentTween = seq;

            await seq.AsyncWaitForCompletion();
        }

        public async UniTask PlayShake()
        {
            if (!canInteract) return;

            currentTween?.Kill();

            Quaternion baseRot = card.localRotation;

            currentTween = card
                .DOPunchRotation(
                    new Vector3(0, 0, shakeAngle),
                    shakeDuration,
                    shakeVibrato,
                    1f
                )
                .SetEase(Ease.OutQuad);

            await currentTween.AsyncWaitForCompletion();

            card.localRotation = baseRot;
        }
    }
}