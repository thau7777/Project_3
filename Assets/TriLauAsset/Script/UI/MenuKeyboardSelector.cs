using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace MyRule
{
    public class MenuKeyboardSelector : MonoBehaviour
    {
        [Header("UI References")]
        public ScrollRect scrollRect;
        public List<MainMenuButton> buttons = new List<MainMenuButton>();
        public GameObject ArrowUp;
        public GameObject ArrowDown;

        [Header("Settings")]
        public float scrollSpeed = 0.2f;
        public float selectDelay = 0.15f;

        private int currentIndex = 0;
        private float nextSelectTime;

        public InputReader inputReader;

        private void Awake()
        {
            inputReader.SwitchActionMap(ActionMap.UI);
        }

        private void OnEnable()
        {
            inputReader.uiActions.onNavigate += OnNavigate;
            inputReader.uiActions.onSubmit += OnSubmit;
        }

        private void OnDisable()
        {
            inputReader.uiActions.onNavigate -= OnNavigate;
            inputReader.uiActions.onSubmit -= OnSubmit;
        }

        private void Start()
        {
            if (buttons.Count == 0) return;

            buttons[0].SelectObject();
            ArrowUp.SetActive(false);
            ArrowDown.SetActive(true);
        }

        private void OnNavigate(Vector2 input)
        {
            if (Time.time < nextSelectTime)
                return;

            if (input.y > 0.5f)
            {
                // Scrolling up
                buttons[currentIndex].DeselectObject();
                currentIndex = Mathf.Max(0, currentIndex - 1);
                buttons[currentIndex].SelectObject();
                ScrollView(currentIndex);
            }
            else if (input.y < -0.5f)
            {
                // Scrolling down
                buttons[currentIndex].DeselectObject();
                currentIndex = Mathf.Min(buttons.Count - 1, currentIndex + 1);
                buttons[currentIndex].SelectObject();
                ScrollView(currentIndex);
            }

            if (currentIndex <= 1)
            {
                ArrowUp.SetActive(false);
            }
            else
            {
                ArrowUp.SetActive(true);
            }

            if (currentIndex >= buttons.Count - 2)
            {
                ArrowDown.SetActive(false);
            }
            else
            {
                ArrowDown.SetActive(true);
            }

            nextSelectTime = Time.time + selectDelay;
        }

        private void OnSubmit()
        {
            if (buttons.Count > 0 && currentIndex >= 0 && currentIndex < buttons.Count)
            {
                buttons[currentIndex].Submit();
            }
        }

        private void ScrollView(int index)
        {
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);

            RectTransform content = scrollRect.content;
            RectTransform target = buttons[index].GetComponent<RectTransform>();

            float viewportHeight = scrollRect.viewport.rect.height;
            float contentHeight = content.rect.height;
            float targetPosY = Mathf.Abs(target.anchoredPosition.y);

            float normalizedPos = Mathf.Clamp01((targetPosY - viewportHeight / 2f) / (contentHeight - viewportHeight));
            scrollRect.verticalNormalizedPosition = 1f - normalizedPos;
        }
    }
}