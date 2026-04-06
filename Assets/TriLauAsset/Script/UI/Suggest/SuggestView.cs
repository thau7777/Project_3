using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyRule
{
    public class SuggestView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image[] images;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private string[] suggestDes;
        [SerializeField] private Button continueBtn;

        private int currentIndex = 0;

        private void Start()
        {
            images[currentIndex].gameObject.SetActive(true);
            text.text = suggestDes[currentIndex];
            continueBtn.onClick.AddListener(Onclick);
        }

        private void Onclick()
        {
            images[currentIndex].gameObject.SetActive(false);
            if (currentIndex + 1 >= images.Length)
            {
                Hide();
                return;
            }

            currentIndex++;
            images[currentIndex].gameObject.SetActive(true);
            text.text = suggestDes[currentIndex];
        }

        public async void Show()
        {
            string scenceName = SceneManager.GetActiveScene().name;
            if (scenceName == "SpaceStationScene")
            {
                Cursor.lockState = CursorLockMode.None;
            }
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1f, 0.4f);

            currentIndex = 0;
            images[currentIndex].gameObject.SetActive(true);
            text.text = suggestDes[currentIndex];

            await UniTask.Delay(400);
            Time.timeScale = 0;
        }

        public void Hide()
        {
            Time.timeScale = 1;
            string scenceName = SceneManager.GetActiveScene().name;
            if (scenceName == "SpaceStationScene")
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0f, 0.4f);

            for (int i = 0; i < images.Length; i++)
            {
                images[i].gameObject.SetActive(false);
            }
        }
    }

}