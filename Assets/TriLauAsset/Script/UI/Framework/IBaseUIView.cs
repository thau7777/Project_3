using UnityEngine;

namespace MyRule.UI
{ 
    public interface IBaseUIView
    {
        bool IsActive { get; set; }
        PanelType Type { get; }
        void Show();
        void Hide();
    }
}