using MyRule.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class AchievementsView : MonoBehaviour
    {
        [SerializeField] private AchievementInfoView[] _achievementInfoViews;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private UnityEvent onEscEvent;

        private int totalAchievements;     
        private int columnCount = 1;
        private int visibleRows = 5;

        private int currentRow = 0;
        private int totalRows;

        private bool isShowing = false;

        private EventBinding<UpdateAchievementEvent> _updateAchievementsEventBinding;

        private void OnEnable()
        {
            _updateAchievementsEventBinding = new EventBinding<UpdateAchievementEvent>(HandleAchievements);
            EventBus<UpdateAchievementEvent>.Register(_updateAchievementsEventBinding);

            _inputReader.uiActions.onNavigate += HandleNavigate;
            _inputReader.uiActions.onCancel += BlockNavigate;
        }

        private void OnDisable()
        {
            EventBus<UpdateAchievementEvent>.Deregister(_updateAchievementsEventBinding);

            _inputReader.uiActions.onNavigate -= HandleNavigate;
            _inputReader.uiActions.onCancel -= BlockNavigate;
        }

        private void HandleAchievements(UpdateAchievementEvent evt)
        {
            totalAchievements = evt.achievementDatas.Count;
            totalRows = Mathf.CeilToInt((float)totalAchievements / columnCount);

            for (int i = 0; i < evt.achievementDatas.Count; i++)
            {
                if (_achievementInfoViews[i] != null)
                {
                    _achievementInfoViews[i].SetAchievement(evt.achievementDatas[i]);
                }
            }
        }

        private void HandleNavigate(Vector2 input)
        {
            if (!isShowing) return;

            if (input.y > 0)
            {
                ScrollUp();
            }
            else if (input.y < 0)
            {
                ScrollDown();
            }

            EventSystem.current.SetSelectedGameObject(_achievementInfoViews[currentRow].gameObject);
        }

        private void ScrollDown()
        {
            if (currentRow < totalRows - 1)
            {
                currentRow++;

                if (currentRow >= visibleRows)
                {
                    UpdateScroll();
                }
            }
        }

        private void ScrollUp()
        {
            if (currentRow > 0)
            {
                currentRow--;

                if (currentRow < totalRows - visibleRows)
                {
                    UpdateScroll();
                }
            }
        }

        private void UpdateScroll()
        {
            if (totalRows <= visibleRows) return;

            float step = 1f / (totalRows - visibleRows);
            float target = 1f - (currentRow * step);

            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(target);
        }

        public void ResetAchiementIndex()
        {
            currentRow = 0;

            isShowing = true;

            EventSystem.current.SetSelectedGameObject(_achievementInfoViews[currentRow].gameObject);
        }

        public void BlockNavigate()
        {
            if (!isShowing) return;

            isShowing = false;
            onEscEvent?.Invoke();
        }
    }
}