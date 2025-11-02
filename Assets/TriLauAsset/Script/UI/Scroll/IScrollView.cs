using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public interface IScrollView
    {
        ScrollRect ScrollRect { get; }
        List<ButtonView> Contents { get; }
        void ShowArrowUp();
        void HideArrowUp();
        void ShowArrowDown();
        void HideArrowDown();
    }
}