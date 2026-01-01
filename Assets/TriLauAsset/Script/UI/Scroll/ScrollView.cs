using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class ScrollView : MonoBehaviour, IScrollView
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private List<ButtonView> buttons;
        [SerializeField] private GameObject arrowUp;
        [SerializeField] private GameObject arrowDown;

        private ScrollPresenter presenter;

        public ScrollRect ScrollRect => scrollRect;
        public List<ButtonView> Contents => buttons;
        
        private void OnEnable()
        {
            presenter = new ScrollPresenter(this);

            arrowDown.SetActive(true);
            arrowUp.SetActive(false);
        }

        private void OnDisable()
        {
            presenter.CleanUp();
        }

        private void Start()
        {
           //EventBus<SelectButtonEvent>.Raise(new SelectButtonEvent(buttons[0]));
        }

        public void ShowArrowUp() => arrowUp.SetActive(true);
        public void HideArrowUp() => arrowUp.SetActive(false);
        public void ShowArrowDown() => arrowDown.SetActive(true);
        public void HideArrowDown() => arrowDown.SetActive(false);
    }
}