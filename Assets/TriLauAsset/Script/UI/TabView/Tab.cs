using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MyRule.UI
{
    public class Tab : MonoBehaviour
    {
        [SerializeField] private GameObject selectedLine;
        [SerializeField] private TextMeshProUGUI tabText;
        [SerializeField] private Color normalColor = Color.gray;
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private CanvasGroup tabContent;

        [Header("Events")]
        [SerializeField] private UnityEvent onFirstSelect;

        public void SetSelected(bool isSelected)
        {
            if (selectedLine != null)
                selectedLine.SetActive(isSelected);

            if (!isSelected)
            {
                ClearCurrentSelectionIfInsideTab();
            }

            if (tabText != null)
                tabText.color = isSelected ? selectedColor : normalColor;

            if (isSelected)
            {
                tabContent.alpha = 1;
            }
            else
            {
                tabContent.alpha = 0;
            }

            if (isSelected)
            {
                onFirstSelect?.Invoke();
            }
        }

        private void ClearCurrentSelectionIfInsideTab()
        {
            if (EventSystem.current == null) return;

            var current = EventSystem.current.currentSelectedGameObject;
            if (current == null) return;

            if (tabContent != null && current.transform.IsChildOf(tabContent.transform))
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
