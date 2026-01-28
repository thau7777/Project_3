using UnityEngine;

namespace Turnbase
{
    public class EvadeMiniGame : MonoBehaviour
    {
        private bool isGameActive = false;
        private bool isLockedOut = false;
        private bool isEvadeWindowOpen = false;
        private System.Action<bool> onComplete;
        private float windowDuration;
        private float timer;

        public void StartAnticipation()
        {
            this.isGameActive = true;
            this.isLockedOut = false;
            this.isEvadeWindowOpen = false;
            gameObject.SetActive(true);
            Debug.Log("<color=cyan>[EVADE]</color> Đang theo dõi... Đừng nhấn Shift sớm!");
        }

        public void StartGame(float duration, System.Action<bool> callback)
        {
            this.onComplete = callback;
            this.isGameActive = true;

            if (isLockedOut)
            {
                Debug.Log("<color=red>[EVADE]</color> Thất bại! Bạn đã spam Shift sớm.");
                EndGame(false);
                return;
            }

            this.isEvadeWindowOpen = true;
            this.windowDuration = duration;
            this.timer = 0;
        }

        private void Update()
        {
            if (!isGameActive) return;

            if (isEvadeWindowOpen)
            {
                timer += Time.unscaledDeltaTime;
                if (timer >= windowDuration)
                {
                    EndGame(false); 
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (isEvadeWindowOpen && !isLockedOut)
                {
                    Debug.Log("<color=green>[EVADE]</color> Né đòn thành công!");
                    EndGame(true);
                }
                else if (!isEvadeWindowOpen && !isLockedOut)
                {
                    isLockedOut = true;
                    Debug.Log("<color=red>[EVADE]</color> Spam sớm! Bị khóa né.");
                }
            }
        }

        private void EndGame(bool success)
        {
            isGameActive = false;
            isEvadeWindowOpen = false;
            onComplete?.Invoke(success);
            gameObject.SetActive(false);
            isLockedOut = false;
        }
    }
}