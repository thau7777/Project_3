using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class MainMenuButton : MonoBehaviour
    {
        public TextMeshProUGUI buttonText;
        public GameObject selectHighlightObj;
        public AudioSource hoverSound;
        public AudioSource activeSound;
        public Button button;
        public Color selectColor;
        public Color deselectColor;

        private void Start()
        {
            button = GetComponent<Button>();
        }

        public void SelectObject()
        {
            selectHighlightObj.SetActive(true);
            buttonText.color = selectColor;
            hoverSound.Play();
        }

        public void DeselectObject()
        {
            selectHighlightObj.SetActive(false);
            buttonText.color = deselectColor;
        }

        public void Submit()
        {
            button.onClick?.Invoke();
            activeSound.Play();
        }
    }
}