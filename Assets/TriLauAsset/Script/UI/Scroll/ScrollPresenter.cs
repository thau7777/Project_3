using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public class ScrollPresenter
    {
        IScrollView view;

        private int currentIndex = 0;

        private int count = 0;

        private EventBinding<MovePressEvent> movePressedEventBinding;

        public ScrollPresenter(IScrollView scrollView)
        {
            this.view = scrollView;

            this.count = view.Contents != null ? view.Contents.Count : 0;

            movePressedEventBinding = new EventBinding<MovePressEvent>(HandleMoveEvent);
            EventBus<MovePressEvent>.Register(movePressedEventBinding);
        }

        public void HandleMoveEvent(MovePressEvent movePressEvent)
        {
            if (movePressEvent.Vertical > 0)
            {
                Move(-1);
            }
            else if (movePressEvent.Vertical < 0)
            {
                Move(1);
            }
        }

        private void Move(int direction)
        {
            if (view.Contents == null || count == 0) return;

            currentIndex = Mathf.Clamp(currentIndex + direction, 0, count - 1);

            EventBus<SelectButtonEvent>.Raise(new SelectButtonEvent(view.Contents[currentIndex]));

            ScrollTo(view.Contents[currentIndex].gameObject);

            ShowArrows(currentIndex, count);
        }

        private void ScrollTo(GameObject buttonTarget)
        {
            RectTransform content = view.ScrollRect.content;
            RectTransform target = buttonTarget.GetComponent<RectTransform>();

            float viewportHeight = view.ScrollRect.viewport.rect.height;
            float contentHeight = content.rect.height;
            float targetPosY = Mathf.Abs(target.anchoredPosition.y);

            float normalizedPos = Mathf.Clamp01((targetPosY - viewportHeight / 2f) / (contentHeight - viewportHeight));
            view.ScrollRect.verticalNormalizedPosition = 1f - normalizedPos;
        }

        private void ShowArrows(int index, int count)
        {
            if (index <= 1)
            {
                view.HideArrowUp();
            }
            else
            {
                view.ShowArrowUp();
            }

            if (index >= count - 2)
            {
                view.HideArrowDown();
            }
            else
            {
                view.ShowArrowDown();
            }
        }

        public void CleanUp()
        {
            view = null;

            EventBus<MovePressEvent>.Deregister(movePressedEventBinding);
        }
    }
}