using MyRule.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRule.UI
{
    public class SettingSelectorPresenter
    {
        private ISettingSelector view;
        private int currentOptionIndex;
        private int optionCount;

        public SettingSelectorPresenter(ISettingSelector view, int currentOptionIndex, int optionCount)
        {
            this.view = view;

            this.currentOptionIndex = currentOptionIndex;
            this.optionCount = optionCount;
        }

        public int GetCurrentOptionIndex(MoveDirection moveDir)
        {
            if (moveDir == MoveDirection.Left)
            {
                currentOptionIndex -= 1;
                AudioManager.Instance.PlaySound("UIButtonClick");
            }
            else if (moveDir == MoveDirection.Right)
            {
                currentOptionIndex += 1;
                AudioManager.Instance.PlaySound("UIButtonClick");
            }

            if (currentOptionIndex < 0)
            {
                currentOptionIndex = optionCount - 1;
            }
            else if (currentOptionIndex >= optionCount)
            {
                currentOptionIndex = 0;
            }

            return currentOptionIndex;
        }
    }
}