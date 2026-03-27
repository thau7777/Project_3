using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class CardRotator : MonoBehaviour
    {
        [SerializeField] private Card card;

        private const float maxRotationX = 30f;
        private const float maxRotationY = 30f;
        private const float rotationSpeed = 0.2f;
        private const float snapBackDuration = 0.15f;

        private bool canRotate = false;
        private bool isRotating = false;

        private Vector3 rotationStartPos;
        private Camera mainCam;

        private Quaternion originalLocalRotation;

        private CancellationTokenSource snapCTS;

        private void Awake()
        {
            mainCam = Camera.main;
            SetOriginalLocalRotation(transform.localRotation);
        }

        private void Update()
        {
            if (!canRotate || card.CardType != CardGameplayType.Detail) return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            Vector2 mouseScreenPos = mouse.position.ReadValue();

            if (!isRotating && mouse.rightButton.wasPressedThisFrame)
            {
                isRotating = true;
                CancelSnapBack();

                rotationStartPos = GetWorldPoint(mouseScreenPos);
            }

            if (isRotating && mouse.rightButton.wasReleasedThisFrame)
            {
                isRotating = false;

                SnapBackAsync().Forget();
            }

            if (isRotating)
            {
                Vector3 currentPos = GetWorldPoint(mouseScreenPos);
                Vector3 offset = currentPos - rotationStartPos;

                float xRot = Mathf.Clamp(offset.x * rotationSpeed, -maxRotationX, maxRotationX);
                float yRot = Mathf.Clamp(offset.y * rotationSpeed, -maxRotationY, maxRotationY);

                Quaternion targetLocalRot = originalLocalRotation * Quaternion.Euler(yRot, -xRot, 0);
                transform.localRotation = targetLocalRot;
            }
        }

        private Vector3 GetWorldPoint(Vector2 screenPos)
        {
            float dist = Vector3.Distance(mainCam.transform.position, transform.position);
            return mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, dist));
        }

        public void SetOriginalLocalRotation(Quaternion rotation)
        {
            originalLocalRotation = Quaternion.Euler(Vector3.zero);
        }

        public void UnlockRotate()
        {
            canRotate = true;
        }

        public void LockRotate()
        {
            canRotate = false;
        }

        private async UniTaskVoid SnapBackAsync()
        {
            CancelSnapBack();
            snapCTS = new CancellationTokenSource();
            var token = snapCTS.Token;

            Quaternion start = transform.localRotation;
            Quaternion end = originalLocalRotation;
            float elapsed = 0f;

            try
            {
                while (elapsed < snapBackDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / snapBackDuration);
                    transform.localRotation = Quaternion.Slerp(start, end, t);

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                transform.localRotation = end;
            }
            catch (OperationCanceledException)
            {
                // 
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[CardRotator] SnapBackAsync error: {e}");
            }
        }

        private void CancelSnapBack()
        {
            if (snapCTS != null)
            {
                snapCTS.Cancel();
                snapCTS.Dispose();
                snapCTS = null;
            }
        }

        private void OnMouseEnter()
        {
            UnlockRotate();
        }

        private void OnMouseExit()
        {
            if (!isRotating)
                LockRotate();
        }

        private void OnDestroy()
        {
            CancelSnapBack();
        }
    }
}