using Cysharp.Threading.Tasks;
using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class HistoryManager : PersistentSingleton<HistoryManager>, IGameData
    {
        private HistoryData historyData;

        public HistoryData HistoryData => historyData;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public void AddMatchToHistory(MatchData matchData) => historyData.AddMatch(matchData);

        public UniTask LoadData(GameData data)
        {
            historyData = new HistoryData();

            if (data.History != null && data.History.Matchs != null)
            {
                historyData = data.History;
                EventBus<UpdateHistoryEvent>.Raise(new UpdateHistoryEvent(historyData));
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.SetHistory(historyData);
        }
    }
}