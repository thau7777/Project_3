using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Turnbase
{
    public class EvadeMiniGame : MonoBehaviour
    {
        [Header("UI Components")]
        public Slider slider;
        public Image fillImage;

        private bool isGameActive = false;
        private System.Action<bool> onComplete;

        public void StartGame(float duration, System.Action<bool> callback)
        {
            this.onComplete = callback;
            this.isGameActive = true;

            gameObject.SetActive(true);
            slider.value = 0;

            StopAllCoroutines();
            StartCoroutine(RunSlider(duration));
        }

        private void Update()
        {
            if (!isGameActive) return;

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                EndGame(true);
            }
        }

        private IEnumerator RunSlider(float duration)
        {
            float elapsed = 0;
            while (elapsed < duration && isGameActive)
            {
                elapsed += Time.unscaledDeltaTime;
                slider.value = elapsed / duration;
                yield return null;
            }

            if (isGameActive)
            {
                EndGame(false);
            }
        }

        private void EndGame(bool success)
        {
            isGameActive = false;
            onComplete?.Invoke(success);
            gameObject.SetActive(false);
        }
    }
}