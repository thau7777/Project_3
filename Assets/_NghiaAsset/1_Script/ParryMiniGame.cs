using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Turnbase
{
    public class ParryMiniGame : MonoBehaviour
    {
        [Header("UI Components")]
        public Slider slider;
        public Image[] arrowImages;
        public Sprite arrowUp, arrowDown, arrowLeft, arrowRight;

        [Header("Settings")]
        public Color defaultColor = Color.white;
        public Color activeColor = Color.yellow;
        public Color successColor = Color.green;
        public Color failColor = Color.red;

        private List<KeyCode> requiredSequence = new List<KeyCode>();
        private int currentInputIndex = 0;
        private bool isGameActive = false;
        private bool hasFailedSequence = false;
        private System.Action<bool> onComplete;

        public void StartGame(float duration, System.Action<bool> callback)
        {
            this.onComplete = callback;
            this.currentInputIndex = 0;
            this.hasFailedSequence = false;
            this.isGameActive = true;

            GenerateRandomSequence();
            gameObject.SetActive(true);
            slider.value = 0;

            UpdateArrowVisuals();
            StartCoroutine(RunSlider(duration));
        }

        private void GenerateRandomSequence()
        {
            requiredSequence.Clear();
            for (int i = 0; i < arrowImages.Length; i++)
            {
                int rand = Random.Range(0, 4);
                switch (rand)
                {
                    case 0: requiredSequence.Add(KeyCode.UpArrow); arrowImages[i].sprite = arrowUp; break;
                    case 1: requiredSequence.Add(KeyCode.DownArrow); arrowImages[i].sprite = arrowDown; break;
                    case 2: requiredSequence.Add(KeyCode.LeftArrow); arrowImages[i].sprite = arrowLeft; break;
                    case 3: requiredSequence.Add(KeyCode.RightArrow); arrowImages[i].sprite = arrowRight; break;
                }
                arrowImages[i].color = defaultColor;
            }
        }

        private void Update()
        {
            if (!isGameActive) return;

            // 1. Nhập chuỗi mũi tên
            if (currentInputIndex < requiredSequence.Count)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                    Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (Input.GetKeyDown(requiredSequence[currentInputIndex]))
                    {
                        arrowImages[currentInputIndex].color = successColor;
                        currentInputIndex++;
                        UpdateArrowVisuals();
                    }
                    else
                    {
                        // Bấm sai hướng
                        arrowImages[currentInputIndex].color = failColor;
                        hasFailedSequence = true;
                        currentInputIndex++;
                        UpdateArrowVisuals();
                        Debug.Log("<color=orange>[MINI-GAME]</color> Bấm sai hướng!");
                    }
                }
            }

            // 2. Nhấn Space để chốt kết quả ngay lập tức
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckResult();
            }
        }

        private void UpdateArrowVisuals()
        {
            if (currentInputIndex < arrowImages.Length)
            {
                if (arrowImages[currentInputIndex].color == defaultColor)
                    arrowImages[currentInputIndex].color = activeColor;
            }
        }

        private void CheckResult()
        {
            // Điều kiện thắng duy nhất: Đã bấm đủ số phím và không có phím nào bị bấm sai
            bool isSequenceComplete = (currentInputIndex == requiredSequence.Count) && !hasFailedSequence;

            if (isSequenceComplete)
            {
                Debug.Log("<color=green>[SUCCESS]</color> Nhập đúng và chốt Space thành công!");
                EndGame(true);
            }
            else
            {
                Debug.Log("<color=red>[FAIL]</color> Chưa nhập xong hoặc đã nhập sai hướng!");
                EndGame(false);
            }
        }

        private IEnumerator RunSlider(float duration)
        {
            float elapsed = 0;
            while (elapsed < duration && isGameActive)
            {
                elapsed += Time.deltaTime;
                slider.value = elapsed / duration;
                yield return null;
            }

            if (isGameActive)
            {
                Debug.Log("<color=red>[FAIL]</color> Hết thời gian!");
                EndGame(false);
            }
        }

        private void EndGame(bool result)
        {
            isGameActive = false;
            onComplete?.Invoke(result);
            StartCoroutine(HideUIDelayed());
        }

        private IEnumerator HideUIDelayed()
        {
            yield return new WaitForSeconds(0.3f);
            gameObject.SetActive(false);
        }
    }
}