using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class UIGridNavigator : MonoBehaviour
    {
        [SerializeField] private int columnCount;
        [SerializeField] private int visibleRows;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject[] views;
        [SerializeField] private InputReader inputReader;

        private int totalViews;
        private int currentRow = 0;
        private int currentColumn = 0;
        private int rowCount;
        private bool isShowing = false;
        private GameObject[,] grid;

        private void OnEnable()
        {
            inputReader.uiActions.onNavigate += HandleNavigate;
        }

        private void OnDisable()
        {
            inputReader.uiActions.onNavigate -= HandleNavigate;
        }

        private void Awake()
        {
            totalViews = views.Length;
            rowCount = Mathf.CeilToInt((float)totalViews / columnCount);

            grid = new GameObject[columnCount, rowCount];
            
            for (int i = 0; i < views.Length; i++)
            {
                int row = i / columnCount;
                int col = i % columnCount;
                grid[row, col] = views[i];
            }
        }

        public void FirstSelect()
        {
            GameObject selected = grid[currentRow, currentColumn];
            if (selected != null)
                EventSystem.current.SetSelectedGameObject(selected);
            UpdateScroll();

            isShowing = true;
        }

        public void ResetView()
        {
            if (!isShowing) return;

            currentRow = 0; currentColumn = 0;

            isShowing = false;
        }

        private void HandleNavigate(Vector2 input)
        {
            if (!isShowing) return;

            if (input.y > 0)
                ScrollUp();
            else if (input.y < 0)
                ScrollDown();
            else if (input.x > 0)
                TurnRight();
            else if (input.x < 0)
                TurnLeft();

            GameObject selected = grid[currentRow, currentColumn];
            if (selected != null)
                EventSystem.current.SetSelectedGameObject(selected);
        }

        private void ScrollDown()
        {
            if (currentRow < rowCount - 1)
            {
                currentRow++;
                if (currentRow >= visibleRows)
                    UpdateScroll();
            }
        }

        private void ScrollUp()
        {
            if (currentRow > 0)
            {
                currentRow--;
                if (currentRow < rowCount - visibleRows)
                    UpdateScroll();
            }
        }

        private void TurnRight()
        {
            if (currentColumn < columnCount - 1)
            {
                currentColumn++;
            }
        }

        private void TurnLeft()
        {
            if (currentColumn > 0)
            {
                currentColumn--;
            }
        }

        private void UpdateScroll()
        {
            if (scrollRect == null) return;
            if (rowCount <= visibleRows) return;

            float step = 1f / (rowCount - visibleRows);
            float target = 1f - (currentRow * step);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(target);
        }
    }
}