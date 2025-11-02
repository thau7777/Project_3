using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public interface ISettingsView
    {
        void Select(Button button);
        void Deselect(Button button);
    }
}