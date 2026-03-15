using MyRule.Event;
using MyRule.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class HistoryView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject noHistory;
        [SerializeField] private TextMeshProUGUI session;
        [SerializeField] private TextMeshProUGUI className;
        [SerializeField] private TextMeshProUGUI enmiesDefeated;
        [SerializeField] private TextMeshProUGUI nodesExplored;
        [SerializeField] private TextMeshProUGUI reward;
        [SerializeField] private Transform sigilContentsParent;
        [SerializeField] private GameObject sigilPreb;
        [SerializeField] private GameObject arrowPrev;
        [SerializeField] private GameObject arrowNext;

        private HistoryData historyData;

        private List<GameObject> historySigilObjs = new List<GameObject>();

        private EventBinding<UpdateHistoryEvent> updateHistoryEvent;

        private void OnEnable()
        {
            updateHistoryEvent = new EventBinding<UpdateHistoryEvent>(UpdateHistoryView);
            EventBus<UpdateHistoryEvent>.Register(updateHistoryEvent);
        }

        private void OnDisable()
        {
            EventBus<UpdateHistoryEvent>.Deregister(updateHistoryEvent);
        }

        private void Start()
        {
            arrowPrev.gameObject.SetActive(false);
            arrowNext.gameObject.SetActive(false);
        }

        private void UpdateHistoryView(UpdateHistoryEvent evt)
        {
            this.historyData = evt.HistoryData;
            canvasGroup.alpha = 1;
            noHistory.SetActive(false);
            SetMatchValue(0, evt.HistoryData.Matchs[0]);
        }

        private void SetMatchValue(int index, MatchData matchData)
        {
            ResetHistorySigilView();

            session.text = index.ToString();
            className.text = matchData.CharacterData.CharacterClass.ToString();
            enmiesDefeated.text = matchData.EnemiesDefeated.ToString();
            nodesExplored.text = matchData.NodesExplored.ToString();

            foreach (var sigil in matchData.SigilStorageInMatch.ActiveSigils)
            {
                GameObject sigilViewObj = Instantiate(sigilPreb, sigilContentsParent);
                LobbySigilView historySigilView = sigilViewObj.GetComponent<LobbySigilView>();
                historySigilView.SetSigil(sigil.Value);
                historySigilObjs.Add(sigilViewObj);
            }
        }

        private void ResetHistorySigilView()
        {
            foreach (var sigilView in historySigilObjs)
            {
                Destroy(sigilView.gameObject);
            }

            historySigilObjs.Clear();
        }
    }
}