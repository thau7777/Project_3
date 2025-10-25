using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace MyRule
{
    public class MenuScrollSelector : MonoBehaviour
    {
        [Header("UI References")]
        public List<MainMenuButton> buttons = new List<MainMenuButton>();
        public GameObject ArrowUp;
        public GameObject ArrowDown;

        private int currentIndex = 0;
        private float temp;
        
        void Start()
        {
            if (buttons.Count == 0) return;

            buttons[0].SelectObject();
            temp = 1;
            ArrowUp.SetActive(false);
            ArrowDown.SetActive(true);
        }

        public void Scroll(Vector2 scrollValue)
        {
            Debug.Log("Scroll Value: " + scrollValue);
            if (buttons.Count == 0) return;

            if (scrollValue.y > temp)
            {
                // Scrolling up
                buttons[currentIndex].DeselectObject();
                currentIndex = Mathf.Max(0, currentIndex - 1);
                buttons[currentIndex].SelectObject();
            }
            else if (scrollValue.y < temp)
            {
                // Scrolling down
                buttons[currentIndex].DeselectObject();
                currentIndex = Mathf.Min(buttons.Count - 1, currentIndex + 1);
                buttons[currentIndex].SelectObject();
            }

            if (currentIndex == 0)
            {
                ArrowUp.SetActive(false);
            }
            else
            {
                ArrowUp.SetActive(true);
            }

            if (currentIndex == buttons.Count - 3)
            {
                ArrowDown.SetActive(false);
            }
            else
            {
                ArrowDown.SetActive(true);
            }

            temp = scrollValue.y;
        }
    }
}