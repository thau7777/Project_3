using System;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class HistoryMatchInfoView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI session;
        [SerializeField] private TextMeshProUGUI timePlayed;
        [SerializeField] private TextMeshProUGUI className;
        [SerializeField] private TextMeshProUGUI enmiesDefeated;
        [SerializeField] private TextMeshProUGUI nodesExplored;
        [SerializeField] private TextMeshProUGUI damage;
        [SerializeField] private TextMeshProUGUI defense;
        [SerializeField] private LobbySigilView[] lobbySigilViews;

        public void Show()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void SetMatchValue(int index, HistotyMatchData matchData)
        {
            ResetHistorySigilView();

            session.text = (index + 1).ToString(); 
            TimeSpan timeSpan = TimeSpan.FromSeconds(matchData.TimePlayed);
            timePlayed.text = timeSpan.ToString(@"hh\:mm\:ss");
            className.text = matchData.CharacterClass.ToString();
            enmiesDefeated.text = matchData.EnemiesDefeated.ToString();
            nodesExplored.text = matchData.NodesExplored.ToString();
            damage.text = matchData.DamageInflicted.ToString();
            defense.text = matchData.DamagePrevented.ToString();

            for (int i = 0; i < matchData.SigilStorageInMatch.ActiveSigils.Length; i++)
            {
                SigilData sigilData = matchData.SigilStorageInMatch.ActiveSigils[i];
                if (sigilData == null) continue;
                lobbySigilViews[i].gameObject.SetActive(true);
                lobbySigilViews[i].SetSigil(matchData.SigilStorageInMatch.ActiveSigils[i]);
            }
        }

        private void ResetHistorySigilView()
        {
            for (int i = 0; i < lobbySigilViews.Length; i++)
            {
                lobbySigilViews[i].gameObject.SetActive(false);
            }
        }
    }
}