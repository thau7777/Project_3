using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRule.UI
{
    public class SettingSelectorView : SettingOptionView, ISettingSelector, IMoveHandler
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI adjustTxt;
        [SerializeField] private List<string> optionList;

        [Header("Navigation")]
        [SerializeField] private GameObject upObj;
        [SerializeField] private GameObject downObj;

        private int defaultOptionIndex = 0;

        private int currentOptionIndex;

        private SettingSelectorPresenter presenter;

        protected override async void Start()
        {
            base.Start();

            await LoadOption();

            presenter = new SettingSelectorPresenter(this, currentOptionIndex, optionList.Count);
        }

        private void SetAdjustText(int optionIndex)
        {
            if (adjustTxt != null && optionList != null && optionIndex >= 0 && optionIndex < optionList.Count)
            {
                adjustTxt.text = optionList[optionIndex];
                PlayerPrefs.SetInt(settingKey, optionIndex);
            }
        }

        private void Navigate(MoveDirection moveDir)
        {
            if (moveDir == MoveDirection.Up && upObj != null)
            {
                EventSystem.current.SetSelectedGameObject(upObj);
            }
            else if (moveDir == MoveDirection.Down && downObj != null)
            {
                EventSystem.current.SetSelectedGameObject(downObj);
            }
        }

        private UniTask LoadOption()
        {
            currentOptionIndex = PlayerPrefs.GetInt(settingKey, defaultOptionIndex);

            SetAdjustText(currentOptionIndex);

            return UniTask.CompletedTask;
        }

        public void OnMove(AxisEventData eventData)
        {
            defaultOptionIndex = presenter.GetCurrentOptionIndex(eventData.moveDir);

            SetAdjustText(defaultOptionIndex);

            Navigate(eventData.moveDir);
        }
    }
}