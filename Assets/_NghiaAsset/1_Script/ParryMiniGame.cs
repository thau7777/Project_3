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

        public void StartAnticipation()
        {
            this.isGameActive = true;
            this.isLockedOut = false;
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

            if (isLockedOut)
            {
                Debug.Log("<color=red>[PARRY]</color> Bị phạt do bấm sớm!");
                EndGame(false);
                return;
            }

            this.isParryWindowOpen = true;

            StopAllCoroutines();
            StartCoroutine(WaitTimer(duration));
        }

        private void Update()
        {
            if (!isGameActive) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isParryWindowOpen && !isLockedOut)
                {
                    Debug.Log("<color=green>[PARRY]</color> Thành công rực rỡ!");
                    EndGame(true);
                }
                else if (!isParryWindowOpen && !isLockedOut)
                {
                    isLockedOut = true;
                    Debug.Log("<color=red>[PARRY]</color> Bấm quá sớm! Bạn bị Lock-out.");
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

            isLockedOut = false;
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