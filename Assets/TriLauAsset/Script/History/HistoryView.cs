using MyRule.Event;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class HistoryView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject noHistory;
        [SerializeField] private Button arrowPrev;
        [SerializeField] private Button arrowNext;
        [SerializeField] private HistoryMatchInfoView[] historyMatchInfoViews;

        private int currentIndex = 0;

        private EventBinding<UpdateHistoryEvent> updateHistoryEvent;

        private void OnEnable()
        {
            updateHistoryEvent = new EventBinding<UpdateHistoryEvent>(UpdateHistoryView);
            EventBus<UpdateHistoryEvent>.Register(updateHistoryEvent);

            arrowPrev.onClick.AddListener(PreviousClick);
            arrowNext.onClick.AddListener(NextClick);
        }

        private void OnDisable()
        {
            EventBus<UpdateHistoryEvent>.Deregister(updateHistoryEvent);

            arrowPrev.onClick.RemoveListener(PreviousClick);
            arrowNext.onClick.RemoveListener(NextClick);
        }

        private void Start()
        {
            arrowPrev.gameObject.SetActive(false);
            arrowNext.gameObject.SetActive(false);
        }

        private void UpdateHistoryView(UpdateHistoryEvent evt)
        {
            canvasGroup.alpha = 1;
            if (evt.HistoryData.Matchs[0] != null) noHistory.SetActive(false);

            if (evt.HistoryData.Matchs[1] != null) arrowNext.gameObject.SetActive(true);

            for (int i = 0; i < evt.HistoryData.Matchs.Length; i++)
            {
                HistotyMatchData historyMatchData = evt.HistoryData.Matchs[i];
                
                if (historyMatchData == null) continue;

                historyMatchInfoViews[i].gameObject.SetActive(true);
                historyMatchInfoViews[i].SetMatchValue(i, evt.HistoryData.Matchs[i]);
            }
        }

        private void PreviousClick()
        {
            if (currentIndex <= 0) return;
            historyMatchInfoViews[currentIndex].Hide();
            currentIndex--;
            historyMatchInfoViews[currentIndex].Show();
            if (currentIndex == 0) arrowPrev.gameObject.SetActive(false);

        }

        private void NextClick()
        {
            if (currentIndex >= historyMatchInfoViews.Length - 1) return;
            historyMatchInfoViews[currentIndex].Hide();
            currentIndex++;
            historyMatchInfoViews[currentIndex].Show();
            if (currentIndex == historyMatchInfoViews.Length - 1) arrowNext.gameObject.SetActive(false);
        }
    }
}