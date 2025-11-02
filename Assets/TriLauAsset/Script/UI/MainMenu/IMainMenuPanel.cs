using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public interface IMainMenuPanel
    {
        public ButtonView CurrentButton { get; set; }

        void ShowMenuButtons();

        void HideAnyButton();
    }
}