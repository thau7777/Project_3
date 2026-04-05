using MyRule.Audio;
using UnityEngine;

namespace MyRule.UI
{
    public class SettingSliderView : SettingOptionView
    {
        public void OnChangeValue()
        {
            AudioManager.Instance.PlaySound("UIButtonClick");
        }   
    }
}