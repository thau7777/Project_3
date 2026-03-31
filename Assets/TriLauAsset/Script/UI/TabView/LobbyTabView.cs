using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRule.UI
{
    public class LobbyTabView : BaseUIView
    {
        [SerializeField] private CanvasGroup tabContent;
        [SerializeField] private float fadeDuration = 0.5f;

        private TabFadePresenter presenter;

        protected override void OnEnable()
        {
            presenter = new TabFadePresenter(this, tabContent, fadeDuration);
        }

        protected override void OnDisable()
        {
            presenter?.Cleanup();
        }

        public void OpenTab()
        {
            EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(Type));
        }    

        public override void Hide()
        {
            tabContent.interactable = false;
            tabContent.blocksRaycasts = false;
        }

        public override void Show()
        {
            tabContent.interactable = true;
            tabContent.blocksRaycasts = true;
        }
    }
}