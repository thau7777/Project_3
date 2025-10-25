using TMPro;
using UnityEngine;

namespace MyRule
{
    public class MainMenuButton : MonoBehaviour
    {
        public TextMeshProUGUI buttonText;
        public GameObject selectHighlightObj;

        private void Start()
        {
            DeselectObject();
        }

        public void SelectObject()
        {
            selectHighlightObj.SetActive(true);
            buttonText.color = Color.white;
        }

        public void DeselectObject()
        {
            selectHighlightObj.SetActive(false);
            buttonText.color = Color.gray;
        }
    }
}