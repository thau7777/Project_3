using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class CameraShake : MonoBehaviour
    {
        private Vector3 originalPos;

        private void OnEnable()
        {
            EventBusUI<CameraShakeEvent>.Subscribe(OnCameraShakeRequested);
        }

        private void OnDisable()
        {
            EventBusUI<CameraShakeEvent>.Unsubscribe(OnCameraShakeRequested);
        }

        private void OnCameraShakeRequested(CameraShakeEvent evt)
        {
            Shake(evt.duration, evt.magnitude);
        }

        public void Shake(float duration, float magnitude)
        {
            StopAllCoroutines();
            StartCoroutine(PerformShake(duration, magnitude));
        }

        private IEnumerator PerformShake(float duration, float magnitude)
        {
            originalPos = transform.localPosition;

            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }
}