using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class TabFadeView : BaseUIView
    {
        [SerializeField] private CanvasGroup tabContent;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Button firstSelect;

        private TabFadePresenter presenter;

        protected override void OnEnable()
        {
            presenter = new TabFadePresenter(this, tabContent , fadeDuration);
        }

        protected override void OnDisable()
        {
            presenter?.Cleanup();
        }

        public override void Hide()
        {
            tabContent.gameObject.SetActive(false);
            VolumeController.Instance.AdjustVolumeWeight();
        }

        public override void Show()
        {
            tabContent.gameObject.SetActive(true);
            firstSelect?.Select();
            VolumeController.Instance.AdjustVolumeWeight();
        }
    }
}