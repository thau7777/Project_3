using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public interface IMainMenuPanel
    {
        public PanelType Type { get; set; }
    }
}