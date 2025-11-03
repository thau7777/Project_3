using UnityEngine;

namespace MyRule.UI
{ 
    public interface IBaseUIView
    {
        PanelType Type { get; }
        void Show();
        void Hide();
    }
}