using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class UIScrollPresenter
    {
        private int totalItems;
        private int columnCount = 1;
        private int visibleRows = 5;

        private int currentRow = 0;
        private int totalRows;

        private ScrollRect scrollRect;

        public UIScrollPresenter(int totalItems, int columnCount, int visibleRows, int currentRow, int totalRows, ScrollRect scrollRect)
        {
            this.totalItems = totalItems;
            this.columnCount = columnCount;
            this.visibleRows = visibleRows;
            this.currentRow = currentRow;
            this.totalRows = totalRows;
            this.scrollRect = scrollRect;
        }

        public void HandleNavigate(Vector2 input)
        {
            if (input.y > 0)
            {
                ScrollUp();
            }
            else if (input.y < 0)
            {
                ScrollDown();
            }
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

            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(target);
        }
    }
}