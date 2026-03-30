using UnityEngine;
using System.Collections;

namespace Turnbase
{
    public class ParryMiniGame : MonoBehaviour
    {
        private bool isGameActive = false;
        private bool isLockedOut = false;    
        private bool isParryWindowOpen = false; 
        private System.Action<bool> onComplete;

        [Header("Settings")]
        public float lockOutDuration = 0.1f;
        private float lockOutTimer = 0f;

        public System.Action onAttempt;

        public void StartAnticipation()
        {
            this.isGameActive = true;
            // this.isLockedOut = false; // Don't reset lockout here to allow persistent penalty
            this.isParryWindowOpen = false;

            gameObject.SetActive(true);
            Debug.Log("<color=white>[PARRY SYSTEM]</color> Đang quan sát... Đừng bấm sớm!");
        }

        public void StartGame(float duration, System.Action<bool> callback)
        {
            if (isGameActive && onComplete != null)
            {
                EndGame(false);
            }

            gameObject.SetActive(true);
            this.onComplete = callback;
            this.isGameActive = true; 

            // Removed immediate fail on lockout to allow recovery during the window
            this.isParryWindowOpen = true;

            StopAllCoroutines();
            StartCoroutine(WaitTimer(duration));
        }

        private void Update()
        {
            if (isLockedOut)
            {
                lockOutTimer -= Time.unscaledDeltaTime;
                if (lockOutTimer <= 0)
                {
                    isLockedOut = false;
                    Debug.Log("<color=white>[PARRY]</color> Hết thời gian Lock-out.");
                }
            }

            if (!isGameActive) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                onAttempt?.Invoke();

                if (isParryWindowOpen && !isLockedOut)
                {
                    Debug.Log("<color=green>[PARRY]</color> Thành công rực rỡ!");
                    EndGame(true);
                }
                else if (!isLockedOut)
                {
                    isLockedOut = true;
                    lockOutTimer = lockOutDuration;
                    Debug.Log("<color=red>[PARRY]</color> Bấm sai/sớm! Bị Lock-out 0.5s.");
                }
                else
                {
                    // Optionally reset timer on spam? 
                    // lockOutTimer = lockOutDuration; 
                    Debug.Log("<color=yellow>[PARRY]</color> Đang trong thời gian Lock-out...");
                }
            }
        }

        private IEnumerator WaitTimer(float duration)
        {
            float elapsed = 0;
            while (elapsed < duration && isGameActive)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (isGameActive && isParryWindowOpen)
            {
                Debug.Log("<color=red>[PARRY]</color> Quá muộn!");
                EndGame(false);
            }
        }

        private void EndGame(bool result)
        {
            if (!isGameActive) return;

            isGameActive = false;
            isParryWindowOpen = false;

            var tempCallback = onComplete;
            onComplete = null;

            tempCallback?.Invoke(result);

            gameObject.SetActive(false);

            // isLockedOut = false; // Persistent lockout timer handles this
        }

        public void ForceReset()
        {
            StopAllCoroutines();
            isGameActive = false;
            isParryWindowOpen = false;
            isLockedOut = false;
            gameObject.SetActive(false);
        }
    }
}